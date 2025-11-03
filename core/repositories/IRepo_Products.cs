using core_mandado.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core_mandado.repositories;
public interface IRepo_Products
{
    Product[] GetAll();
    void Add(ref Product item);
    Product GetById(int id);
    void RemoveItem(int id);
    void Update(Product value);
}
//public interface IProductsRepository<T> where T:class
//{
//    T[] GetAll();
//    void Add(ref T item);
//    T GetById(int id);
//    void RemoveById(int id);
//    void Update(string value);
//}
