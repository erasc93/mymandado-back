using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_CartItems
{
    CartItem[] GetAll(User user);
    void Add(int cartNumber, ref CartItem item,User user);
    void Update(int cartNumber, CartItem item,User user);
    void RemoveById(int id);
    
    void AddAll(int userid,int cartid, IDbConnection c, IDbTransaction t);
}