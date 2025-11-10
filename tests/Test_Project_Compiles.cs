namespace tests_mandado;

using System.Net;
using System.Threading.Tasks;
using api_mymandado;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class Test_Project_Compiles : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public Test_Project_Compiles(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test_Api_WeatherForecast_Success()
    {
        // Act
        HttpResponseMessage
            response = await _client.GetAsync("api/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("temperature", content, StringComparison.OrdinalIgnoreCase);

        //Assert.IsType<WeatherForecast[]>(response.Content);
        //var controller = new ProductsController(new ProductsRepository());
        //var res = controller.Get();
        //var okresult = Assert.IsType<OkObjectResult>(response.c);
        //Assert.IsType<WeatherForecast[]>(okresult.Value);
    }
    [Fact]
    public async Task Test_Api_WeatherForecast_TypeConversion_Works()
    {
        // Act
        HttpResponseMessage
            response = await _client.GetAsync("api/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //response.
        string
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("temperature", content, StringComparison.OrdinalIgnoreCase);

        //Assert.IsType<WeatherForecast[]>(response.Content);
        //var controller = new ProductsController(new ProductsRepository());
        //var res = controller.Get();
        //var okresult = Assert.IsType<OkObjectResult>(response.c);
        //Assert.IsType<WeatherForecast[]>(okresult.Value);
    }
}
