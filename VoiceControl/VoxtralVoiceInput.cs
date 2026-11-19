using System.Net.Http.Headers;
using System.Text.Json;
using VoiceControl.Audio;
using VoiceControl.Settings;

namespace VoiceControl;

/// <summary>
/// Saisie vocale via l'API Mistral (Voxtral) : enregistre l'énoncé dans un WAV
/// temporaire, l'uploade en multipart sur /v1/audio/transcriptions, retourne le champ "text".
/// Approche batch : simple, adaptée aux énoncés courts complets.
/// </summary>
public class VoxtralVoiceInput : IVoiceInput
{
    private readonly IVoiceSettingsProvider _settingsProvider;
    private readonly IAudioSource _audioSource;
    private readonly HttpClient _httpClient;

    public VoxtralVoiceInput(IVoiceSettingsProvider settingsProvider, IAudioSource audioSource, HttpClient? httpClient = null)
    {
        _settingsProvider = settingsProvider;
        _audioSource = audioSource;
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<string> CaptureTextAsync(CancellationToken cancellationToken = default)
    {
        var provider = (ConfigurationVoiceSettingsProvider)_settingsProvider;
        string apiKey = provider.RequireMistralApiKey();
        VoiceSettings settings = _settingsProvider.Settings;

        string wavPath = Path.Combine(Path.GetTempPath(), $"voice-{Guid.NewGuid():N}.wav");
        try
        {
            // En mode batch, l'annulation du token signifie "fin de saisie" :
            // on arrête l'enregistrement proprement puis on upload quand même.
            await WavFileWriter.WriteAsync(
                wavPath,
                ReadUntilStopRequested(_audioSource.ReadChunksAsync(), cancellationToken),
                settings.AudioCapture.SampleRate,
                settings.AudioCapture.Channels,
                CancellationToken.None);

            using var request = new HttpRequestMessage(HttpMethod.Post, settings.Mistral.TranscriptionsUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(settings.Mistral.Model), "model");
            await using FileStream fileStream = new(wavPath, FileMode.Open, FileAccess.Read);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
            form.Add(fileContent, "file", Path.GetFileName(wavPath));
            request.Content = form;

            using HttpResponseMessage response = await _httpClient.SendAsync(request, CancellationToken.None);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Transcription Voxtral a échoué ({(int)response.StatusCode}) : {body}");
            }

            using JsonDocument doc = JsonDocument.Parse(body);
            return doc.RootElement.TryGetProperty("text", out JsonElement text)
                ? text.GetString() ?? string.Empty
                : string.Empty;
        }
        finally
        {
            if (File.Exists(wavPath))
            {
                File.Delete(wavPath);
            }
        }
    }

    /// <summary>
    /// Lit la source audio jusqu'à ce que <paramref name="stopToken"/> soit annulé,
    /// sans propager l'annulation : la fin de lecture est alors propre.
    /// </summary>
    private static async IAsyncEnumerable<byte[]> ReadUntilStopRequested(
        IAsyncEnumerable<byte[]> source,
        CancellationToken stopToken,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var enumerator = source.WithCancellation(stopToken).GetAsyncEnumerator();
        while (true)
        {
            bool moved;
            try
            {
                moved = await enumerator.MoveNextAsync();
            }
            catch (OperationCanceledException) when (stopToken.IsCancellationRequested)
            {
                break;
            }
            if (!moved)
            {
                break;
            }
            yield return enumerator.Current;
        }
    }
}
