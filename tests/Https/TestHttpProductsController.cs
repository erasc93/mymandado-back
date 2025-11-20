//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

using api_mandado.Controllers;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using tests_mandado.utilities;

namespace tests_mandado.Https;

public class TestHttpProductsController : IClassFixture<MymandadoWebAppFactory>
{
    private HttpClient _client { get; init; }

    private T? Svc<T>(IServiceScope scope) where T : notnull => scope.ServiceProvider.GetRequiredService<T>();


    private const string URL = "/api/products";

    private readonly LoginInfo _loginInfo = new() { username = "manu", password = null };
    public TestHttpProductsController(MymandadoWebAppFactory webAppFactory)
    {
        IServiceScope s;
        AuthResponse? _authResponse;
        using (s = webAppFactory.Services.CreateScope())
        {
            _authResponse = Svc<UsersController>(s)?.Login(_loginInfo).Value as AuthResponse;
        }

        _client = webAppFactory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResponse?.token);
    }

    [Fact]
    public async Task Products_Should_Return_Success()
    {
        // DEFINE
        HttpResponseMessage response;

        // SET

        // ACT
        response = await _client.GetAsync(URL);

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task Products_Respons_ParsedTo_Product()
    {
        // Act
        Product[]? products;
        products = await _client.GetFromJsonAsync<Product[]>(URL);

        //ASSERT
        Assert.NotNull(products);
        Assert.Empty(products);
    }
}
