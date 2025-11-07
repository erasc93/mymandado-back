using api_mandado.services;
using core_mandado.Cart;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private IRepo_Cart _cartitemsRepository { get; init; }
    private IRepo_Users _repoUser { get; init; }
    private UserContextService _svc_context { get; set; }
    private string _username { get; set; }
    public CartController(
        IRepo_Cart cartitemsRepo, IRepo_Users repoUser,
        UserContextService svc_context
        )
    {
        _cartitemsRepository = cartitemsRepo;
        _repoUser = repoUser;
        _svc_context = svc_context;
        _username = _svc_context.GetUsername();
    }
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
    public ActionResult<CartItem> Post([FromBody] CartItem value)
    {
        //TODO: Get User from session guid
        User user = _repoUser.GetCurrent();
        _cartitemsRepository.Add(ref value, user);
        return value;
    }

    [HttpPut]
    public void Put([FromBody] CartItem value)
    {
        User user = _repoUser.GetCurrent();
        _cartitemsRepository.Update(value, user);
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _cartitemsRepository.RemoveById(id);
    }

}
