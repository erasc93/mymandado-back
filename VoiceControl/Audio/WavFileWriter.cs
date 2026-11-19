using System.Buffers.Binary;

namespace VoiceControl.Audio;

/// <summary>Résultat d'écriture d'un WAV : nombre d'octets de données écrits et amplitude crête (16 bits).</summary>
public readonly record struct WavWriteResult(long DataSize, int Peak);

/// <summary>
/// Écrit un flux PCM 16 bits mono dans un fichier WAV (en-tête RIFF minimal),
/// utile pour l'API batch de Mistral qui attend un fichier audio.
/// </summary>
public static class WavFileWriter
{
    /// <summary>Écrit le flux PCM dans un WAV et retourne la taille des données + l'amplitude crête.</summary>
    public static async Task<WavWriteResult> WriteAsync(string path, IAsyncEnumerable<byte[]> pcmChunks, int sampleRate, int channels, CancellationToken cancellationToken = default)
    {
        await using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
        // En-tête placeholder (taille inconnue en streaming) ; on réécrit en fin d'écriture.
        WriteHeader(fs, sampleRate, channels, dataSize: 0);

        long dataSize = 0;
        int peak = 0;
        await foreach (byte[] chunk in pcmChunks.WithCancellation(cancellationToken))
        {
            await fs.WriteAsync(chunk, cancellationToken);
            dataSize += chunk.Length;
            // Amplitude crête sur les échantillons 16 bits little-endian (pour détecter un silence complet).
            for (int i = 0; i + 1 < chunk.Length; i += 2)
            {
                int sample = Math.Abs(BinaryPrimitives.ReadInt16LittleEndian(chunk.AsSpan(i, 2)));
                if (sample > peak)
                {
                    peak = sample;
                }
            }
        }

        fs.Seek(4, SeekOrigin.Begin);
        await fs.WriteAsync(BitConverter.GetBytes((int)(36 + dataSize)), cancellationToken);
        fs.Seek(40, SeekOrigin.Begin);
        await fs.WriteAsync(BitConverter.GetBytes((int)dataSize), cancellationToken);
        return new WavWriteResult(dataSize, peak);
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
