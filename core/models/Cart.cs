namespace core_mandado.models;

public class Cart
{
    public required int id { get; set; }
    public required User user { get; set; }
    public required CartItem[] cartItems { get; set; }
}
public class CartItem
{
    public required int id { get; set; }
    //public required User user { get; set; }
    public required Product product { get; set; }
    public required int quantity { get; set; }
    public bool isdone { get; set; }
}
