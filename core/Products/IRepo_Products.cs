using System.Data;

namespace core_mandado.Products;

public interface IRepo_Products
{
    Product[] GetAll(IDbConnection? connection = null, IDbTransaction? transaction = null);
    Product? GetById(int id,IDbConnection? connection = null, IDbTransaction? transaction = null);

    void Add(ref Product product, IDbConnection? conn = null, IDbTransaction? trans = null);
    void RemoveItem(int id, IDbConnection? conn = null, IDbTransaction? trans = null);

    void Update(Product value,IDbConnection? connection = null, IDbTransaction? transaction = null);
}