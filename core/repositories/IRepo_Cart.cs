using core_mandado.models;

namespace core_mandado.repositories;

public interface IRepo_Cart
{
    CartItem[] GetAll(User user);
    void Add(ref CartItem item,User user);
    void Update(CartItem item,User user);
    void RemoveById(int id);
}