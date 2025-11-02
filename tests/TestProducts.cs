//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado;

using System.Net;
using System.Threading.Tasks;
using api_mandado.Controllers;
using api_mymandado;
using core_mandado.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class TestProducts : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TestProducts(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        // On peut configurer le HttpClient ici
        //_client = new HttpClient
        //{
        //    BaseAddress = new Uri("http://localhost:8080")
        //};
    }

    [Fact]
    public async Task Products_Should_Return_Success()
    {
        // Act
        HttpResponseMessage 
            response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //Assert.IsType<Ok<Product[]>(response);
        // Optionnel : vérifier que le contenu est du JSON et non vide
        string 
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("name", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("quantity", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("status", content, StringComparison.OrdinalIgnoreCase);


        var controller = new ProductsController();
        var res = controller.Get();
        var okresult = Assert.IsType<OkObjectResult>(res.Result);
        Assert.IsType<Product[]>(okresult.Value);
    }
}
