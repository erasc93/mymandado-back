namespace tests_mandado;

using System.Net;
using System.Threading.Tasks;
using api_mymandado;
using Microsoft.AspNetCore.Mvc.Testing;
using models;
using Xunit;

public class TestWeatherForecast : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TestWeatherForecast(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        // On peut configurer le HttpClient ici
        //_client = new HttpClient
        //{
        //    BaseAddress = new Uri("http://localhost:8080")
        //};
    }

    [Fact]
    public async Task WeatherForecast_Should_Return_Success()
    {
        // Act
        HttpResponseMessage 
            response = await _client.GetAsync("api/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Optionnel : vérifier que le contenu est du JSON et non vide
        string 
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("temperature", content, StringComparison.OrdinalIgnoreCase);

        Assert.IsType<WeatherForecast[]>(response.Content);
    }
}
