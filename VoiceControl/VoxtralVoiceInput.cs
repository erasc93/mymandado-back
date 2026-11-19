using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;

using VoiceControl.Audio;
using VoiceControl.Settings;

namespace VoiceControl;

/// <summary>
/// Saisie vocale via l'API Mistral (Voxtral) : enregistre l'énoncé dans un WAV
/// temporaire, l'uploade en multipart sur /v1/audio/transcriptions, retourne le champ "text".
/// Approche batch : simple, adaptée aux énoncés courts complets.
/// </summary>
public class VoxtralVoiceInput(
        IVoiceSettingsProvider settingsProvider,
        IAudioSource _audioSource,
        HttpClient? httpClient = null
    ) : IVoiceInput
{
    private readonly HttpClient _httpClient = httpClient ?? new();

    public async Task<string> CaptureTextAsync(CancellationToken cancellationToken = default)
    {
        string wavPath;
        Task<string> output;

        wavPath = Path.Combine(Path.GetTempPath(), $"voice-{Guid.NewGuid():N}.wav");
        try
        {
            await WavFileWriter.WriteAsync(
                                            wavPath,
                                            ReadUntilStopRequested(_audioSource.ReadChunksAsync(), cancellationToken),
                                            settingsProvider.Settings.AudioCapture.SampleRate,
                                            settingsProvider.Settings.AudioCapture.Channels,
                                            CancellationToken.None
                                           );
            output = TranscribeWavAsync(wavPath, cancellationToken);
            return await output;
        }
        finally
        {
            if (!File.Exists(wavPath))
            {
                File.Delete(wavPath);
            }
        }
    }


    /// <summary>Uploade le WAV sur l'endpoint de transcription et retourne le champ "text".</summary>
    private async Task<string> TranscribeWavAsync(string wavPath, CancellationToken cancellationToken)
    {
        MistralSettings mistralSettings = settingsProvider.Settings.Mistral;
        string apiMistralKey = settingsProvider.RequireMistralApiKey();

        using HttpRequestMessage request = BuildTranscriptionRequest(wavPath, apiMistralKey, mistralSettings);

        // Pas de token ici : même après un "stop", on va au bout de l'upload.
        using HttpResponseMessage response = await _httpClient.SendAsync(request, CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Transcription Voxtral a échoué ({(int)response.StatusCode}) : {responseBody}");
        }

        return ExtractTranscriptText(responseBody);
    }

    private static HttpRequestMessage BuildTranscriptionRequest(string wavPath, string apiKey, MistralSettings mistral)
    {
        HttpRequestMessage request = new(HttpMethod.Post, mistral.TranscriptionsUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        StringContent stringContent = new(mistral.Model);

        StreamContent fileContent = new(new FileStream(wavPath, FileMode.Open, FileAccess.Read));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

        request.Content = new MultipartFormDataContent()
                            {
                                { stringContent, "model"},
                                { fileContent, "file", Path.GetFileName(wavPath) }
                            };

        return request;
    }

    private static string ExtractTranscriptText(string responseBody)
    {
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.TryGetProperty("text", out JsonElement text)
            ? text.GetString() ?? string.Empty
            : string.Empty;
    }

    /// <summary>
    /// Lit la source audio jusqu'à ce que <paramref name="stopToken"/> soit annulé,
    /// sans propager l'annulation : la fin de lecture est alors propre.
    /// </summary>
    private static async IAsyncEnumerable<byte[]> ReadUntilStopRequested(
        IAsyncEnumerable<byte[]> source,
        CancellationToken stopToken,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using ConfiguredCancelableAsyncEnumerable<byte[]>.Enumerator enumerator = source.WithCancellation(stopToken).GetAsyncEnumerator();

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
