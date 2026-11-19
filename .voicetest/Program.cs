using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using VoiceControl;
using VoiceControl.Audio;
using VoiceControl.Settings;

class FakeMicSource : IAudioSource
{
    public async IAsyncEnumerable<byte[]> ReadChunksAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        byte[] frame = new byte[24000 * 2 * 80 / 1000];
        while (true)
        {
            await Task.Delay(50, cancellationToken);
            yield return frame;
        }
    }
}

class EofSource : IAudioSource
{
    public async IAsyncEnumerable<byte[]> ReadChunksAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}

class FakeProvider : IVoiceSettingsProvider
{
    public VoiceSettings Settings { get; } = new();
    public string RequireMistralApiKey() => "fake-key";
    public string RequireGradiumApiKey() => "fake-key";
}

class FakeHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Console.WriteLine($"  [HTTP] upload recu, content-length={request.Content?.Headers.ContentLength}");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"text\":\"bonjour le monde\"}")
        });
    }
}

class P
{
    static async Task Main()
    {
        Console.WriteLine("--- Scenario 1 : micro qui stream, cancel (touche o) apres 2s ---");
        var input = new VoxtralVoiceInput(new FakeProvider(), new FakeMicSource(), new HttpClient(new FakeHandler()));
        using (var cts = new CancellationTokenSource())
        {
            _ = Task.Run(async () => { await Task.Delay(2000); cts.Cancel(); });
            var sw = Stopwatch.StartNew();
            try
            {
                string text = await input.CaptureTextAsync(cts.Token);
                sw.Stop();
                Console.WriteLine($"  => elapsed={sw.ElapsedMilliseconds}ms, text=\"{text}\"");
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($"  => elapsed={sw.ElapsedMilliseconds}ms, EXCEPTION {ex.GetType().Name}: {ex.Message}");
            }
        }

        Console.WriteLine("--- Scenario 2 : source en EOF immediat (ffmpeg en echec), pas d'annulation ---");
        var input2 = new VoxtralVoiceInput(new FakeProvider(), new EofSource(), new HttpClient(new FakeHandler()));
        var sw2 = Stopwatch.StartNew();
        string text2 = await input2.CaptureTextAsync(CancellationToken.None);
        sw2.Stop();
        Console.WriteLine($"  => elapsed={sw2.ElapsedMilliseconds}ms, text=\"{text2}\"");
    }
}
