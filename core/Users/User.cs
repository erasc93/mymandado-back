namespace core_mandado.Users;

public class User
{
    public required int id { get; set; }
    public required string name { get; set; }
    public required Role role { get; set; }
    public enum Role
    {
        visitor = 0,
        admin = 13,
        friend = 1
    }
}
