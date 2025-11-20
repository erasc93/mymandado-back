//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.utilities;

using api_mandado;
using api_mandado.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;
public abstract class ACustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(ConfigureContext);
        //builder.ConfigureServices(DI_Services.instance.AddDependencies);
        builder.ConfigureServices(SetupDependencyInjectionServices);
    }

    private void SetupDependencyInjectionServices(WebHostBuilderContext context, IServiceCollection services)
    {
        //context.Configuration
        DI_Services.instance.AddDependencies(services, context.Configuration);
        AddControllers(services);
        //DI_Services.instance.AddHostContext
    }
    protected abstract void AddControllers(IServiceCollection services);



    private void ConfigureContext(WebHostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddEnvironmentVariables()
               .AddUserSecrets<Program>(optional: true)
               //.AddJsonFile("appsettings.json", optional: true)
               //.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
               ;
    }
}
