using api_mandado.services;
using core_mandado.Cart;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartItemsController(
        IRepo_CartItems _cartitemsRepository,
        IRepo_Users _repoUser,
        IRepo_Cart _repoCart,
        ClaimsAccessor _svc_context
    ) : ControllerBase
{
    //private IRepo_CartItems _cartitemsRepository { get; init; }
    //private IRepo_Users _repoUser { get; init; }
    //private ClaimsAccessor _svc_context { get; set; }
    //private string _username { get; set; }
    private string _username { get; init; } = _svc_context.GetUsername();
    
    // GET: api/<CartItemsController>
    [HttpGet]
    [Authorize]
    public ActionResult<CartItem[]> Get()
    {
        User user = _repoUser.GetUserByName(_username);
        CartItem[] output;
        output = _cartitemsRepository.GetAll(user);
        return Ok(output);
    }


    // POST api/<CartItemsController>
    [HttpPost]
    public ActionResult<CartItem> Post(int cartNumber, CartItem value)
    {
        //TODO: Get User from session guid
        User
            user = _repoUser.GetCurrent();
        //_repoCart.Add
        _cartitemsRepository.AddItem(user, cartNumber, ref value);
        return value;
    }

    [HttpPut]
    public void Put(int cartNumber, CartItem value)
    {
        User user = _repoUser.GetCurrent();
        _cartitemsRepository.Update(user,cartNumber, value);
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _cartitemsRepository.RemoveById(id);
    }

}
