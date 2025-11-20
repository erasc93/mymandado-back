using core_mandado.Users;

namespace api_mandado.models;

public class AuthResponse(User userobject, string tokenstring)
{
    public  string token { get; init; } = tokenstring;
    public User user { get; init; } = userobject;
}



