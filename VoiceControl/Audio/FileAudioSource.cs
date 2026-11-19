using System.Runtime.CompilerServices;
using VoiceControl.Settings;

namespace VoiceControl.Audio;

/// <summary>
/// Source audio depuis un fichier PCM 16 bits mono (ex: dump de debug), découpé en
/// frames de <see cref="AudioCaptureSettings.FrameDurationMs"/>. Utile pour rejouer
/// un enregistrement hors ligne.
/// </summary>
public class FileAudioSource : IAudioSource
{
    private readonly string _path;
    private readonly VoiceSettings _settings;

    public FileAudioSource(string path, IVoiceSettingsProvider settingsProvider)
    {
        _path = path;
        _settings = settingsProvider.Settings;
    }

    public async IAsyncEnumerable<byte[]> ReadChunksAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int frameBytes = _settings.AudioCapture.SampleRate * _settings.AudioCapture.Channels * 2
                         * _settings.AudioCapture.FrameDurationMs / 1000;

        await using FileStream fs = new(_path, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[frameBytes];
        int read;
        while ((read = await fs.ReadAsync(buffer, cancellationToken)) > 0)
        {
            byte[] frame = new byte[read];
            Array.Copy(buffer, frame, read);
            yield return frame;
        }
    }
}
