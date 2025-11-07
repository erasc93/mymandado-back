using core_mandado.Products;

namespace core_mandado.Cart;

public class CartItem
{
    public required int id { get; set; }
    //public required User user { get; set; }
    public required Product product { get; set; }
    public required int quantity { get; set; }
    public bool isdone { get; set; }
}
