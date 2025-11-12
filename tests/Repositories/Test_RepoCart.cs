using core_mandado.Cart;
using core_mandado.Users;
using Newtonsoft.Json.Schema;
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

            _fac._repoCart.Remove(newCart, conn, trans);
            carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.True(carts.Length == 1);
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
