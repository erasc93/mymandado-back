
using api_mandado.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Configuration;
//using SkillCraft.Category;
//using SkillCraft.LearnDIYs;
//using SkillCraft.MatchingTeacherStudent;
//using SkillCraft_FakeData;

public interface IAppHost
{
    T CreateInstance<T>() where T : notnull;
}
public abstract class AHostBuilder : IAppHost
{
    public ServiceCollection _services { get; init; }
    public IConfiguration? _config { get; init; }

    protected readonly ServiceProvider _serviceProvider;
    public AHostBuilder()
    {
        _services = new ();
        ConfigurationBuilder builder = new();

        builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        AddAzureConfiguration(builder);
        
        _config = builder.Build();

        AddCoreDependencies(_services, _config);
        AddPlatformSpecificDependencies();
        _serviceProvider = _services.BuildServiceProvider();
    }
    private void AddAzureConfiguration(IConfigurationBuilder builder)
    {
        Uri vaultUri = new(@"https://kv-cleeven-prj-azure.vault.azure.net/");
        DefaultAzureCredential cred = new();
        builder.AddAzureKeyVault(vaultUri, cred);
    }
    private void AddCoreDependencies(IServiceCollection services, IConfiguration config)
    {
        DI_Services.instance.AddDependencies(services, config);
    }
    protected abstract void AddPlatformSpecificDependencies();

    public T CreateInstance<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
