//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.utilities;

using api_mandado.DependencyInjection;
using api_mymandado;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(ConfigureContext);
        //builder.ConfigureServices(DI_Services.instance.AddDependencies);
        builder.ConfigureServices(f);
    }

    private void f(WebHostBuilderContext context, IServiceCollection collection)
    {
        //context.Configuration
        DI_Services.instance.AddDependencies(collection, context.Configuration);
        //DI_Services.instance.AddHostContext
    }

    private void ConfigureContext(WebHostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddEnvironmentVariables()
               .AddUserSecrets<Program>(optional: true)
               //.AddJsonFile("appsettings.json", optional: true)
               //.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
               ;
    }
}
