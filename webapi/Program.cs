using api_mandado.services;
using Microsoft.AspNetCore.Cors.Infrastructure;

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
        app.MapControllers();

        app.Run();
    }
    private static WebApplication BuildWebApp(string[] args)
    {
        WebApplicationBuilder builder;

        builder = WebApplication.CreateBuilder(args);



        DI_Services.instance.AddDependencies(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi()
                        .AddCors((CorsOptions options) => options.AddPolicy(CORS_NAME, ConfigurePolicyCORS));


        return builder.Build();
    }

    private static void ConfigurePolicyCORS(CorsPolicyBuilder builder)
    {
        builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
    }
}
