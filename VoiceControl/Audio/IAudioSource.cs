namespace VoiceControl.Audio;

/// <summary>
/// Source audio brute : flux de chunks PCM 16 bits signé, mono, au sample rate
/// configuré (<see cref="Settings.VoiceSettings.AudioCapture"/>).
/// </summary>
public interface IAudioSource
{
    IAsyncEnumerable<byte[]> ReadChunksAsync(CancellationToken cancellationToken = default);
}
