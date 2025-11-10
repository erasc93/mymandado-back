using core_mandado.Products;
using core_mandado.Users;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    void AddToCart(Product newproduct);
    Cart[] GetAll(User user);
    //void Add(int cartNumber, ref CartItem item,User user);
    //void Update(int cartNumber, CartItem item,User user);
    //void RemoveById(int id);
}
public interface IRepo_CartItems
{
    CartItem[] GetAll(User user);
    void Add(int cartNumber, ref CartItem item,User user);
    void Update(int cartNumber, CartItem item,User user);
    void RemoveById(int id);
}