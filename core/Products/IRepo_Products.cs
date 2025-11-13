using System.Data;

namespace core_mandado.Products;

public interface IRepo_Products
{
    Product[] GetAll();
    Product? GetById(int id);

    void Add(ref Product product);
    void RemoveItem(int id);

    void Update(Product value);
}