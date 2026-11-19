using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VoiceControl.Audio;
using VoiceControl.Settings;

namespace VoiceControl;

/// <summary>
/// Saisie vocale via Gradium : stream du PCM micro sur le WebSocket
/// wss://api.gradium.ai/api/speech/asr (protocole JSON : setup, chunks binaires,
/// flush, stop) et collecte des messages "transcript" jusqu'au segment final.
/// La fin de parole détectée par le serveur (is_final) ou une annulation externe
/// (ex: touche d'arrêt) stoppent la capture et renvoient le transcript reçu.
/// </summary>
public class GradiumVoiceInput : IVoiceInput
{
    private readonly IVoiceSettingsProvider _settingsProvider;
    private readonly IAudioSource _audioSource;

    public GradiumVoiceInput(IVoiceSettingsProvider settingsProvider, IAudioSource audioSource)
    {
        _settingsProvider = settingsProvider;
        _audioSource = audioSource;
    }

    public async Task<string> CaptureTextAsync(CancellationToken cancellationToken = default)
    {
        string apiKey = _settingsProvider.RequireGradiumApiKey();
        VoiceSettings settings = _settingsProvider.Settings;
        using var log = new DebugLog(settings.Gradium.DebugLogPath);

        using var socket = new ClientWebSocket();
        socket.Options.SetRequestHeader("x-api-key", apiKey);

        // Lié au token externe : une annulation (touche d'arrêt) ou la fin de parole
        // détectée par le serveur coupent la capture.
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        log.Write($"connexion à {settings.Gradium.AsrWebSocketUrl}");
        await socket.ConnectAsync(new Uri(settings.Gradium.AsrWebSocketUrl), cts.Token);
        log.Write("connecté");

        // input_format est une chaîne : "pcm_16000", "pcm_24000", etc. (PCM 16 bits mono au sample rate indiqué).
        string setupJson = JsonSerializer.Serialize(new
        {
            type = "setup",
            model_name = settings.Gradium.ModelName,
            input_format = $"pcm_{settings.AudioCapture.SampleRate}",
            json_config = new { language = settings.Gradium.Language, delay_in_frames = settings.Gradium.DelayInFrames },
        });
        await SendTextAsync(socket, setupJson, cts.Token);
        log.Write($"setup envoyé : {setupJson}");

        Task<string> receiveTask = ReceiveTranscriptsAsync(socket, cts, log, settings.Gradium.VadStopTimeoutMs);

        try
        {
            int frameMs = settings.AudioCapture.FrameDurationMs;
            await foreach (byte[] chunk in _audioSource.ReadChunksAsync(cts.Token))
            {
                await SendAudioAsync(socket, chunk, cts.Token);
                // Cadence temps réel : une frame toutes les frameMs (80 ms par défaut).
                if (settings.AudioCapture.RealtimePacing)
                {
                    await Task.Delay(frameMs, cts.Token);
                }
            }
            log.Write("source audio terminée, envoi flush/end_of_stream");
            await SendTextAsync(socket, """{"type":"flush","flush_id":1}""", CancellationToken.None);
            await SendTextAsync(socket, """{"type":"end_of_stream"}""", CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            log.Write("capture interrompue (fin de parole ou annulation), envoi flush/end_of_stream");
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    await SendTextAsync(socket, """{"type":"flush","flush_id":1}""", CancellationToken.None);
                    await SendTextAsync(socket, """{"type":"end_of_stream"}""", CancellationToken.None);
                }
            }
            catch (WebSocketException) { }
        }
        catch (WebSocketException ex)
        {
            log.Write($"envoi interrompu (serveur a fermé) : {ex.Message}");
        }

        // On attend d'abord la réponse du serveur (text restants + end_of_stream),
        // sinon la fermeture côté client couperait les derniers messages en vol.
        string transcript = await receiveTask;
        log.Write($"transcript final : \"{transcript}\"");

        try
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "fin de saisie", CancellationToken.None);
            }
        }
        catch (WebSocketException)
        {
            // Le serveur a déjà fermé la connexion.
        }
        return transcript;
    }

    private static async Task SendTextAsync(ClientWebSocket socket, string message, CancellationToken ct)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }

    /// <summary>Envoie une frame audio : message JSON texte {"type":"audio","audio":"base64"}.</summary>
    private static async Task SendAudioAsync(ClientWebSocket socket, byte[] pcmFrame, CancellationToken ct)
    {
        string json = $$"""{"type":"audio","audio":"{{Convert.ToBase64String(pcmFrame)}}"}""";
        await SendTextAsync(socket, json, ct);
    }

    private static async Task<string> ReceiveTranscriptsAsync(ClientWebSocket socket, CancellationTokenSource cts, DebugLog log, int vadStopTimeoutMs)
    {
        var transcript = new StringBuilder();
        byte[] buffer = new byte[8192];

        // Arrêt automatique : après un end_text, si aucun nouveau segment n'arrive
        // pendant vadStopTimeoutMs, on considère la parole terminée et on stoppe la capture.
        DateTime lastSegmentAt = DateTime.MinValue;
        bool endTextSeen = false;
        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await Task.Delay(200);
                if (endTextSeen && vadStopTimeoutMs > 0
                    && DateTime.Now - lastSegmentAt > TimeSpan.FromMilliseconds(vadStopTimeoutMs))
                {
                    log.Write("inactivité après end_text, arrêt automatique de la capture");
                    cts.Cancel();
                    return;
                }
            }
        });

        while (socket.State == WebSocketState.Open)
        {
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;
            try
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);
            }
            catch (WebSocketException ex)
            {
                log.Write($"réception interrompue : {ex.Message}");
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                log.Write("connexion fermée par le serveur");
                break;
            }

            string json = Encoding.UTF8.GetString(ms.ToArray());
            log.Write($"reçu : {json}");
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("type", out JsonElement type))
            {
                continue;
            }
            switch (type.GetString())
            {
                case "text":
                    if (root.TryGetProperty("text", out JsonElement text))
                    {
                        string? chunk = text.GetString();
                        if (!string.IsNullOrWhiteSpace(chunk))
                        {
                            if (transcript.Length > 0 && !char.IsWhiteSpace(transcript[^1]))
                            {
                                transcript.Append(' ');
                            }
                            transcript.Append(chunk);
                            lastSegmentAt = DateTime.Now;
                            endTextSeen = false;
                        }
                    }
                    break;
                case "end_text":
                    // Segment finalisé par le VAD : on arme le timer d'inactivité.
                    log.Write("end_text reçu (segment finalisé)");
                    lastSegmentAt = DateTime.Now;
                    endTextSeen = true;
                    break;
                case "end_of_stream":
                    log.Write("end_of_stream reçu");
                    return transcript.ToString().Trim();
                case "error":
                    log.Write($"ERREUR serveur : {json}");
                    return transcript.ToString().Trim();
            }
        }
        return transcript.ToString().Trim();
    }

    /// <summary>Log texte simple vers un fichier si un chemin est configuré.</summary>
    private sealed class DebugLog : IDisposable
    {
        private readonly StreamWriter? _writer;

        public DebugLog(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            _writer = new StreamWriter(path, append: false) { AutoFlush = true };
        }

        public void Write(string message)
        {
            _writer?.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        }

        public void Dispose() => _writer?.Dispose();
    }
}
