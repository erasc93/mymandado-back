using api_mandado.services;
using core_mandado.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IRepo_Products _productsRepository) : ControllerBase
{
    // GET: api/<ProductsController>
    [HttpGet]
    public ActionResult<Product[]> Get()
    {
        Product[] output = _productsRepository.GetAll();
        return Ok(output);
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        Product? output = _productsRepository.GetById(id);
        return output is null ? NotFound() : Ok(output);
    }

    // POST api/<ProductsController>
    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product value)
    {
        _productsRepository.Add(ref value);
        return CreatedAtAction(nameof(Get), new { value.id }, value);
    }

    [HttpPut]
    public IActionResult Put([FromBody] Product value)
    {
        _productsRepository.Update(value);
        return NoContent();
    }

    // DELETE api/<ProductsController>/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _productsRepository.RemoveItem(id);
        return NoContent();
    }


}
