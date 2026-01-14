using api_mandado.services;
using api_mandado.models;
using core_mandado.Cart;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartController(
        IRepo_Cart _repoCart,
        IRepo_CartShares _repoCartShares,
        IRepo_Users _repoUser,
        ClaimsAccessor _claimsAccessor
    ) : ControllerBase
{
    [HttpGet]
    public ActionResult<Cart[]> Get()
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        Cart[] owned = _repoCart.GetAll(user) ?? [];
        int[] sharedIds = _repoCartShares.GetSharedCartIds(user);
        Cart[] shared = sharedIds
            .Select(id => _repoCart.GetBy(id))
            .Where(cart => cart is not null)
            .Cast<Cart>()
            .ToArray();

        return Ok(owned.Concat(shared).ToArray());
    }

    [HttpPost]
    public ActionResult<Cart> Post()
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        int nextNumero = NextCartNumber(user);
        Cart output = _repoCart.AddEmptyCart(user, nextNumero, name: "cart", "");
        return Ok(output);
    }

    [HttpPost("{numero}")]
    public ActionResult<Cart> Post(int numero)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        if (_repoCart.GetBy(user, numero) is not null) return Conflict($"Cart {numero} already exists.");

        Cart output = _repoCart.AddEmptyCart(user, numero, name: "cart", "");
        return Ok(output);
    }

    [HttpPost("{cartId}/share")]
    public IActionResult Share(int cartId, [FromBody] ShareCartRequest request)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        if (string.IsNullOrWhiteSpace(request.username)) return BadRequest();

        Cart? cart = _repoCart.GetBy(cartId);
        if (cart is null) return NotFound();
        if (cart.userid != user.id) return Forbid();

        User targetUser;
        try
        {
            targetUser = _repoUser.GetUserByName(request.username);
        }
        catch
        {
            return NotFound();
        }
        if (targetUser.id == user.id) return Conflict("Cannot share with yourself.");
        if (_repoCartShares.IsSharedWithUser(cartId, targetUser)) return Conflict("Already shared.");

        _repoCartShares.ShareCart(cartId, targetUser);
        return NoContent();
    }

    [HttpDelete("{cartId}")]
    public IActionResult Delete(int cartId)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        Cart? cart = _repoCart.GetBy(cartId);
        if (cart is null) return NotFound();
        if (cart.userid != user.id) return Forbid();

        _repoCart.Remove(cart);
        return NoContent();
    }

    private int NextCartNumber(User user)
    {
        Cart[] owned = _repoCart.GetAll(user) ?? [];
        int nextNumero = owned.Length == 0
            ? 0
            : owned.Max(c => c.numero) + 1;
        return nextNumero;
    }

    private bool TryGetUser(out User user, out ActionResult error)
    {
        user = null!;
        error = Unauthorized();
        string username = _claimsAccessor.GetUsername();
        if (string.IsNullOrWhiteSpace(username)) return false;

        try
        {
            user = _repoUser.GetUserByName(username);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
