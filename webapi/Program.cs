using api_mandado.services;
using api_mandado.Hubs;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Azure.Identity;

namespace api_mandado;


public class Program
{
    private const string CORS_NAME = "AllowAll";
    public static void Main(string[] args)
    {

        WebApplication app;

        app = BuildWebApp(args);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors(CORS_NAME);//
        app.UseAuthentication();//
        app.UseAuthorization();
        //app.Use(async (context, next) =>
        //{
        //    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        //    await next();
        //});
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
        app.MapControllers();
        app.MapHub<CartHub>("/hubs/cart");

        app.Run();
    }
    private static WebApplication BuildWebApp(string[] args)
    {
        WebApplicationBuilder builder;

        builder = WebApplication.CreateBuilder(args);

        //AddAzureVaultConfiguration(builder.Configuration);
        DI_Services.instance.AddDependencies(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddOpenApi()
                        .AddCors((CorsOptions options) => options.AddPolicy(CORS_NAME, ConfigurePolicyCORS));


        return builder.Build();
    }
    private static void AddAzureVaultConfiguration(IConfigurationBuilder builder)
    {
        Uri vaultUri = new(@"https://kv-cleeven-prj-azure.vault.azure.net/");
        DefaultAzureCredential cred = new();
        builder.AddAzureKeyVault(vaultUri, cred);
    }

    private static void ConfigurePolicyCORS(CorsPolicyBuilder builder)
    {
        builder
            //.WithOrigins("http://localhost:4200")
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    }
}
