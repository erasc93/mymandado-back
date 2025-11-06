using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace core_mandado.authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private JwtSecurityTokenHandler _jwtHandler { get; set; }
    private string _secretKey { get; init; }
    public JwtTokenGenerator(string secretKey, JwtSecurityTokenHandler jwtHandler)
    {
        _secretKey = secretKey;
        _jwtHandler = jwtHandler;
    }
    public string GenerateJwtTokenAsString(string username, bool isAdmin)
    {
        string output;

        Claim[] claims;
        claims = [
                    new Claim(type:"username", value:username),
                    //new Claim(type:"isAdmin", value:isAdmin.ToString().ToLower()),
                 ];

        JwtSecurityToken
            jwtToken = BuildJwtSecurityToken(claims);


        output = _jwtHandler.WriteToken(jwtToken);
        return output;
    }
    private JwtSecurityToken BuildJwtSecurityToken(Claim[] claims)
    {

        JwtSecurityToken output;

        byte[] secretKeyBites;
        SymmetricSecurityKey symmetricKey;
        SigningCredentials signingCredentials;

        secretKeyBites = Encoding.UTF8.GetBytes(_secretKey);
        symmetricKey = new SymmetricSecurityKey(secretKeyBites);
        signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        output = new JwtSecurityToken(
                                        claims: claims,
                                        expires: DateTime.Now.AddMinutes(300),
                                        signingCredentials: signingCredentials
                                     );
        return output;

    }

}
