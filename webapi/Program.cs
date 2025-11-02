using models;

namespace api_mymandado;


public class Program
{
    public static void Main(string[] args)
    {

        WebApplicationBuilder 
            builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddScoped<WeatherForecast>();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();
        builder.Services.AddControllers();

        WebApplication 
            app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        //app.UseCors();//
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
