using core_mandado.Users;
using core_mandado.Users.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api_mandado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private IJwtTokenGenerator _authManager { get; init; }
        private IRepo_Users _repoUsers { get; init; }

        public UsersController(IJwtTokenGenerator jwtTokenGenerator, IRepo_Users userManager)
        {
            _repoUsers = userManager;
            _authManager = jwtTokenGenerator;
        }

        /// <summary>
        /// permet de se connecter à l'application
        /// </summary>
        /// <param name="loginInfo">{ username :string,  password :string }</param>
        /// <returns>jwt token string </returns>
        [HttpPost("login")]
        public ObjectResult Login([FromBody] LoginInfo loginInfo)
        {
            AuthResponse token;
            string tokenstr;
            Claim[] claims;

            claims = UserClaimInfo(loginInfo);
            tokenstr = _authManager.GenerateJwtTokenAsString(claims);
            token = new AuthResponse(tokenstr);

            return Ok(token);
        }

        /// <summary>
        /// User creation
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult Create([FromBody] User appUser)
        {
            try
            {
                _repoUsers.Add(ref appUser);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // --- --- --- 
        private Claim[] UserClaimInfo(LoginInfo loginInfo)
        {
            Claim[] claims;
            bool isAdmin;

            isAdmin = _repoUsers.Login(loginInfo);
            claims = [
                        new Claim(type:"username", value:loginInfo.username),
                        new Claim(type:"isAdmin", isAdmin.ToString()),
                     ];
            return claims;
        }

    }

}
public class AuthResponse(string tokenstring)
{
    public string token { get; set; } = tokenstring;
}



