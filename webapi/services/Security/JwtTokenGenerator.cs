using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace api_mandado.services.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private JwtSecurityTokenHandler _jwtHandler { get; set; }
    private string _secretKey { get; init; }
    public JwtTokenGenerator(string secretKey, JwtSecurityTokenHandler jwtHandler)
    {
        _secretKey = secretKey;
        _jwtHandler = jwtHandler;
    }
    //public string GenerateJwtTokenAsString(string username, bool isAdmin)
    public string GenerateJwtTokenAsString(Claim[] claims, double? lifeSpanInMinutes = 30)
    {
        string output;
        SecurityToken jwtToken;

        jwtToken = BuildJwtSecurityToken(claims, lifeSpanInMinutes);
        output = _jwtHandler.WriteToken(jwtToken);

        return output;
    }

    private JwtSecurityToken BuildJwtSecurityToken(Claim[] claims, double? lifeSpanInMinutes)
    {

        JwtSecurityToken output;

        byte[] secretKeyBites;
        SymmetricSecurityKey symmetricKey;
        SigningCredentials signingCredentials;


        secretKeyBites = Encoding.UTF8.GetBytes(_secretKey);
        symmetricKey = new SymmetricSecurityKey(secretKeyBites);
        signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        DateTime?
            endLifeTime = lifeSpanInMinutes is not null
            ? DateTime.Now.AddMinutes((double)lifeSpanInMinutes)
            : null;
        output = new JwtSecurityToken(
                                        claims: claims,
                                        expires: endLifeTime,
                                        signingCredentials: signingCredentials,
                                        issuer: null,
                                        audience: null
                                     );
        return output;

    }

}
