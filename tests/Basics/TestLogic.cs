//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.Basics;

using System.Data;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.tables;
using Services.Repositories;
using tests_mandado.utilities;
using Xunit;

public class TestLogic(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{




    [Fact]
    public void UserCRUD()
    {
        _fac.SecureTest((c, t) =>
        {

            User[] allUsers = _fac._repoUsers.GetAll(c, t);
            Assert.NotEmpty(allUsers);

            string userName = "usertest";
            User? u;
            u = _fac._repoUsers.GetUserByName(userName, c, t);
            Assert.Null(u);

            u = _fac._repoUsers.AddByName(userName, c, t);
            u = _fac._repoUsers.GetUserByName(userName, c, t);
            Assert.NotNull(u);

            _fac._repoUsers.Delete(u, c, t);
            u = _fac._repoUsers.GetUserByName(userName, c, t);
            Assert.Null(u);
        });
    }
    [Fact]
    public void TABLE_CRUD()
    {
        _fac.SecureTest((c, t) =>
        {
            MND_PRODUCT[] all;
            all = _fac._repoDBProducts.GetAll(c,t);

            string notHere = "nothere";
            MND_PRODUCT? p = new MND_PRODUCT() { prd_name = notHere };

            all = _fac._repoDBProducts.GetAll(c,t);
            Assert.DoesNotContain(all, x => x.prd_name == notHere);
            _fac._repoDBProducts.Add(ref p,c,t);

            all = _fac._repoDBProducts.GetAll(c, t);
            p = all.Where(x => x.prd_name == notHere).FirstOrDefault();
            Assert.Contains(all, x => x.prd_id == p.prd_id); // id KEY

            _fac._repoDBProducts.Delete(p,c,t);
            all = _fac._repoDBProducts.GetAll(c, t);
            Assert.DoesNotContain(all, x => x.prd_name == notHere);
        });
    }
}