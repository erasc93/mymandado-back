using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using MySql.Data.MySqlClient;
using System.Data;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoCart(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{
    private const string USERNAME = "testuser13";
    private const int UNDEFINED = -13;

    [Fact]
    public void TEST_OnUserCreated_CartCreated_CartEmpty()
    {
        _fac.SecureTest(
        () =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));
            //_fac._repoUsers.Add(ref testUser);
            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });
            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            int
            nextid = carts.Length + 1;
            _fac._repoCart.AddEmptyCart(testUser, nextid, "cart", "");
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Equal(2, carts.Length);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);


        });
    }
    [Fact]
    public void TEST_DeletedCart()
    {
        _fac.SecureTest(() =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));
            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });
            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = _fac._repoCart.AddEmptyCart(testUser, 1, "cart", "");
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Equal(2, carts.Length);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);

            _fac._repoCart.Remove(newCart);
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
        });
    }
    [Fact]
    public void TEST_UpdateCart()
    {
        _fac.SecureTest(
        () =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));
            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });

            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = _fac._repoCart.AddEmptyCart(user: testUser, numero: 1, name: "cart", description: "");
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Equal(2, carts.Length);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);


            const string newdescription = "new description";
            const string newname = "new name";
            newCart.name = newname;
            newCart.description = newdescription;
            _fac._repoCart.Update( newCart);
            Cart?
            updated = _fac._repoCart.GetBy(testUser, newCart.id);
            Assert.Equal(newdescription, updated?.description);
            Assert.Equal(newname, updated?.name);

            _fac._repoCart.Remove(newCart);
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
        });
    }
    [Fact]
    public void TEST_AddItem()
    {
        _fac.SecureTest(
        () =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));

            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });
            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = carts[0];

            Product
            newproduct1 = new()
            {
                id = UNDEFINED,
                name = "testproduct1",
                unit = "",
            },
            newproduct2 = new()
            {
                id = UNDEFINED,
                name = "testproduct2",
                unit = "",
            };
            Product[]
            prds = _fac._repoProducts.GetAll();
            Assert.Empty(prds.Where(x => x.name == newproduct1.name).ToArray());
            _fac._repoProducts.Add(ref newproduct1);
            _fac._repoProducts.Add(ref newproduct2);
            prds = _fac._repoProducts.GetAll();
            Assert.NotEmpty(prds);

            Product
                p = prds[0];
            _fac._repoCart.AddAllProductsAsItems(testUser.id, newCart.numero);

            carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.NotEmpty(carts[0].items!);


            _fac._repoCart.Remove(newCart);
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Empty(carts);

            _fac._repoCartItems.GetAll(testUser);
        });
    }
    [Fact]
    public void TEST_AddItem_Single()
    {
        _fac.SecureTest(
        () =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));
            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });
            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = carts[0];

            Product
            newproduct1 = new()
            {
                id = UNDEFINED,
                name = "testproduct1",
                unit = "",
            },
            newproduct2 = new()
            {
                id = UNDEFINED,
                name = "testproduct2",
                unit = "",
            };
            Product[]
                prds = _fac._repoProducts.GetAll();
            Assert.Empty(prds.Where(x => x.name == newproduct1.name).ToArray());
            _fac._repoProducts.Add(ref newproduct1);
            _fac._repoProducts.Add(ref newproduct2);
            prds = _fac._repoProducts.GetAll();
            Assert.NotEmpty(prds);

            Product
                product = prds[0];

            CartItem
            item1 = _fac._repoCart.AddProduct(testUser, newCart.numero, product, qtt: 0, isdone: false);
            Assert.Throws<MySqlException>(() => _fac._repoCart.AddProduct(testUser, newCart.numero, product, qtt: 0, isdone: false));

            carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.NotEmpty(carts[0].items!);


            _fac._repoCart.Remove(newCart);
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Empty(carts);

            _fac._repoCartItems.GetAll(testUser);
        });
    }
    [Fact]
    public void TEST_AddItem_Update()
    {
        _fac.SecureTest(
        () =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser));
            testUser = _fac._repoUsers.AddByName(new() { username = testUser.name, password = "password" });
            Assert.True(UserExists_ByName(testUser));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = carts[0];

            Product
            newproduct1 = new()
            {
                id = UNDEFINED,
                name = "testproduct1",
                unit = "",
            },
            newproduct2 = new()
            {
                id = UNDEFINED,
                name = "testproduct2",
                unit = "",
            };
            Product[]
                prds = _fac._repoProducts.GetAll();
            Assert.Empty(prds.Where(x => x.name == newproduct1.name).ToArray());
            _fac._repoProducts.Add(ref newproduct1);
            //_fac._repoProducts.Add(ref newproduct2);
            prds = _fac._repoProducts.GetAll();
            Assert.NotEmpty(prds);

            Product
                product = newproduct2;

            CartItem item1 =
            new()
            {
                id = UNDEFINED,
                product = product,
                quantity = 3,
                isdone = false,
            };

            _fac._repoCartItems.AddItem(testUser, newCart.numero, ref item1);
            Assert.Throws<MySqlException>(() => _fac._repoCart.AddProduct(testUser, newCart.numero, product, qtt: 0, isdone: false));

            carts = _fac._repoCart.GetAll(testUser);
            Assert.Single(carts);
            Assert.NotNull(carts[0].items);
            Assert.NotEmpty(carts[0].items!);
            CartItem[]
            founditem = [.. carts[0].items!.Where(x =>
            x.product.name == product.name && x.product.id == product.id)];
            Assert.Equal(1, founditem?.Length);

            Assert.NotNull(founditem);
            Assert.NotNull(founditem[0]);
            const int QTTU = 17;
            founditem[0].quantity = QTTU;
            _fac._repoCartItems.Update(testUser, newCart.numero, founditem[0]);

            carts = _fac._repoCart.GetAll(testUser);
            founditem = [.. carts[0].items!.Where(x =>
            x.product.name == product.name && x.product.id == product.id && x.quantity == QTTU)];
            Assert.Equal(1, founditem?.Length);
            Assert.NotNull(founditem);
            Assert.NotNull(founditem[0]);

            _fac._repoCartItems.RemoveById(founditem[0].id);

            carts = _fac._repoCart.GetAll(testUser);
            founditem = [.. carts[0].items!.Where(x =>
            x.product.name == product.name && x.product.id == product.id && x.quantity == QTTU)];
            Assert.Equal(0, founditem?.Length);
            //Assert.Empty(founditem);
            //Assert.NotNull(founditem[0]);



            _fac._repoCart.Remove(newCart);
            carts = _fac._repoCart.GetAll(testUser);
            Assert.Empty(carts);

            _fac._repoCartItems.GetAll(testUser);
        });
    }
    private User BuildUser()
    {
        return new User() { id = UNDEFINED, name = USERNAME, role = User.Role.admin };
    }
    private Cart BuildCart(User user, int nb)
    {
        return new Cart()
        {
            id = UNDEFINED,
            items = [],
            numero = nb,
            userid = user.id,
            description = "test description",
            name = "test cart"
        };
    }
    private bool UserExists_ByName(User user)
    {
        User?
            u = _fac._repoUsers.GetUserByName(user.name);
        return u is not null;
    }
}
