using core_mandado.Products;
using core_mandado.Users;
using System.Data;

namespace core_mandado.Cart;

public interface IRepo_Cart
{
    Cart[] GetAll(User user);
    Cart? GetBy(User testUser, int id);

    Cart AddEmptyCart(User user, int cartnumber);
    void Remove(Cart cart);

    void Update(User user, Cart newCart);

    void AddAllProductsAsItems(int userid, int cartid);
    CartItem AddProduct(User user, int cartnumber, Product product, int qtt, bool isdone = false);
}