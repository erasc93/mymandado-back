using core_mandado.Users;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    CartItem[] GetAll(User user);
    void Add(ref CartItem item,User user);
    void Update(CartItem item,User user);
    void RemoveById(int id);
}