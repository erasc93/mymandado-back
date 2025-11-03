namespace ALTEN_CORE_LOGIC.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateJwtTokenAsString(string username,bool isAdmin);
}
