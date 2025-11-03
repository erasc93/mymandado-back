using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ALTEN_CORE_LOGIC.Authentication;

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
                        new Claim(type:"isAdmin", value:isAdmin.ToString().ToLower()),
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

    public class AppUserInfo
    {
        public string name { get; set; }
        public string guid { get; set; }
        public string token { get; set; }
    }


    //public AppUserInfo GenerateJWTAppUserInfo(string login)
    //{
    //    AppUserInfo output;
    //    Guid guid;
    //    string token;
    //    List<Claim> claims;
    //    JwtSecurityToken jwtSecurityToken;

    //    SigningCredentials signingCreds;
    //    JwtSecurityTokenHandler myTokenHandler;

    //    guid = Guid.NewGuid();

    //    claims = BuildClaims(login, guid);

    //    signingCreds = BuildSigningCredentials();
    //    jwtSecurityToken = new JwtSecurityToken(
    //                                        claims: claims,
    //                                        notBefore: DateTime.UtcNow,
    //                                        expires: DateTime.UtcNow.AddMinutes(30),
    //                                        signingCredentials: signingCreds
    //                                    );

    //    myTokenHandler = new JwtSecurityTokenHandler();
    //    token = myTokenHandler.WriteToken(jwtSecurityToken);
    //    output = new AppUserInfo()
    //    {
    //        name = login,
    //        guid = guid.ToString(),
    //        token = token
    //    };
    //    return output;
    //}
}
