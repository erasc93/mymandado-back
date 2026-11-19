using System.Diagnostics;
using System.Runtime.CompilerServices;
using VoiceControl.Settings;

namespace VoiceControl.Audio;

/// <summary>
/// Capture le micro via une commande externe (ffmpeg par défaut) qui écrit du PCM
/// sur sa sortie standard. La commande est configurable dans
/// "Voice:AudioCapture:CaptureCommand".
/// </summary>
public class ProcessAudioSource : IAudioSource
{
    private readonly VoiceSettings _settings;

    public ProcessAudioSource(IVoiceSettingsProvider settingsProvider)
    {
        _settings = settingsProvider.Settings;
    }

    public async IAsyncEnumerable<byte[]> ReadChunksAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string command = _settings.AudioCapture.CaptureCommand;
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new InvalidOperationException(
                "Aucune commande de capture configurée ('Voice:AudioCapture:CaptureCommand').");
        }

        // Découpe exécutable + arguments en respectant les guillemets
        // (ex: "C:\Program Files\ffmpeg\bin\ffmpeg.exe" -i audio="Casque (EarPods)" ...).
        string fileName;
        string arguments;
        if (command.StartsWith('"'))
        {
            int endQuote = command.IndexOf('"', 1);
            fileName = command[1..endQuote];
            arguments = command[(endQuote + 1)..].TrimStart();
        }
        else
        {
            int firstSpace = command.IndexOf(' ');
            fileName = firstSpace < 0 ? command : command[..firstSpace];
            arguments = firstSpace < 0 ? string.Empty : command[(firstSpace + 1)..];
        }

        var startInfo = new ProcessStartInfo(fileName, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Impossible de démarrer la capture audio : {fileName}");

        byte[] buffer = new byte[4096];
        Stream stdout = process.StandardOutput.BaseStream;

        // Vide stderr en arrière-plan pour éviter le blocage si ffmpeg écrit beaucoup.
        _ = Task.Run(async () =>
        {
            using var stderr = process.StandardError;
            while (await stderr.ReadLineAsync(cancellationToken) is not null) { }
        });

        FileStream? debugDump = null;
        if (!string.IsNullOrWhiteSpace(_settings.AudioCapture.DebugDumpPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settings.AudioCapture.DebugDumpPath)!);
            debugDump = new FileStream(_settings.AudioCapture.DebugDumpPath, FileMode.Create, FileAccess.Write);
        }

        // Découpe le flux en frames de taille fixe (ex: 80 ms) comme attendu par Gradium.
        int frameBytes = _settings.AudioCapture.SampleRate * _settings.AudioCapture.Channels * 2
                         * _settings.AudioCapture.FrameDurationMs / 1000;

        try
        {
            var pending = new MemoryStream();
            int read;
            while ((read = await stdout.ReadAsync(buffer, cancellationToken)) > 0)
            {
                pending.Write(buffer, 0, read);
                if (debugDump is not null)
                {
                    await debugDump.WriteAsync(buffer.AsMemory(0, read), CancellationToken.None);
                }

                byte[] pendingBytes = pending.GetBuffer();
                int offset = 0;
                while (pending.Length - offset >= frameBytes)
                {
                    byte[] frame = new byte[frameBytes];
                    Array.Copy(pendingBytes, offset, frame, 0, frameBytes);
                    offset += frameBytes;
                    yield return frame;
                }
                // Garde le reste pour la prochaine frame.
                byte[] rest = new byte[pending.Length - offset];
                Array.Copy(pendingBytes, offset, rest, 0, rest.Length);
                pending.SetLength(0);
                pending.Write(rest, 0, rest.Length);
            }
        }
        finally
        {
            if (debugDump is not null)
            {
                await debugDump.FlushAsync();
                await debugDump.DisposeAsync();
            }
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
    }
}
