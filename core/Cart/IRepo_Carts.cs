using core_mandado.Products;
using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    Cart AddNew(User user, int cartnumber, IDbConnection? conn = null, IDbTransaction? transac = null);
    CartItem AddProduct(User user, int cartnumber, Product product, int qtt, bool isdone = false, IDbConnection? conn = null, IDbTransaction? transac = null);

    //void AddToCart(Product newproduct);
    //Cart[] GetAll(User user);
    Cart[] GetAll(User user, IDbConnection? conn = null, IDbTransaction? trns = null);
    Cart GetBy(User testUser, int id, IDbConnection? conn = null, IDbTransaction? trans = null);
    void Remove(Cart cart, IDbConnection? conn = null, IDbTransaction? trans = null);
    void Update(User user, Cart newCart, IDbConnection? conn = null, IDbTransaction? trans = null);
    //void Add(int cartNumber, ref CartItem item,User user);
    //void Update(int cartNumber, CartItem item,User user);
    //void RemoveById(int id);


    void AddAll(int userid, int cartid, IDbConnection? c = null, IDbTransaction? t = null);

}