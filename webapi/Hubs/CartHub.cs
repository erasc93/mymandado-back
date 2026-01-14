using core_mandado.Cart;
using core_mandado.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api_mandado.Hubs;

[Authorize]
public class CartHub(
        IRepo_Cart _repoCart,
        IRepo_CartShares _repoCartShares,
        IRepo_Users _repoUsers
    ) : Hub
{
    public const string CartItemsChangedEvent = "cartItemsChanged";

    public static string GroupName(int cartId) => $"cart-{cartId}";

    public async Task JoinCart(int cartId)
    {
        if (!TryGetUser(out User user)) throw new HubException("Unauthorized");

        Cart? cart = _repoCart.GetBy(cartId);
        if (cart is null) throw new HubException("Cart not found");
        if (!CanAccessCart(user, cart)) throw new HubException("Forbidden");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(cartId));
    }

    public Task LeaveCart(int cartId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(cartId));
    }

    private bool TryGetUser(out User user)
    {
        user = null!;
        string? username = Context.User?.Claims?.FirstOrDefault(c => c.Type == "username")?.Value;
        if (string.IsNullOrWhiteSpace(username)) return false;

        try
        {
            user = _repoUsers.GetUserByName(username);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool CanAccessCart(User user, Cart cart)
    {
        if (cart.userid == user.id) return true;
        return _repoCartShares.IsSharedWithUser(cart.id, user);
    }
}
