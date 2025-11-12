using core_mandado.Products;
using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    void AddToCart(Product newproduct);
    Cart[] GetAll(User user);
    Cart[] GetAll(User user, IDbConnection conn, IDbTransaction trns);
    //void Add(int cartNumber, ref CartItem item,User user);
    //void Update(int cartNumber, CartItem item,User user);
    //void RemoveById(int id);
}