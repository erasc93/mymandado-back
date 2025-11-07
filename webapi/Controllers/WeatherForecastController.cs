using core_mandado.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_mandado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {


        [HttpGet]
        public WeatherForecast[] Get()
        {
            WeatherForecast[] forecast;
            string[] summaries;

            summaries = [
                            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                        ];

            forecast = Enumerable.Range(1, 5)
                                .Select(index => new WeatherForecast
                                        (
                                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                            Random.Shared.Next(-20, 55),
                                            summaries[Random.Shared.Next(summaries.Length)]
                                        )).ToArray();
            return forecast;
        }
    }
}
