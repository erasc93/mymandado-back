namespace VoiceControl.Settings;

/// <summary>
/// Fournit les paramètres de la saisie vocale. Permet de découpler les consommateurs
/// de la source concrète de configuration (DIP).
/// </summary>
public interface IVoiceSettingsProvider
{
    VoiceSettings Settings { get; }

    /// <summary>Retourne la clé API Mistral ou lève une erreur explicite si absente.</summary>
    string RequireMistralApiKey();

    /// <summary>Retourne la clé API Gradium ou lève une erreur explicite si absente.</summary>
    string RequireGradiumApiKey();
}
