using core_mandado.Users;

namespace api_mandado.models;

public class AuthResponse(User userobject, string tokenstring)
{
    public User user { get; init; } = userobject;
    public  string token { get; init; } = tokenstring;
}



