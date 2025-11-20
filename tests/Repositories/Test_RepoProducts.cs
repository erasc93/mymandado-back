using core_mandado.Products;
using System.Data;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoProducts(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{
    private const string PRD = "test";
    private const int UNDEFINED = -13;

    [Fact]
    public void TEST_TestProduct_DoesNotExist()
    {
        
        _fac.SecureTest(() =>
        {
            Product
                testProduct = BuildNewProduct();
            Product[]
                allUsers = _fac._repoProducts.GetAll();
            Assert.Empty(allUsers.Where(x => x.name == PRD).ToArray());
        });
    }
    [Fact]
    public void TEST_WhenCreated_PrductExists()
    {

        _fac.SecureTest(() =>
        {
            Product
                testProduct = BuildNewProduct();
            bool
            productExists = FoundByName_CompareValues(testProduct);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundByName_CompareValues(testProduct);
            Assert.True(productExists);
            productExists = FoundById(testProduct);
            Assert.True(productExists);
        });
    }
    [Fact]
    public void TEST_WhenRenamed_WhenUnitChanges()
    {
        _fac.SecureTest(() =>
        {
            Product
                testProduct = BuildNewProduct();

            bool
            productExists = FoundByName_CompareValues(testProduct);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct);
            Assert.True(productExists);

            _fac._repoProducts.RemoveItem(testProduct.id);
            productExists = FoundById(testProduct);
            Assert.False(productExists);
        });
    }
    [Fact]
    public void TEST_WhenDeleted_Success()
    {

        _fac.SecureTest(() =>
        {
            Product
                    testProduct = BuildNewProduct();
            bool
                productExists = FoundByName_CompareValues(testProduct);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct);
            Assert.True(productExists);

            _fac._repoProducts.RemoveItem(testProduct.id);
            productExists = FoundById(testProduct);
            Assert.False(productExists);
        });
    }
    [Fact]
    public void TEST_WhenUpdated()
    {
        _fac.SecureTest(() =>
        {
            Product
                testProduct = BuildNewProduct();
            bool
                productExists = FoundByName_CompareValues(testProduct);
            Assert.False(productExists);

            _fac._repoProducts.Add(ref testProduct);
            Assert.False(UNDEFINED == testProduct.id);
            productExists = FoundById(testProduct);
            Assert.True(productExists);

            const string newname = "test3";
            const string newUnit = "unot";
            testProduct.name = newname;
            testProduct.unit = newUnit;
            productExists = FoundByName_CompareValues(testProduct);
            Assert.False(productExists);

            _fac._repoProducts.Update(testProduct);
            productExists = FoundByName_CompareValues(testProduct);
            Assert.True(productExists);

        });
    }
    // --- --- ---
    private static Product BuildNewProduct()
    {
        return new Product
        {
            id = UNDEFINED,
            name = PRD,
            unit = ""
        };
    }
    private bool FoundByName_CompareValues(Product product)
    {
        bool output;
        Product[]
            allProducts = _fac._repoProducts.GetAll(),
            matching = [.. allProducts.Where(x => x.name == product.name)];

        output = matching.Length == 1;
        return output;
    }
    private bool FoundById(Product product)
    {
        bool output;
        Product?
            allProducts = _fac._repoProducts.GetById(product.id);
        output = allProducts is not null
            && allProducts.name == product.name
            && allProducts.unit == product.unit;
        return output;
    }
}
