namespace api_mandado.services;

public class DI_Console : AHostBuilder,IAppHost
{
    #region Singleton
    private static DI_Console? _instance { get; set; }
    public static DI_Console Instance { get { if (_instance is null) { _instance = new DI_Console(); } return _instance; } }

    #endregion
    protected override void AddPlatformSpecificDependencies()
    {
        //_services.AddSingleton<>();
    }
}
