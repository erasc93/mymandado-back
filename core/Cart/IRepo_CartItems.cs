using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_CartItems
{
    CartItem[] GetAll(User user);
    void AddItem(User user, int cartnumber, ref CartItem item);
    void Update(User user, int cartNumber, CartItem item);
    void RemoveById(int id);
}