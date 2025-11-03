using core_mandado.models;

namespace core_mandado.repositories;

public interface IRepo_Cart
{
    CartItem[] GetAll(User user);
    void Add(ref CartItem item);
    void Update(CartItem item);
    void RemoveById(int id);
}