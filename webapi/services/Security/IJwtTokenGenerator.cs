using System.Security.Claims;

namespace api_mandado.services.Security;

public interface IJwtTokenGenerator
{
    string GenerateJwtTokenAsString(Claim[] claim,double? lifeSpanInMinutes=300);
}