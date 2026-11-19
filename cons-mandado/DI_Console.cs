using Microsoft.Extensions.DependencyInjection;
using VoiceControl;
using VoiceControl.Audio;
using VoiceControl.Settings;

namespace cons_mandado;

public class DI_Console : AHostBuilder,IAppHost
{
    #region Singleton
    private static DI_Console? _instance { get; set; }
    public static DI_Console Instance { get { if (_instance is null) { _instance = new DI_Console(); } return _instance; } }

    #endregion
    protected override void AddPlatformSpecificDependencies()
    {
        AddMenus();
        AddVoiceControls();
    }

    private void AddVoiceControls()
    {
        _services.AddSingleton<IVoiceSettingsProvider>(
            new ConfigurationVoiceSettingsProvider(_config!));
        _services.AddSingleton<IAudioSource, ProcessAudioSource>();
        //_services.AddSingleton<IVoiceInput, GradiumVoiceInput>();
        _services.AddSingleton<IVoiceInput, VoxtralVoiceInput>();
    }

    private void AddMenus()
    {
        _services.AddSingleton<MenuMain>();
        _services.AddSingleton<MenuHome>();
    }
}
