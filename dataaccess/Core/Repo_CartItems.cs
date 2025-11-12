using core_mandado.Cart;
using core_mandado.parameters;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using Services.Repositories.Abstractions;

namespace core;
public class Repo_CartItems : ARepository,
                        IRepo_CartItems
{

    private Repo_AnyTable<MND_CART_ITEM> _repo_CRT { get; init; }
    private Repo_AnyTable<MND_PRODUCT> _repo_PRD { get; init; }
    private IRepo_Products _repoProducts { get; init; }
    private Repo_AnyTable<MND_USERS> _repo_USR { get; init; }
    public Repo_CartItems(IQueries query,
            IRepo_Products repoProducts,
            Repo_AnyTable<MND_CART_ITEM> repoCartItems,
            Repo_AnyTable<MND_USERS> repoUsers,
            Repo_AnyTable<MND_PRODUCT> repoPRD
        ) : base(query)
    {
        _repo_CRT = repoCartItems;
        _repo_PRD = repoPRD;
        _repoProducts = repoProducts;
        _repo_USR = repoUsers;
    }

    public void Add(int cartNumber, ref CartItem item, User user)
    {
        MND_CART_ITEM model;

        bool itemExists = item.id == APP_PARAMS.instance.UNDEFINED;
        if (!itemExists)
        {
            Product prd;

            prd = item.product;
            _repoProducts.Add(ref prd);
            item.product = prd;
        }

        model = Factory.FromView(cartNumber, item, user);
        model.crt_itid=_repo_CRT.Add(ref model);
        item.id = model.crt_itid;
    }
    public void Update(int cartNumber, CartItem item, User user)
    {
        MND_CART_ITEM mnd_cart_item;

        mnd_cart_item = new MND_CART_ITEM()
        {
            crt_crtnb = cartNumber,
            crt_itid = item.id,
            crt_usrid = user.id,
            crt_prdid = item.product.id,
            crt_isdone = item.isdone,
            crt_qtty = item.quantity
        };

        _repo_CRT.Update(mnd_cart_item);
    }
    public CartItem[] GetAll(User user)
    {
        CartItem[] output;

        MND_CART_ITEM[] crts = _repo_CRT.GetAll();
        //MND_PRODUCT[] prds = _repo_PRD.GetAll();
        Product[] prds = _repoProducts.GetAll();

        CartItem[] carts = (
                        from item in crts
                        where item.crt_usrid == user.id
                        select new CartItem()
                        {
                            id = item.crt_itid,
                            quantity = item.crt_qtty,
                            isdone = item.crt_isdone,
                            product = prds.Where(x => x.id == item.crt_prdid)
                                          .FirstOrDefault()!,
                        }
                    ).ToArray();
        return carts;
    }
    public void RemoveById(int id)
    {
        bool success = _repo_CRT.Delete(
            new MND_CART_ITEM()
            {
                crt_itid = id,
                crt_crtnb = APP_PARAMS.instance.UNDEFINED,
                crt_qtty = APP_PARAMS.instance.UNDEFINED,
                crt_prdid = APP_PARAMS.instance.UNDEFINED,
                crt_usrid = APP_PARAMS.instance.UNDEFINED,
                crt_isdone = false
            });
        if (!success)
        {
            string msg;
            msg = $"Item with id ${id} could not be deleted";
            throw new Exception(msg);
        }
    }

    public void AddAll(int userid, int cartnumber, System.Data.IDbConnection c, System.Data.IDbTransaction t)
    {

        MND_PRODUCT[]
            products = _repo_PRD.GetAll(c, t);
        MND_CART_ITEM[]
            newitems = Factory.ToCART_ITEMS(userid, cartnumber, products, isdone: false);
        //foreach(newitems)
        _repo_CRT.Add(ref newitems, c, t);
    }

    // --- --- ---
    private static class Factory
    {
        internal static MND_CART_ITEM FromView(int cartNumber, CartItem item, User user)
        {
            MND_CART_ITEM output;
            output = new MND_CART_ITEM()
            {
                crt_itid = APP_PARAMS.instance.UNDEFINED,
                crt_qtty = item.quantity,
                crt_crtnb = cartNumber,
                crt_usrid = user.id,
                crt_prdid = item.product.id,
                crt_isdone = item.isdone
            };
            return output;
        }

        internal static MND_CART_ITEM[] ToCART_ITEMS(int userid, int cartnumber, MND_PRODUCT[] products, bool isdone)
        {
            MND_CART_ITEM[] output = new MND_CART_ITEM[products.Length];
            for (int i = 0; i < products.Length; i++)
            {
                MND_PRODUCT prd = products[i];
                output[i] = new MND_CART_ITEM
                {
                    crt_usrid = userid,
                    crt_crtnb = cartnumber,
                    crt_prdid = prd.prd_id,
                    //crt_itid = APP_PARAMS.instance.UNDEFINED,
                    crt_qtty = 0,
                    crt_isdone = isdone,
                };
            }
            return output;
        }
    }

}
