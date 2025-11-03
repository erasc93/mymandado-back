//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado;

using System.Net;
using System.Threading.Tasks;
using api_mandado.Controllers;
using api_mymandado;
using core_mandado.models;
using core_mandado.repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using repositories;
using tests_mandado.utilities;
using Xunit;
public class TestProducts : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IRepo_Products _productsRepo;
    private readonly ProductsController _controller;
    public TestProducts(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        IServiceScope scope;
        scope = factory.Services.CreateScope();
        using (scope)
        {
            _productsRepo = scope.ServiceProvider.GetRequiredService<IRepo_Products>();
            _controller = new ProductsController(_productsRepo);
        }

    }

    [Fact]
    public async Task Products_Should_Return_Success()
    {
        // Act
        HttpResponseMessage
            response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("id", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("name", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("unit", content, StringComparison.OrdinalIgnoreCase);



        var res = _controller.Get();
        var okresult = Assert.IsType<OkObjectResult>(res.Result);
        Product last =Assert.IsType<Product[]>(okresult.Value).Last();

        _controller.Put(last);
        //Product last = okresult.Value
    }
}
