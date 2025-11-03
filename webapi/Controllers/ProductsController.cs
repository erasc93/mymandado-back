using core_mandado.models;
using core_mandado.repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private IRepo_Products _productsRepository { get; init; }
    public ProductsController(IRepo_Products productsRepo)
    {
        _productsRepository = productsRepo;
    }
    // GET: api/<ProductsController>
    [HttpGet]
    public ActionResult<Product[]> Get()
    {
        Product[] output;
        output = _productsRepository.GetAll();
        return Ok(output);
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public Product Get(int id)
    {
        Product output = _productsRepository.GetById(id);
        return output;
    }

    // POST api/<ProductsController>
    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product value)
    {
        _productsRepository.Add(ref value);
        return value;
    }

    [HttpPut]
    public void Put([FromBody] Product value)
    {
        _productsRepository.Update(value);
    }

    // DELETE api/<ProductsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _productsRepository.RemoveItem(id);
    }


}
