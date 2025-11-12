using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Abstractions;
using System.Data;

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

        _query.ExecuteInTransaction((connect, transact) =>
        {
            output = GetAll(user,connect,transact);
        });
        return output;
    }
    public Cart[] GetAll(User user, IDbConnection connect, IDbTransaction transact)
    {
        Cart[] output = [];

        MND_CART[]
            mnd_carts = QueryCARTS(user, connect, transact);
        MND_CART_ITEM[]
            mnd_cartitems = QueryCART_ITEMS(user, connect, transact);
        MND_PRODUCT[]
            products = QueryItemsPRODUCTS(mnd_cartitems, connect, transact);

        output = new Cart[mnd_carts.Length];
        for (int i = 0; i < mnd_carts.Length; i++)
        {
            output[i] = Factory.BuildCart(mnd_carts[i], mnd_cartitems, products);
        }

        return output;
    }
    private MND_CART[] QueryCARTS(User user, IDbConnection connect, IDbTransaction transact)
    {
        Dictionary<string, object>
            param = new() { { "@car_usrid", user.id } };
        string
            query = "SELECT * FROM  CART WHERE car_usrid=@car_usrid;";
        MND_CART[]
            mnd_carts = _query.free.Query<MND_CART>(query, param, connect, transact)
                                .ToArray();
        return mnd_carts;
    }
    private MND_CART_ITEM[] QueryCART_ITEMS(User user, IDbConnection connect, IDbTransaction transact)
    {
        Dictionary<string, object>
            param = new() { { "@car_usrid", user.id }, };
        string
        query = "SELECT * from CART_ITEMS WHERE crt_usrid=@car_usrid;";
        MND_CART_ITEM[]
            mnd_cartitems = _query.free.Query<MND_CART_ITEM>(query, param, connect, transact)
                                        .DistinctBy(item => (item.crt_crtnb, item.crt_prdid))
                                        .ToArray();
        return mnd_cartitems;
    }
    private MND_PRODUCT[] QueryItemsPRODUCTS(MND_CART_ITEM[] mnd_cartitems, IDbConnection connect, IDbTransaction transact)
    {
        Dictionary<string, object>
            param = new(){
                         {"@prd_id",(from itms in mnd_cartitems
                                    select itms.crt_prdid
                                    ).Distinct().ToArray() },
                         };
        string
            query = "SELECT * FROM PRODUCTS WHERE prd_id IN @prd_id";

        MND_PRODUCT[]
            output = _query.free.Query<MND_PRODUCT>(query, param, connect, transact)
                           .ToArray();
        return output;
    }
    private static class Factory
    {
        public static Cart FillRAW(MND_CART car, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return new Cart
            {
                numero = car.car_crtnb,
                userid = car.car_usrid,
                items = (from MND_CART_ITEM itm in mnd_cartitems
                         select new CartItem
                         {
                             id = itm.crt_itid,
                             isdone = itm.crt_isdone,
                             product = (from MND_PRODUCT prd in products
                                        where prd.prd_id == itm.crt_prdid
                                        select new Product
                                        {
                                            id = prd.prd_id,
                                            name = prd.prd_name,
                                            unit = prd.prd_unit
                                        }).First(),
                             quantity = itm.crt_qtty
                         }).ToArray()
            };
        }
        public static Cart BuildCart(MND_CART car, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return new Cart
            {
                numero = car.car_crtnb,
                userid = car.car_usrid,
                items = BuildItemsInList(car, mnd_cartitems, products)
            };
        }
        private static CartItem[] BuildItemsInList(MND_CART car_crtnb, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return (from MND_CART_ITEM itm in mnd_cartitems
                    where itm.crt_crtnb == car_crtnb.car_crtnb
                    select new CartItem
                    {
                        id = itm.crt_itid,
                        isdone = itm.crt_isdone,
                        product = BuildProductInList(itm, products),
                        quantity = itm.crt_qtty
                    }).ToArray();
        }
        private static Product BuildProductInList(MND_CART_ITEM mnd_cartitem, MND_PRODUCT[] products)
        {
            return (from MND_PRODUCT prd in products
                    where prd.prd_id == mnd_cartitem.crt_prdid
                    select new Product
                    {
                        id = prd.prd_id,
                        name = prd.prd_name,
                        unit = prd.prd_unit
                    }).First();
        }
    }
}
