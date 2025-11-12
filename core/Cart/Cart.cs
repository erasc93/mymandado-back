namespace core_mandado.Cart;

public class Cart
{
    public required int id { get; set; }
    public required string name { get; set; }

    public required int userid { get; set; }
    public required int numero { get; set; }
    public required string description { get; set; }
    public required CartItem[]? items { get; set; } 
}