using System.Diagnostics;
using System.Runtime.CompilerServices;
using VoiceControl.Settings;

namespace VoiceControl.Audio;

/// <summary>
/// Capture le micro via une commande externe (ffmpeg par défaut) qui écrit du PCM
/// sur sa sortie standard. La commande est configurable dans
/// "Voice:AudioCapture:CaptureCommand".
/// </summary>
public class ProcessAudioSource(IVoiceSettingsProvider settingsProvider) : IAudioSource
{
    private readonly VoiceSettings _settings = settingsProvider.Settings;

    public async IAsyncEnumerable<byte[]> ReadChunksAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ProcessStartInfo startInfo = GetCaptureAudioProcess(out string fileName);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Impossible de démarrer la capture audio : {fileName}");

        // Vide stderr en arrière-plan pour éviter le blocage si ffmpeg écrit beaucoup.
        Task stderrDrain = DrainStderrAsync(process, cancellationToken);

        Stream stdout = process.StandardOutput.BaseStream;
        FileStream? debugDump = OpenDebugDump();

        // Découpe le flux en frames de taille fixe (ex: 80 ms) comme attendu par Gradium.
        int frameBytes = ComputeFrameBytes();

        byte[] buffer = new byte[4096];
        try
        {
            MemoryStream pending = new();
            int read;
            while ((read = await stdout.ReadAsync(buffer, cancellationToken)) > 0)
            {
                pending.Write(buffer, 0, read);
                if (debugDump is not null)
                {
                    await debugDump.WriteAsync(buffer.AsMemory(0, read), CancellationToken.None);
                }

                foreach (byte[] frame in ReadFrames(pending, frameBytes))
                {
                    yield return frame;
                }
            }
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }

            // Le Kill ci-dessus coupe les flux : le drain se termine aussitôt,
            // on attend qu'il soit terminé avant de disposer le process.
            await stderrDrain;

            if (debugDump is not null)
            {
                await debugDump.FlushAsync();
                await debugDump.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Consomme stderr en continu. En cas d'annulation ou de fermeture du process,
    /// le drainage est simplement interrompu : il n'est pas critique pour la capture.
    /// </summary>
    private static async Task DrainStderrAsync(Process process, CancellationToken cancellationToken)
    {
        try
        {
            using StreamReader stderr = process.StandardError;
            while (await stderr.ReadLineAsync(cancellationToken) is not null) { }
        }
        catch (OperationCanceledException)
        {
            // Annulation attendue (arrêt de la capture).
        }
        catch (Exception)
        {
            // Non critique : ex ObjectDisposedException si le process est déjà fermé.
        }
    }

    /// <summary>
    /// Extrait les frames complètes du buffer d'accumulation et garde le reste
    /// (partie incomplète) pour le prochain appel.
    /// </summary>
    private static IEnumerable<byte[]> ReadFrames(MemoryStream pending, int frameBytes)
    {
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
        int restLength = (int)pending.Length - offset;
        byte[] rest = new byte[restLength];
        Array.Copy(pendingBytes, offset, rest, 0, restLength);
        pending.SetLength(0);
        pending.Write(rest, 0, restLength);
    }

    private FileStream? OpenDebugDump()
    {
        string? path = _settings.AudioCapture.DebugDumpPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        return new FileStream(path, FileMode.Create, FileAccess.Write);
    }

    private int ComputeFrameBytes()
    {
        return _settings.AudioCapture.SampleRate * _settings.AudioCapture.Channels * 2
               * _settings.AudioCapture.FrameDurationMs / 1000;
    }

    private ProcessStartInfo GetCaptureAudioProcess(out string fileName)
    {
        string command = _settings.AudioCapture.CaptureCommand;
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new InvalidOperationException(
                "Aucune commande de capture configurée ('Voice:AudioCapture:CaptureCommand').");
        }

        // Découpe exécutable + arguments en respectant les guillemets
        // (ex: "C:\Program Files\ffmpeg\bin\ffmpeg.exe" -i audio="Casque (EarPods)" ...).
        SplitCommand(command, out fileName, out string arguments);

        ProcessStartInfo startInfo = new(fileName, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        return startInfo;
    }

    /// <summary>
    /// Sépare l'exécutable de ses arguments : entre guillemets si le chemin en contient,
    /// sinon jusqu'au premier espace.
    /// </summary>
    private static void SplitCommand(string command, out string fileName, out string arguments)
    {
        if (command.StartsWith('"'))
        {
            int endQuote = command.IndexOf('"', 1);
            fileName = command[1..endQuote];
            arguments = command[(endQuote + 1)..].TrimStart();
            return;
        }

        int firstSpace = command.IndexOf(' ');
        fileName = firstSpace < 0 ? command : command[..firstSpace];
        arguments = firstSpace < 0 ? string.Empty : command[(firstSpace + 1)..];
    }
}
