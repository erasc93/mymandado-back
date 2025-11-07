using System.Security.Claims;

namespace core_mandado.Users.Security;

public interface IJwtTokenGenerator
{
    string GenerateJwtTokenAsString(Claim[] claim,double? lifeSpanInMinutes=300);
}