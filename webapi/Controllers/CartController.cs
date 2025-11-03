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
    public CartController(IRepo_Cart cartitemsRepo)
    {
        _cartitemsRepository = cartitemsRepo;
    }
    // GET: api/<CartItemsController>
    [HttpGet]
    public ActionResult<CartItem[]> Get([FromBody] User user)
    {
        CartItem[] output;
        output = _cartitemsRepository.GetAll(user);
        return Ok(output);
    }


    // POST api/<CartItemsController>
    [HttpPost]
    public ActionResult<CartItem> Post([FromBody] CartItem value)
    {
        _cartitemsRepository.Add(ref value);
        return value;
    }

    [HttpPut]
    public void Put([FromBody] CartItem value)
    {
        _cartitemsRepository.Update(value);
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _cartitemsRepository.RemoveById(id);
    }

}
