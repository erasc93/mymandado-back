using api_mandado.DependencyInjection;
using models;

namespace api_mymandado;


public class Program
{
    private const string CORS_NAME= "AllowAll";
    public static void Main(string[] args)
    {

        WebApplicationBuilder builder;
        WebApplication app;


        builder = WebApplication.CreateBuilder(args);



        DI_Services.instance.AddDependencies(builder.Services, builder.Configuration);

        builder.Services.AddCors((options) =>
        {
            options.AddPolicy(CORS_NAME,
                (builder) =>
                {
                    builder.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                });
        });


        app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors(CORS_NAME);//
        app.UseAuthorization();
        //app.UseAuthentication();//
        app.MapControllers();


        //string[]
        //    summaries = [
        //                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //                ];
        //app.MapGet("/weatherforecast", () =>
        //{
        //    WeatherForecast[]
        //    forecast = Enumerable.Range(1, 5).Select(index =>
        //        new WeatherForecast
        //        (
        //            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //            Random.Shared.Next(-20, 55),
        //            summaries[Random.Shared.Next(summaries.Length)]
        //        ))
        //        .ToArray();
        //    return forecast;
        //})
        //.WithName("GetWeatherForecast");



        app.Run();
    }
}
