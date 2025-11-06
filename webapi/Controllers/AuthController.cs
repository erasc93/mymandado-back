using core_mandado.authentication;
using core_mandado.models;
using core_mandado.repositories;
using information_schema.tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using repositories;

namespace api_mandado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private IJwtTokenGenerator _authManager { get; init; }
        private IRepo_Users _userManager { get; init; }

        public AuthController(IJwtTokenGenerator jwtTokenGenerator, IRepo_Users userManager)
        {
            _userManager = userManager;
            _authManager = jwtTokenGenerator;
        }


        /// <summary>
        /// AppUser creation
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost("account")]
        public IActionResult Account([FromBody] User appUser)
        {
            try
            {
                //_userManager.AddUser(appUser);
                _userManager.Add(ref appUser);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// permet de se connecter à l'application
        /// </summary>
        /// <param name="loginInfo">contains username and password</param>
        [HttpPost("token")]
        public IActionResult Login([FromBody] UserLoginInfo loginInfo)
        {
            string token;
            bool isAdmin;
            //User appUser;

            isAdmin = true;
            //appUser = new User() { 
            //    id = 1, 
            //    name = loginInfo.username 
            //};

            token = _authManager.GenerateJwtTokenAsString(loginInfo.username, isAdmin);
            return Ok(new { token });
        }

    }
}


