using core_mandado.models;
using core_mandado.repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private IRepo_Cart _cartitemsRepository { get; init; }
    private IRepo_Users _repoUser{ get; init; }
    public CartController(
        IRepo_Cart cartitemsRepo, IRepo_Users repoUser)
    {
        _cartitemsRepository = cartitemsRepo;
        _repoUser = repoUser;
    }
    // GET: api/<CartItemsController>
    [HttpGet]
    //public ActionResult<CartItem[]> Get([FromBody] User user)
    public ActionResult<CartItem[]> Get()
    {
        User user = _repoUser.GetCurrent();
        CartItem[] output;
        output = _cartitemsRepository.GetAll(user);
        return Ok(output);
    }


    // POST api/<CartItemsController>
    [HttpPost]
    public ActionResult<CartItem> Post([FromBody] CartItem value )
    {
        //TODO: Get User from session guid
        User user = _repoUser.GetCurrent();
        _cartitemsRepository.Add(ref value,user);
        return value;
    }

    [HttpPut]
    public void Put([FromBody] CartItem value)
    {
        User user = _repoUser.GetCurrent();
        _cartitemsRepository.Update(value,user);
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _cartitemsRepository.RemoveById(id);
    }

}
