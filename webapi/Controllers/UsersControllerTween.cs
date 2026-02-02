using api_mandado.models;
using api_mandado.services.Security;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _core = core_mandado.Users;

namespace api_mandado.Controllers;



public partial class UsersController
{
    [HttpPost("logint0")]
    public ActionResult<AuthResponse> LoginTest0(LoginInfo loginInfo)
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
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }
    [HttpPost("logint1")]
    public ActionResult<AuthResponse> LoginTest1([FromBody] LoginInfo loginInfo)
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
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }

    [HttpPost("logint2")]
    public ActionResult<AuthResponse> LoginTest2([FromBody] LoginInfo loginInfo)
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
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }
    private ActionResult<AuthResponse> ExceptionHandle(Exception e)
    {
            string msg = 
                $""""
                message : {e.Message}
                credentials : {e.StackTrace}
                """";

            return Ok(msg);
    }

    [HttpPost("logint3")]
    public ActionResult<AuthResponse> LoginTest3([FromBody] LoginInfo loginInfo)
    {
        if (string.IsNullOrWhiteSpace(loginInfo.username)) return BadRequest();

        try
        {
            User user = _repoUsers.GetUserByName(loginInfo.username);
            _repoUsers.Login(loginInfo);
            Claim[] claims = UserClaimInfo(user);
            JwtSecurityToken t = new();
            //string tokenstr = _jwtTokenGenerator.GenerateJwtTokenAsString(claims);
            return new AuthResponse(user, "not a token");
        }
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }
    [HttpPost("logint4")]
    public ActionResult<AuthResponse> LoginTest4([FromBody] LoginInfo loginInfo)
    {
        if (string.IsNullOrWhiteSpace(loginInfo.username)) return BadRequest();

        try
        {
            User user = _repoUsers.GetUserByName(loginInfo.username);
            _repoUsers.Login(loginInfo);
            return new AuthResponse(user, "not a token");
        }
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }
    [HttpPost("logint5")]
    public ActionResult<AuthResponse> LoginTest5([FromBody] LoginInfo loginInfo)
    {
        if (string.IsNullOrWhiteSpace(loginInfo.username)) return BadRequest();

        try
        {
            User user = new() { id = 0, name = "Not a user", role = _core.User.Role.visitor };
            return new AuthResponse(user, "not a token");
        }
        catch (Exception e)
        {
            return ExceptionHandle(e);
        }
    }
    [HttpPost("logint6")]
    public ActionResult<AuthResponse> LoginTest6([FromBody] LoginInfo loginInfo)
    {
        return Ok("Hi");
    }

    [HttpPost("logint7")]
    public ActionResult<AuthResponse> LoginTest7()
    {
        return Ok("Hi");
    }

    [AllowAnonymous]
    [HttpPost("logint8")]
    public ActionResult<AuthResponse> LoginTest8()
    {
        return Ok("Hi");
    }
    
}
