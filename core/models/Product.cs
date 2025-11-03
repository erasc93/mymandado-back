namespace core_mandado.models;


public class Product
{
    public required int id { get; set; }
    public required string name { get; set; }
    public string? unit { get; set; }

}
public class User
{
    public required int id { get; set; }
    public required string name { get; set; }
}
public class Cart
{
    public required User user { get; set; }
    public required CartItem[] cartItems { get; set; }
}
public class CartItem
{
    public required int id { get; set; }
    public required User user { get; set; }
    public required Product product { get; set; }
    public bool isdone { get; set; }
}
