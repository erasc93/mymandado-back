using core_mandado.models;
using core_mandado.repositories;
using dataaccess.information_schema.tables;
using dbaccess;
using repositories.Abstractions;
using repositories.infoSchema;

namespace repositories;

public class Repo_Cart : ARepository, IRepo_Cart
{
    private const int UNDEFINED = -13;
    private Repo_DbTable<MND_CART_ITEM> _repo_CRT { get; init; }
    private Repo_DbTable<MND_PRODUCT> _repo_PRD { get; init; }
    private Repo_DbTable<MND_USERS> _repo_USR { get; init; }
    public Repo_Cart(ICRUDQuery query,
        Repo_DbTable<MND_CART_ITEM> repoCartItems,
        Repo_DbTable<MND_PRODUCT> repoProducts,
        Repo_DbTable<MND_USERS> repoUsers
        ) : base(query)
    {
        _repo_CRT = repoCartItems;
        _repo_PRD = repoProducts;
        _repo_USR = repoUsers;
    }

    public void Add(ref CartItem item)
    {
        MND_CART_ITEM model;
        model = FromView(item);
        _repo_CRT.Add(ref model);
        item.id = model.crt_itid;
    }

    public void Update(CartItem item)
    {
        MND_CART_ITEM mnd_cart_item;
        mnd_cart_item = new MND_CART_ITEM()
        {
            crt_itid = item.id,
            crt_userid = item.user.id,
            crt_prdid = item.product.id,
            crt_isdone = item.isdone
        };
        _repo_CRT.Update(mnd_cart_item);
    }

    public CartItem[] GetAll(User user)
    {
        CartItem[] output;

        MND_CART_ITEM[] crts = _repo_CRT.GetAll();
        MND_PRODUCT[] prds = _repo_PRD.GetAll();

        CartItem[] carts = (
                        from item in crts
                        where item.crt_userid == user.id
                        select new CartItem()
                        {
                            id = item.crt_itid,
                            user = user,
                            product = prds.Where(x => x.prd_id == item.crt_prdid).Select(x => new Product() { id = x.prd_id, name = x.prd_name, unit = x.prd_unit }).FirstOrDefault()!,
                            isdone = item.crt_isdone
                        }
                    ).ToArray();
        return carts;
    }

    public void RemoveById(int id)
    {
        bool success=_repo_CRT.Delete(new MND_CART_ITEM()
        {
            crt_itid = id,
            crt_prdid=-13,
            crt_userid=-13,
            crt_isdone=false
        });
        if(!success)
        {
            string msg;
            msg = $"Item with id ${id} could not be deleted";
            throw new Exception(msg);
        }
    }
    // --- --- ---
    private MND_CART_ITEM FromView(CartItem item)
    {
        MND_CART_ITEM output;
        output = new MND_CART_ITEM()
        {
            crt_itid = UNDEFINED,
            crt_userid = item.user.id,
            crt_prdid = item.product.id,
            crt_isdone = item.isdone
        };
        return output;
    }

}
