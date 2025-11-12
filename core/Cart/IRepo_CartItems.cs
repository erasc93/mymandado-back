using core_mandado.Products;
using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_CartItems
{
    CartItem[] GetAll(User user, IDbConnection? c = null, IDbTransaction? t = null);

    void AddItem(User user, int cartnumber, ref CartItem item, IDbConnection? c = null, IDbTransaction? t = null);
    void Update(User user, int cartNumber, CartItem item, IDbConnection? c = null, IDbTransaction? t = null);
    void RemoveById(int id, IDbConnection? c = null, IDbTransaction? t = null);


    //void AddAll(int userid, int cartid, IDbConnection? c = null, IDbTransaction? t = null);
    //CartItem AddProduct(User user, Cart cart, Product product, IDbConnection? c = null, IDbTransaction? t = null);
}