using api_mandado.services;
using core_mandado.Cart;
using core_mandado.Users;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(
        IRepo_Cart _repoCart,
        IRepo_Users _repoUser,
        ClaimsAccessor _claimsAccessor
    ) : ControllerBase
{
    private string _username { get; init; } = _claimsAccessor.GetUsername();

    [HttpGet]
    public ActionResult<Cart[]> Get()
    {
        Cart[] output;
        User?
            user = _repoUser.GetUserByName(_username)
            ?? throw new Exception($"user '{_username}' could not be found");
        output = _repoCart.GetAll(user) ?? [];
        return Ok(output);
    }

    [HttpPost("{numero}")]
    public ActionResult<Cart> Post(int numero)
    {
        //Cart[] output;
        User?
            user = _repoUser.GetUserByName(_username)
            ?? throw new Exception($"user '{_username}' could not be found");
        //output = _repoCart.GetAll(user) ?? [];
        Cart output = _repoCart.AddEmptyCart(user, numero,name:"cart","");
        return Ok(output);
    }

}
