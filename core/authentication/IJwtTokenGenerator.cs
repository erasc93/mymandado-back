namespace core_mandado.authentication;

public interface IJwtTokenGenerator
{
    string GenerateJwtTokenAsString(string username,bool isAdmin);
}
