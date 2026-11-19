namespace VoiceControl;

/// <summary>
/// Contrat de saisie vocale : écoute le micro et retourne le texte transcrit.
/// Implémentations : <see cref="VoxtralVoiceInput"/> (Mistral, batch) et
/// <see cref="GradiumVoiceInput"/> (Gradium, streaming WebSocket).
/// </summary>
public interface IVoiceInput
{
    /// <summary>
    /// Capture un énoncé vocal et renvoie sa transcription.
    /// </summary>
    /// <param name="cancellationToken">Annulation (ex: touche Échap dans le menu console).</param>
    Task<string> CaptureTextAsync(CancellationToken cancellationToken = default);
}
