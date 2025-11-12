using core_mandado.Products;
using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    Cart AddNew(User user, int cartnumber, IDbConnection? conn=null, IDbTransaction? transac=null);
    //void AddToCart(Product newproduct);
    //Cart[] GetAll(User user);
    Cart[] GetAll(User user, IDbConnection? conn=null, IDbTransaction? trns=null);

    void Remove(Cart newCart, IDbConnection? conn, IDbTransaction? trans);
    //void Add(int cartNumber, ref CartItem item,User user);
    //void Update(int cartNumber, CartItem item,User user);
    //void RemoveById(int id);
}