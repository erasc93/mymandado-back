using api_mandado.services;
using core_mandado.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private IRepo_Products _productsRepository { get; init; }
    private string? username { get; init; }
    private ClaimsAccessor _svc_context { get; init; }
    public ProductsController(
        IRepo_Products productsRepo,
        ClaimsAccessor svc_context
        )
    {
        _svc_context = svc_context;
        _productsRepository = productsRepo;
        username = _svc_context.GetUsername();
    }
    // GET: api/<ProductsController>
    [HttpGet]
    [Authorize]
    public ActionResult<Product[]> Get()
    {
        Product[] output;
        output = _productsRepository.GetAll();
        return output;
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public Product Get(int id)
    {
        Product output;
        output = _productsRepository.GetById(id);
        return output;
    }

    // POST api/<ProductsController>
    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product value)
    {
        _productsRepository.Add(ref value);
        return CreatedAtAction(nameof(Get),new {id=value.id},value);
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
