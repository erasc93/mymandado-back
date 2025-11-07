//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

using System.Net;
using System.Net.Http.Headers;
using api_mandado.Controllers;
using api_mandado.services;
using core_mandado.Products;
using core_mandado.Users;
using core_mandado.Users.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using tests_mandado.utilities;

namespace tests_mandado;
public class TestProducts : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IRepo_Products _productsRepo;
    private readonly ProductsController _controller;
    private readonly UserContextService _svc_user;
    private readonly UsersController _authController;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRepo_Users _repoUser;

    private readonly IServiceScope _scope;
    public TestProducts(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        using (_scope)
        {
            _productsRepo = Svc<IRepo_Products>();
            _svc_user = Svc<UserContextService>();

            _repoUser = Svc<IRepo_Users>();
            _jwtTokenGenerator = Svc<IJwtTokenGenerator>();



            _authController = new UsersController(_jwtTokenGenerator, _repoUser);
            _controller = new ProductsController(_productsRepo, _svc_user);
        }
    }
    private T Svc<T>()
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    [Fact]
    public async Task Products_Should_Return_Success()
    {
        // Act
        HttpResponseMessage response;
        LoginInfo loginInfo = new LoginInfo() { username = "manu", password = null };

        ObjectResult actionResult = _authController.Login(loginInfo);
        TokenType result = actionResult.Value as TokenType;

        //string token = okResult.Value as string;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.token);
        response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert
        string
            content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
        Assert.Contains("id", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("name", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("unit", content, StringComparison.OrdinalIgnoreCase);



        var res = _controller.Get();
        var okresult = Assert.IsType<OkObjectResult>(res.Result);
        Product last = Assert.IsType<Product[]>(okresult.Value).Last();

        _controller.Put(last);
        //Product last = okresult.Value
    }
}
