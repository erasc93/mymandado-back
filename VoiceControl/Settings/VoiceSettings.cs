namespace VoiceControl.Settings;

/// <summary>
/// Paramètres de la saisie vocale, bindés depuis la section "Voice" de la configuration
/// (appsettings.json, variables d'environnement, etc.). Les secrets (clés API) ne sont
/// jamais codés en dur : ils sont renseignés par l'utilisateur dans appsettings.json.
/// </summary>
public class VoiceSettings
{
    public const string SectionName = "Voice";

    public MistralSettings Mistral { get; set; } = new();

    public GradiumSettings Gradium { get; set; } = new();

    public AudioCaptureSettings AudioCapture { get; set; } = new();
}

public class MistralSettings
{
    /// <summary>Clé API Mistral (Authorization: Bearer). Ex: "Voice:Mistral:ApiKey".</summary>
    public string? ApiKey { get; set; }

    /// <summary>Modèle de transcription Voxtral. Par défaut voxtral-mini-latest.</summary>
    public string Model { get; set; } = "voxtral-mini-latest";

    public string TranscriptionsUrl { get; set; } = "https://api.mistral.ai/v1/audio/transcriptions";
}

public class GradiumSettings
{
    /// <summary>Clé API Gradium (en-tête x-api-key sur le WebSocket). Ex: "Voice:Gradium:ApiKey".</summary>
    public string? ApiKey { get; set; }

    public string AsrWebSocketUrl { get; set; } = "wss://api.gradium.ai/api/speech/asr";

    /// <summary>Modèle de transcription. "default" ou "fast" selon le guide Gradium.</summary>
    public string ModelName { get; set; } = "default";

    /// <summary>Langue de l'audio (json_config.language). "any" si inconnue.</summary>
    public string Language { get; set; } = "fr";

    /// <summary>
    /// Contexte VAD en frames de 80 ms avant de conclure à une fin de parole.
    /// Plus la valeur est haute, plus le serveur attend avant de couper (évite les fins prématurées).
    /// </summary>
    public int DelayInFrames { get; set; } = 30;

    /// <summary>
    /// Délai d'inactivité (ms) après un end_text : si aucun nouveau segment n'arrive
    /// pendant ce délai, la capture s'arrête automatiquement. 0 = arrêt immédiat sur end_text.
    /// </summary>
    public int VadStopTimeoutMs { get; set; } = 2000;

    /// <summary>Debug : si renseigné, chaque message JSON reçu du WebSocket y est loggé.</summary>
    public string? DebugLogPath { get; set; }
}

public class AudioCaptureSettings
{
    /// <summary>Fréquence d'échantillonnage du micro. 24000 recommandé (natif Gradium).</summary>
    public int SampleRate { get; set; } = 24000;

    public int Channels { get; set; } = 1;

    /// <summary>Durée d'une frame audio envoyée. 80 ms = frame native Gradium.</summary>
    public int FrameDurationMs { get; set; } = 80;

    /// <summary>
    /// Si true, l'envoi des frames est cadencé au temps réel (une frame toutes les
    /// FrameDurationMs). Mettre false pour rejouer un fichier le plus vite possible.
    /// </summary>
    public bool RealtimePacing { get; set; } = true;

    /// <summary>
    /// Commande externe produisant du PCM 16 bits signé sur stdout (ex: ffmpeg).
    /// Laisser vide désactive la capture micro temps réel.
    /// </summary>
    public string CaptureCommand { get; set; } =
        "ffmpeg -loglevel error -f avfoundation -i :0 -f s16le -acodec pcm_s16le -ar 24000 -ac 1 -";

    /// <summary>
    /// Debug : si renseigné, tout le PCM reçu du micro est aussi écrit dans ce fichier
    /// (ex: "C:\\temp\\capture.pcm") pour vérifier que le matériel envoie bien du son.
    /// </summary>
    public string? DebugDumpPath { get; set; }
}
