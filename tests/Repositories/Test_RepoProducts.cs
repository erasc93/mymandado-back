using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using System.Data;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoProducts : IClassFixture<MymandadoWebAppFactory>
{
    //private readonly Repo_StoredProcedures _repoStoredPro;
    //private readonly Repo_TableInfos _repoTables;
    //private readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;

    //private readonly IQueries _queries;

    //private readonly IRepo_CartItems _repoCartItems;
    //private readonly IRepo_Products _repoProducts;
    //private readonly IRepo_Users _repoUsers;
    //private readonly IRepo_Cart _repoCart;
    private MymandadoWebAppFactory _fac { get; init; }
    public Test_RepoProducts(MymandadoWebAppFactory fac)
    {
        _fac = fac;

        //using IServiceScope
        //    scope = _fac.Services.CreateScope();

        //_repoCart = fac.Svc<IRepo_Cart>(scope)!;
        ////_repoUsers = fac.Svc<IRepo_Users>(scope)!;
        //_repoProducts = fac.Svc<IRepo_Products>(scope)!;
        //_repoDBProducts = fac.Svc<Repo_AnyTable<MND_PRODUCT>>(scope)!;
        //_repoCartItems = fac.Svc<IRepo_CartItems>(scope)!;
        //_queries = fac.Svc<IQueries>(scope)!;

    }

    private const string PRD = "test";
    private const int UNDEFINED = -13;
    [Fact]
    public void TEST_TestProduct_DoesNotExist()
    {
        _fac.SecureTest((conn, trans) =>
        {
            Product
                testProduct = BuildNewProduct();
            Product[]
                allUsers = _fac._repoProducts.GetAll(conn, trans);
            Assert.Empty(allUsers.Where(x => x.name == PRD).ToArray());
        });
    }
    [Fact]
    public void TEST_WhenCreated_PrductExists()
    {

        _fac.SecureTest((conn, trans) =>
        {
            Product
                testProduct = BuildNewProduct();
            bool
            productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct, conn, trans);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.True(productExists);
            productExists = FoundById(testProduct, conn, trans);
            Assert.True(productExists);
        });
    }
    [Fact]
    public void TEST_WhenRenamed_WhenUnitChanges()
    {
        _fac.SecureTest((conn, trans) =>
        {
            Product
                testProduct = BuildNewProduct();

            bool
            productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct, conn, trans);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct, conn, trans);
            Assert.True(productExists);

            _fac._repoProducts.RemoveItem(testProduct.id, conn, trans);
            productExists = FoundById(testProduct, conn, trans);
            Assert.False(productExists);
        });
    }
    [Fact]
    public void TEST_WhenDeleted_Success()
    {

        _fac.SecureTest((conn, trans) =>
        {
            Product
                    testProduct = BuildNewProduct();
            bool
                productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct, conn, trans);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct, conn, trans);
            Assert.True(productExists);

            _fac._repoProducts.RemoveItem(testProduct.id, conn, trans);
            productExists = FoundById(testProduct, conn, trans);
            Assert.False(productExists);
        });
    }

    [Fact]
    public void TEST_WhenUpdated()
    {
        _fac.SecureTest((conn, trans) =>
        {
            Product
                testProduct = BuildNewProduct();
            bool
                productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct, conn, trans);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct, conn, trans);
            Assert.True(productExists);

            const string newname = "test3";
            const string newUnit = "unot";
            testProduct.name = newname;
            testProduct.unit = newUnit;
            productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.False(productExists);

            _fac._repoProducts.Update(testProduct, conn, trans);
            productExists = FoundByName_CompareValues(testProduct, conn, trans);
            Assert.True(productExists);

        });
    }
    // --- --- ---
    private Product BuildNewProduct()
    {
        return new Product
        {
            id = UNDEFINED,
            name = PRD,
            unit = ""
        };
    }
    private bool FoundByName_CompareValues(Product product, IDbConnection conn, IDbTransaction trans)
    {
        bool output;
        Product[]
            allProducts = _fac._repoProducts.GetAll(conn, trans),
            matching = allProducts.Where(x => x.name == product.name).ToArray();

        output = matching.Length == 1;
        return output;
    }
    private bool FoundById(Product product, IDbConnection conn, IDbTransaction trans)
    {
        bool output;
        Product?
            allProducts = _fac._repoProducts.GetById(product.id, conn, trans);
        output = allProducts is not null
            && allProducts.name == product.name
            && allProducts.unit == product.unit;
        return output;
    }
}
