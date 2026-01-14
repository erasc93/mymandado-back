using api_mandado.models;
using api_mandado.services.Security;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using _core = core_mandado.Users;

namespace api_mandado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController(IJwtTokenGenerator _jwtTokenGenerator, IRepo_Users _repoUsers) : ControllerBase
    {
        /// <summary>
        /// permet de se connecter à l'application
        /// </summary>
        /// <param name="loginInfo">{ username :string,  password :string }</param>
        /// <returns>jwt token string </returns>
        [HttpPost("login")]
        public ActionResult<AuthResponse> Login(LoginInfo loginInfo)
        {
            if (string.IsNullOrWhiteSpace(loginInfo.username)) return BadRequest();

            try
            {
                User user = _repoUsers.GetUserByName(loginInfo.username);
                _repoUsers.Login(loginInfo);
                Claim[] claims = UserClaimInfo(user);
                string tokenstr = _jwtTokenGenerator.GenerateJwtTokenAsString(claims);
                return new AuthResponse(user, tokenstr);
            }
            catch
            {
                return Unauthorized();
            }
        }
        [HttpGet]
        public string[] GetUserNames()
        {

            return [..from u in _repoUsers.GetAll()
                      select u.name];
        }

        /// <summary>
        /// User creation
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult<User> Create([FromBody] LoginInfo loginInfo)
        {
            try
            {
                User
                    user = _repoUsers.AddByName(loginInfo, _core.User.Role.friend);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // --- --- --- 
        private static Claim[] UserClaimInfo(_core.User user)
        {
            bool isAdmin = user.role == _core.User.Role.admin;
            return [
                        new Claim(type:"username", value:user.name),
                        new Claim(type:"isAdmin", isAdmin.ToString()),
                   ];
        }

    }

}



