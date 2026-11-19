using Microsoft.Extensions.Configuration;

namespace VoiceControl.Settings;

/// <summary>
/// Lit <see cref="VoiceSettings"/> depuis un <see cref="IConfiguration"/>
/// (section "Voice", ex: appsettings.json). Lève une erreur explicite si une clé API manque.
/// </summary>
public class ConfigurationVoiceSettingsProvider : IVoiceSettingsProvider
{
    public VoiceSettings Settings { get; }

    public ConfigurationVoiceSettingsProvider(IConfiguration configuration)
    {
        Settings = configuration.GetSection(VoiceSettings.SectionName).Get<VoiceSettings>()
                   ?? new VoiceSettings();
    }

    public ConfigurationVoiceSettingsProvider(VoiceSettings settings)
    {
        Settings = settings;
    }

    /// <summary>Retourne la clé API Mistral ou lève si absente.</summary>
    public string RequireMistralApiKey()
    {
        return string.IsNullOrWhiteSpace(Settings.Mistral.ApiKey)
            ? throw new InvalidOperationException(
                $"Clé API Mistral manquante : renseignez '{VoiceSettings.SectionName}:Mistral:ApiKey' dans appsettings.json.")
            : Settings.Mistral.ApiKey;
    }

    /// <summary>Retourne la clé API Gradium ou lève si absente.</summary>
    public string RequireGradiumApiKey()
    {
        return string.IsNullOrWhiteSpace(Settings.Gradium.ApiKey)
            ? throw new InvalidOperationException(
                $"Clé API Gradium manquante : renseignez '{VoiceSettings.SectionName}:Gradium:ApiKey' dans appsettings.json.")
            : Settings.Gradium.ApiKey;
    }
}
