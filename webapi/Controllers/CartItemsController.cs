using api_mandado.services;
using api_mandado.Hubs;
using api_mandado.models;
using core_mandado.Cart;
using core_mandado.Users;
using _user=core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartItemsController(
        IRepo_CartItems _cartitemsRepository,
        IRepo_Cart _repoCart,
        IRepo_CartShares _repoCartShares,
        IRepo_Users _repoUser,
        ClaimsAccessor _svc_context,
        IHubContext<CartHub> _hubContext
    ) : ControllerBase
{
    // GET: api/<CartItemsController>
    [HttpGet]
    public ActionResult<CartItem[]> Get()
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        CartItem[] output = _cartitemsRepository.GetAll(user) ?? [];
        return Ok(output);
    }

    // POST api/<CartItemsController>
    [HttpPost]
    public ActionResult<CartItem> Post(int? cartNumber, [FromBody] CartItem value)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;

        int resolvedCartNumber = cartNumber ?? 0;
        Cart? cart = _repoCart.GetBy(user, resolvedCartNumber);
        if (cart is null) return NotFound();

        _cartitemsRepository.AddItem(user, resolvedCartNumber, ref value);
        NotifyCartItemsChanged(cart.id, user, "item-added");
        return Ok(value);
    }

    [HttpPost("cart/{cartId}")]
    public ActionResult<CartItem> PostByCartId(int cartId, [FromBody] CartItem value)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        if (!TryGetAccessibleCart(user, cartId, out Cart cart, out ActionResult accessError)) return accessError;

        User owner = BuildOwnerUser(cart.userid);
        _cartitemsRepository.AddItem(owner, cart.numero, ref value);
        NotifyCartItemsChanged(cartId, user, "item-added");
        return Ok(value);
    }

    [HttpPut("{cartNumber}")]
    public IActionResult Put(int cartNumber, [FromBody] CartItem value)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        Cart? cart = _repoCart.GetBy(user, cartNumber);
        if (cart is null) return NotFound();
        if (!ItemBelongsToUser(user, value.id)) return NotFound();

        _cartitemsRepository.Update(user, cartNumber, value);
        NotifyCartItemsChanged(cart.id, user, "item-updated");
        return NoContent();
    }

    [HttpPut("cart/{cartId}")]
    public IActionResult PutByCartId(int cartId, [FromBody] CartItem value)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        if (!TryGetAccessibleCart(user, cartId, out Cart cart, out ActionResult accessError)) return accessError;
        if (cart.items is null || !cart.items.Any(item => item.id == value.id)) return NotFound();

        User owner = BuildOwnerUser(cart.userid);
        _cartitemsRepository.Update(owner, cart.numero, value);
        NotifyCartItemsChanged(cartId, user, "item-updated");
        return NoContent();
    }

    [HttpDelete("{crt_id}")]
    public IActionResult Delete(int crt_id)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        if (!ItemBelongsToUser(user, crt_id)) return NotFound();

        Cart? cart = _repoCart.GetAll(user)
            .FirstOrDefault(c => c.items is not null && c.items.Any(item => item.id == crt_id));

        _cartitemsRepository.RemoveById(crt_id);
        if (cart is not null)
        {
            NotifyCartItemsChanged(cart.id, user, "item-deleted");
        }
        return NoContent();
    }

    [HttpDelete("cart/{cartId}/{crt_id}")]
    public IActionResult DeleteByCartId(int cartId, int crt_id)
    {
        if (!TryGetUser(out User user, out ActionResult error)) return error;
        if (!TryGetAccessibleCart(user, cartId, out Cart cart, out ActionResult accessError)) return accessError;
        if (cart.items is null || !cart.items.Any(item => item.id == crt_id)) return NotFound();

        _cartitemsRepository.RemoveById(crt_id);
        NotifyCartItemsChanged(cartId, user, "item-deleted");
        return NoContent();
    }

    private bool TryGetUser(out User user, out ActionResult error)
    {
        user = null!;
        error = Unauthorized();
        string username = _svc_context.GetUsername();
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

    private bool ItemBelongsToUser(User user, int itemId)
        => _cartitemsRepository.GetAll(user).Any(item => item.id == itemId);

    private bool TryGetAccessibleCart(User user, int cartId, out Cart cart, out ActionResult error)
    {
        cart = null!;
        error = NotFound();

        Cart? found = _repoCart.GetBy(cartId);
        if (found is null) return false;

        if (found.userid == user.id)
        {
            cart = found;
            return true;
        }

        if (!_repoCartShares.IsSharedWithUser(cartId, user)) return false;

        cart = found;
        return true;
    }

    private static User BuildOwnerUser(int userId)
    {
        return new User
        {
            id = userId,
            name = string.Empty,
            role = _user.User.Role.friend
        };
    }

    private void NotifyCartItemsChanged(int cartId, User user, string action)
    {
        CartChangeEvent payload = new(cartId, action, user.name);
        _ = _hubContext.Clients.Group(CartHub.GroupName(cartId))
            .SendAsync(CartHub.CartItemsChangedEvent, payload);
    }
}
