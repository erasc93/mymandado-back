using api_mandado.models;
using api_mandado.services.Security;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public ObjectResult Login(LoginInfo loginInfo)
        {
            AuthResponse token;
            string tokenstr;
            Claim[] claims;

            User
            user = _repoUsers.GetUserByName(loginInfo.username) ?? throw new Exception($"user {loginInfo.username} could not be found");
            claims = UserClaimInfo(loginInfo);
            tokenstr = _jwtTokenGenerator.GenerateJwtTokenAsString(claims);
            token = new AuthResponse(user, tokenstr) ;

            return Ok(token);
        }
        [HttpGet]
        public string[] GetUsers()
        {
            return [.._repoUsers.GetAll().Select(x => x.name)];
            //return ["ha","he","hi","ho","hu"];
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



