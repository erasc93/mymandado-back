using core_mandado.Cart;
using core_mandado.parameters;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Org.BouncyCastle.Bcpg;
using Services.Dapper.Interfaces;
using Services.Factories;
using Services.Repositories;
using Services.Repositories.Abstractions;

namespace core;

public class Repo_Cart : ARepository, IRepo_Cart
{
    public Repo_Cart(IQueries query) : base(query) { }

    public void AddToCart(Product newproduct)
    {
        throw new Exception();
    }

    public Cart[] GetAll(User user)
    {
        Cart[] output = [];
        string query;


        Dictionary<string, object> param;
        param = new Dictionary<string, object>
        {
            { "@userid",user.id},
            //{ "cartnumber",user.id}
        };
        //query = "SELECT * FROM  CART WHERE car_usrid=@userid and car_crtnb=@cartnumber;";
        query = "SELECT * FROM  CART WHERE car_usrid=@userid ;";

        MND_CART[] mndcarts;

        _query.ExecuteInTransaction((c, t) =>
        {
            mndcarts = _query.free.Query<MND_CART>(query, param, c, t).ToArray();

            MND_CART_ITEM[]
                items = _query.crud.GetAll<MND_CART_ITEM>(c, t).ToArray();

            //MND_PRODUCT[]
            //        products = _query.crud.GetAll<MND_PRODUCT>(c, t).ToArray();


            string select;
            Dictionary<string, object> ids;
            int[] indids = items.Select(x => x.crt_prdid).Distinct().ToArray();

            ids = new Dictionary<string, object> { { "@ids", indids } };

            select = "SELECT * FROM PRODUCTS WHERE prd_id IN @ids";
            MND_PRODUCT[]
                    products = _query.free.Query<MND_PRODUCT>(select, ids, c, t).ToArray();

            output = mndcarts.Select(car => new Cart
            {
                numero = car.car_crtnb,
                userid = car.car_usrid,
                //items = _query.crud.GetById<MND_CART_ITEM>(car.car_crtnb,c, t)
                items = items.Select((i) => new CartItem
                {
                    id = i.crt_itid,
                    isdone = i.crt_isdone,
                    product = new Product
                    {
                        id = i.crt_prdid,
                        name = products.Where(p => p.prd_id == i.crt_prdid).First().prd_name,
                        unit = products.Where(p => p.prd_id == i.crt_prdid).First().prd_unit,
                    },
                    quantity = i.crt_qtty
                }).ToArray()
            }).ToArray();
        });
        return output;
    }
}
public class Repo_CartItems : ARepository,
                        IRepo_CartItems
{
    private FactoryProducts _facPrds { get; init; }

    private Repo_AnyTable<MND_CART_ITEM> _repo_CRT { get; init; }
    //private Repo_DbTable<MND_PRODUCT> _repo_PRD { get; init; }
    private IRepo_Products _repoProducts { get; init; }
    private Repo_AnyTable<MND_USERS> _repo_USR { get; init; }
    public Repo_CartItems(IQueries query,
            IRepo_Products repoProducts,
            Repo_AnyTable<MND_CART_ITEM> repoCartItems,
            Repo_AnyTable<MND_USERS> repoUsers,
            FactoryProducts facProducts
        ) : base(query)
    {
        _facPrds = facProducts;
        _repo_CRT = repoCartItems;
        //_repo_PRD = repoProducts;
        _repoProducts = repoProducts;
        _repo_USR = repoUsers;
    }

    public void Add(int cartNumber, ref CartItem item, User user)
    {
        MND_CART_ITEM model;
        if (item.id == APP_PARAMS.instance.UNDEFINED)
        {
            Product prd;
            prd = item.product;

            _repoProducts.Add(ref prd);
            item.product = prd;
        }
        model = FromView(cartNumber, item, user);
        _repo_CRT.Add(ref model);
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
    //public CartItem[] GetAll(User user)
    //{
    //    CartItem[] output;

    //    MND_CART_ITEM[] crts = _repo_CRT.GetAll();
    //    MND_PRODUCT[] prds = _repo_PRD.GetAll();

    //    CartItem[] carts = (
    //                    from item in crts
    //                    where item.crt_userid == user.id
    //                    select new CartItem()
    //                    {
    //                        id = item.crt_itid,
    //                        user = user,
    //                        product = prds.Where(x => x.prd_id == item.crt_prdid).Select(x => new Product() { id = x.prd_id, name = x.prd_name, unit = x.prd_unit }).FirstOrDefault()!,
    //                        isdone = item.crt_isdone
    //                    }
    //                ).ToArray();
    //    return carts;
    //}

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
    // --- --- ---
    private MND_CART_ITEM FromView(int cartNumber, CartItem item, User user)
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

}
