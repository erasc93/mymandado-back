namespace VoiceControl.Audio;

/// <summary>
/// Écrit un flux PCM 16 bits mono dans un fichier WAV (en-tête RIFF minimal),
/// utile pour l'API batch de Mistral qui attend un fichier audio.
/// </summary>
public static class WavFileWriter
{
    public static async Task WriteAsync(string path, IAsyncEnumerable<byte[]> pcmChunks, int sampleRate, int channels, CancellationToken cancellationToken = default)
    {
        await using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
        // En-tête placeholder (taille inconnue en streaming) ; on réécrit en fin d'écriture.
        WriteHeader(fs, sampleRate, channels, dataSize: 0);

        long dataSize = 0;
        await foreach (byte[] chunk in pcmChunks.WithCancellation(cancellationToken))
        {
            await fs.WriteAsync(chunk, cancellationToken);
            dataSize += chunk.Length;
        }

        fs.Seek(4, SeekOrigin.Begin);
        await fs.WriteAsync(BitConverter.GetBytes((int)(36 + dataSize)), cancellationToken);
        fs.Seek(40, SeekOrigin.Begin);
        await fs.WriteAsync(BitConverter.GetBytes((int)dataSize), cancellationToken);
    }

    private static void WriteHeader(Stream s, int sampleRate, int channels, int dataSize)
    {
        const short bitsPerSample = 16;
        short blockAlign = (short)(channels * bitsPerSample / 8);
        int byteRate = sampleRate * blockAlign;

        using BinaryWriter w = new(s, System.Text.Encoding.ASCII, leaveOpen: true);
        w.Write("RIFF".ToCharArray());
        w.Write(36 + dataSize);
        w.Write("WAVE".ToCharArray());
        w.Write("fmt ".ToCharArray());
        w.Write(16); // taille du bloc fmt
        w.Write((short)1); // PCM
        w.Write((short)channels);
        w.Write(sampleRate);
        w.Write(byteRate);
        w.Write(blockAlign);
        w.Write(bitsPerSample);
        w.Write("data".ToCharArray());
        w.Write(dataSize);
    }
}
