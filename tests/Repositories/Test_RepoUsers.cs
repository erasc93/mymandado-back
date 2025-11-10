using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.tables;
using Services.Repositories;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoUsers : IClassFixture<MymandadoWebAppFactory>
{


    private readonly Repo_StoredProcedures _repoStoredPro;
    private readonly Repo_TableInfos _repoTables;


    private readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;

    private readonly IRepo_Users _repoUsers;
    private readonly IRepo_CartItems _repoCartItems;
    private readonly IRepo_Cart _repoCart;
    private readonly IRepo_Products _repoProducts;
    public Test_RepoUsers(MymandadoWebAppFactory factory)
    {
        IServiceScope scope;
        scope = factory.Services.CreateScope();
        using (scope)
        {
            _repoCart = scope.ServiceProvider.GetRequiredService<IRepo_Cart>();
            _repoUsers = scope.ServiceProvider.GetRequiredService<IRepo_Users>();
            _repoProducts = scope.ServiceProvider.GetRequiredService<IRepo_Products>();
            _repoDBProducts = scope.ServiceProvider.GetRequiredService<Repo_AnyTable<MND_PRODUCT>>();
            _repoCartItems = scope.ServiceProvider.GetRequiredService<IRepo_CartItems>();
        }
    }

    [Fact]
    public void UserCRUD()
    {
        User[] allUsers = _repoUsers.GetAll();
        Assert.NotEmpty(allUsers);
        string userName = "usertest4";
        User? u;
        u = _repoUsers.GetUserByName(userName);
        Assert.Null(u);

        u = _repoUsers.AddByName(userName);
        u = _repoUsers.GetUserByName(userName);
        Assert.NotNull(u);
        Cart[] userCarts;
        Cart? firstCart;

        userCarts = _repoCart.GetAll(u);
        Assert.NotEmpty(userCarts);
        firstCart = userCarts[0];

        Assert.NotNull(firstCart);
        Assert.NotNull(firstCart.items);
        Assert.Empty(firstCart.items);

        Product newproduct;
        newproduct = new Product()
        {
            id = 0,
            name = "testproduct2",
            unit = "g"
        };

        CartItem item = new CartItem
        {
            id = -13,
            product = newproduct,
            quantity = 3,
            isdone = false
        };

        _repoCartItems.Add(firstCart.numero, ref item, u);

        userCarts = _repoCart.GetAll(u);
        Assert.NotEmpty(userCarts);
        firstCart = userCarts[0];

        Assert.NotNull(firstCart);
        Assert.NotNull(firstCart.items);
        Assert.NotEmpty(firstCart.items);

        _repoProducts.RemoveItem(newproduct.id);
        //_repoProducts.Add(ref newproduct);
        //_repoCart.AddToCart(newproduct);
        //_repoCartItems.Add(0, ref userCarts[0], u);

        _repoUsers.Delete(u);
        u = _repoUsers.GetUserByName(userName);
        Assert.Null(u);
    }
}
