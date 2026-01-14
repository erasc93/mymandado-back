using core_mandado.Cart;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Parameters;
using Services.Repositories;
using Services.Repositories.Generics;

namespace core;

public class Repo_CartShares(
        IQueries query,
        Repo_AnyTable<MND_CART_SHARE> _shares
    ) : ARepository(query), IRepo_CartShares
{
    public int[] GetSharedCartIds(User user)
    {
        Dictionary<string, object> param = new()
        {
            { "@userId", user.id }
        };
        const string sql = "SELECT csh_carid FROM CART_SHARES WHERE csh_usrid=@userId;";
        return [.. _query.free.Query<int>(sql, param)];
    }

    public bool IsSharedWithUser(int cartId, User user)
    {
        Dictionary<string, object> param = new()
        {
            { "@cartId", cartId },
            { "@userId", user.id }
        };
        const string sql = "SELECT COUNT(*) FROM CART_SHARES WHERE csh_carid=@cartId AND csh_usrid=@userId;";
        int count = _query.free.Query<int>(sql, param).FirstOrDefault();
        return count > 0;
    }

    public void ShareCart(int cartId, User user)
    {
        MND_CART_SHARE share = new()
        {
            csh_id = APP_PARAMS.instance.UNDEFINED,
            csh_carid = cartId,
            csh_usrid = user.id
        };
        _shares.Add(ref share);
    }

    public void UnshareCart(int cartId, User user)
    {
        Dictionary<string, object> param = new()
        {
            { "@cartId", cartId },
            { "@userId", user.id }
        };
        const string sql = "DELETE FROM CART_SHARES WHERE csh_carid=@cartId AND csh_usrid=@userId;";
        _query.free.Query(sql, param);
    }
}
