using core_mandado.Cart;
using core_mandado.parameters;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using Services.Repositories.Abstractions;
using System.Data;

namespace core;

public class Repo_Cart(IQueries queries,
                            IRepo_CartItems _repoCartItems,
                            IRepo_Products _repoProducts,
                            Repo_AnyTable<MND_CART_ITEM> _CRT_ITEMS,
                            Repo_AnyTable<MND_USERS> _repoUsers,
                            Repo_AnyTable<MND_PRODUCT> _PRDODUCTS

    ) : ARepository(queries), IRepo_Cart
{
    private const string DESC = "";
    private const string NAME = "cart";
    public Cart AddNew(User user, int cartnumber)
    {
        MND_CART
            firstCart = new MND_CART
            {
                //car_id = APP_PARAMS.instance.UNDEFINED,
                car_usrid = user.id,
                car_crtnb = cartnumber,
                car_desc = DESC,
                car_name = NAME,
            };
        _query.crud.Add<MND_CART>(ref firstCart);
        return Factory.FillRAW(firstCart, [], []);
    }

    public void AddToCart(Product newproduct)
    {
        throw new Exception();
    }

    public Cart[] GetAll(User user)
    {
        Cart[] output = [];

        MND_CART[]
            mnd_carts = QueryCARTS(user);
        MND_CART_ITEM[]
            mnd_cartitems = QueryCART_ITEMS(user);
        MND_PRODUCT[]
            products = QueryItemsPRODUCTS(mnd_cartitems);

        output = new Cart[mnd_carts.Length];
        for (int i = 0; i < mnd_carts.Length; i++)
        {
            output[i] = Factory.BuildCart(mnd_carts[i], mnd_cartitems, products);
        }

        return output;
    }

    public Cart? GetBy(User user, int cardId)
    {
        Cart? output = null;

        MND_CART?
            mnd_carts = QueryCART(user, cardId);
        MND_CART_ITEM[]
            mnd_cartitems = QueryCART_ITEMS(user, cardId);
        MND_PRODUCT[]
            products = QueryItemsPRODUCTS(mnd_cartitems);

        //output = new Cart[mnd_carts.Length];
        //for (int i = 0; i < mnd_carts.Length; i++)
        //{
        if (mnd_carts != null)
        {
            output = Factory.BuildCart(mnd_carts, mnd_cartitems, products);
        }
        //}

        return output;
    }

    public void Remove(Cart cart)
    {
        MND_CART mndcart = Factory.FromView(cart);
        if (!_query.crud.Delete<MND_CART>(mndcart))
        {
            throw new Exception($"cart no {cart.numero}, for user : {cart.userid} could not be deleted;");
        }
    }

    public void Update(User user, Cart newCart)
    {
        MND_CART? cart = QueryCART(user, newCart.id);
        if (cart != null)
        {
            cart = Factory.FromView(newCart);
            _query.crud.Update<MND_CART>(cart);
        }
    }

    private MND_CART[] QueryCARTS(User user)
    {
        Dictionary<string, object>
            param = new() { { "@car_usrid", user.id } };
        string
            query = "SELECT * FROM  CART WHERE car_usrid=@car_usrid;";
        MND_CART[]
            mnd_carts = _query.free.Query<MND_CART>(query, param)
                                .ToArray();
        return mnd_carts;
    }
    private MND_CART? QueryCART(User user, int id)
    {
        Dictionary<string, object>
            param = new() {
                { "@car_usrid", user.id } ,
                { "@car_id", id } ,
            };
        string
            query = "SELECT * FROM  CART WHERE car_id=@car_id AND car_usrid=@car_usrid;";
        MND_CART?
            mnd_carts = _query.free.Query<MND_CART>(query, param).First();

        return mnd_carts;
    }
    private MND_CART_ITEM[] QueryCART_ITEMS(User user, int crtnb)
    {
        Dictionary<string, object>
            param = new() {
                { "@car_usrid", user.id },
                { "@car_crtnb", crtnb },
            };
        string
        query = "SELECT * from CART_ITEMS WHERE crt_usrid=@car_usrid AND crt_crtnb=@car_crtnb;";
        MND_CART_ITEM[]
            mnd_cartitems = _query.free.Query<MND_CART_ITEM>(query, param)
                                        .DistinctBy(item => (item.crt_crtnb, item.crt_prdid))
                                        .ToArray();
        return mnd_cartitems;
    }
    private MND_CART_ITEM[] QueryCART_ITEMS(User user)
    {
        Dictionary<string, object>
            param = new() { { "@car_usrid", user.id }, };
        string
        query = "SELECT * from CART_ITEMS WHERE crt_usrid=@car_usrid;";
        MND_CART_ITEM[]
            mnd_cartitems = _query.free.Query<MND_CART_ITEM>(query, param)
                                        .DistinctBy(item => (item.crt_crtnb, item.crt_prdid))
                                        .ToArray();
        return mnd_cartitems;
    }
    private MND_PRODUCT[] QueryItemsPRODUCTS(MND_CART_ITEM[] mnd_cartitems)
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
            output = _query.free.Query<MND_PRODUCT>(query, param)
                           .ToArray();
        return output;
    }

    //public CartItem AddProduct(User user, Cart cart, Product product, int qtt, bool isdone, IDbConnection conn, IDbTransaction trans)
    //{
    //    return _repoCartItems.AddProduct(user, cart, product);
    //}
    public CartItem AddProduct(User user, int cartnumber, Product product, int qtt, bool isdone = false)
    //public CartItem AddProduct(User user, int cartnumber, Product product, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        CartItem output;
        MND_CART_ITEM item;
        //Product prd;

        bool itemExists = product.id != APP_PARAMS.instance.UNDEFINED;
        if (!itemExists)
        {
            _repoProducts.Add(ref product);
        }

        item = Repo_CartItems.Factory.ToCART_ITEMS(user, cartnumber, product, qtt, isdone);
        _CRT_ITEMS.Add(ref item);

        output = Repo_CartItems.Factory.ToView(item, product);
        return output;
    }
    public void AddAllProductsAsItems(int userid, int cartnumber)
    {

        MND_PRODUCT[]
            products = _PRDODUCTS.GetAll();
        MND_CART_ITEM[]
            newitems = Repo_CartItems.Factory.ToCART_ITEMS(userid, cartnumber, products, isdone: false);
        //foreach(newitems)
        _CRT_ITEMS.Add(ref newitems);
    }


    private static class Factory
    {
        public static Cart FillRAW(MND_CART car, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return new Cart
            {
                id = car.car_id,
                numero = car.car_crtnb,
                userid = car.car_usrid,
                name = car.car_name,
                description = car.car_desc,
                items = (from MND_CART_ITEM itm in mnd_cartitems
                         select new CartItem
                         {
                             id = itm.crt_id,
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
                id = car.car_id,
                numero = car.car_crtnb,
                userid = car.car_usrid,
                items = BuildItemsInList(car, mnd_cartitems, products),
                name = car.car_name,
                description = car.car_desc,
            };
        }
        private static CartItem[] BuildItemsInList(MND_CART car_crtnb, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return (from MND_CART_ITEM itm in mnd_cartitems
                    where itm.crt_crtnb == car_crtnb.car_crtnb
                    select new CartItem
                    {
                        id = itm.crt_id,
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

        internal static MND_CART FromView(Cart cart)
        {
            return new MND_CART()
            {
                car_id = cart.id,
                car_crtnb = cart.numero,
                car_desc = cart.description,
                car_name = cart.name,
                car_usrid = cart.userid,
            };
        }


    }
}
