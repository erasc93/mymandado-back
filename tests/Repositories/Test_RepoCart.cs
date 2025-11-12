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
    private int UNDEFINED = -13;

    [Fact]
    public void TEST_OnUserCreated_CartCreated_CartEmpty()
    {
        _fac.SecureTest(
        (conn, trans) =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser, conn, trans));
            _fac._repoUsers.Add(ref testUser, conn, trans);
            Assert.True(UserExists_ByName(testUser, conn, trans));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            int
            nextid = carts.Length + 1;
            _fac._repoCart.AddNew(testUser, nextid, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 2);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);


        });
    }
    [Fact]
    public void TEST_DeletedCart()
    {
        _fac.SecureTest(
        (conn, trans) =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser, conn, trans));
            _fac._repoUsers.Add(ref testUser, conn, trans);
            Assert.True(UserExists_ByName(testUser, conn, trans));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = _fac._repoCart.AddNew(testUser, 1, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 2);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);

            _fac._repoCart.Remove(newCart, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
        });
    }
    [Fact]
    public void TEST_UpdateCart()
    {
        _fac.SecureTest(
        (conn, trans) =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser, conn, trans));
            _fac._repoUsers.Add(ref testUser, conn, trans);
            Assert.True(UserExists_ByName(testUser, conn, trans));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = _fac._repoCart.AddNew(testUser, 1, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 2);
            Assert.NotNull(carts[1].items);
            Assert.Empty(carts[1].items!);


            const string newdescription = "new description";
            const string newname = "new name";
            newCart.name = newname;
            newCart.description = newdescription;
            _fac._repoCart.Update(testUser, newCart, conn, trans);
            Cart updated = _fac._repoCart.GetBy(testUser, newCart.id, conn, trans);
            Assert.Equal(newdescription, updated.description);
            Assert.Equal(newname, updated.name);

            _fac._repoCart.Remove(newCart, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
        });
    }
    [Fact]
    public void TEST_AddItem()
    {
        _fac.SecureTest(
        (conn, trans) =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser, conn, trans));
            _fac._repoUsers.Add(ref testUser, conn, trans);
            Assert.True(UserExists_ByName(testUser, conn, trans));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = carts[0];

            Product
            newproduct1 = new Product()
            {
                id = UNDEFINED,
                name = "testproduct1",
                unit = "",
            },
            newproduct2 = new Product()
            {
                id = UNDEFINED,
                name = "testproduct2",
                unit = "",
            };
            Product[]
            prds = _fac._repoProducts.GetAll(conn, trans);
            Assert.Empty(prds.Where(x => x.name == newproduct1.name).ToArray());
            _fac._repoProducts.Add(ref newproduct1, conn, trans);
            _fac._repoProducts.Add(ref newproduct2, conn, trans);
            prds = _fac._repoProducts.GetAll(conn, trans);
            Assert.NotEmpty(prds);

            Product
                p = prds[0];
            _fac._repoCart.AddAll(testUser.id, newCart.numero, conn, trans);

            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.NotEmpty(carts[0].items!);


            _fac._repoCart.Remove(newCart, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 0);

            _fac._repoCartItems.GetAll(testUser, conn, trans);
        });
    }
    [Fact]
    public void TEST_AddItem_Single()
    {
        _fac.SecureTest(
        (conn, trans) =>
        {
            User
                testUser = BuildUser();

            Assert.False(UserExists_ByName(testUser, conn, trans));
            _fac._repoUsers.Add(ref testUser, conn, trans);
            Assert.True(UserExists_ByName(testUser, conn, trans));

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.Empty(carts[0].items!);

            Cart
                newCart = carts[0];

            Product
            newproduct1 = new Product()
            {
                id = UNDEFINED,
                name = "testproduct1",
                unit = "",
            },
            newproduct2 = new Product()
            {
                id = UNDEFINED,
                name = "testproduct2",
                unit = "",
            };
            Product[]
            prds = _fac._repoProducts.GetAll(conn, trans);
            Assert.Empty(prds.Where(x => x.name == newproduct1.name).ToArray());
            _fac._repoProducts.Add(ref newproduct1, conn, trans);
            _fac._repoProducts.Add(ref newproduct2, conn, trans);
            prds = _fac._repoProducts.GetAll(conn, trans);
            Assert.NotEmpty(prds);

            Product
                product = prds[0];

            CartItem
            item1 = _fac._repoCart.AddProduct(testUser, newCart.numero, product, qtt: 0, isdone: false, conn, trans);
            Assert.Throws<MySqlException>(() => _fac._repoCart.AddProduct(testUser, newCart.numero, product, qtt: 0, isdone: false, conn, trans));
            //item2 = ;

            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
            Assert.NotNull(carts[0].items);
            Assert.NotEmpty(carts[0].items!);


            _fac._repoCart.Remove(newCart, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 0);

            _fac._repoCartItems.GetAll(testUser, conn, trans);
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
    private bool UserExists_ByName(User user, IDbConnection conn, IDbTransaction trans)
    {
        User?
            u = _fac._repoUsers.GetUserByName(user.name, conn, trans);
        return u is not null;
    }
}
