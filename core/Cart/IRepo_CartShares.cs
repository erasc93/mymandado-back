using core_mandado.Users;

namespace core_mandado.Cart;

public interface IRepo_CartShares
{
    int[] GetSharedCartIds(User user);
    bool IsSharedWithUser(int cartId, User user);
    void ShareCart(int cartId, User user);
    void UnshareCart(int cartId, User user);
}
