using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Parameters;
using Services.Repositories;
using Services.Repositories.Generics;
using System.Data;

namespace core;

public class Repo_Cart(IQueries queries,
                            IRepo_Products _repoProducts,
                            Repo_AnyTable<MND_CART_ITEM> _CRT_ITEMS,
                            Repo_AnyTable<MND_PRODUCT> _PRDODUCTS

    ) : ARepository(queries), IRepo_Cart
{
    private const string DESC = "";
    private const string NAME = "cart";
    public Cart AddEmptyCart(User user, int numero, string name, string description)
    {
        Cart output;
        int car_crtnb = numero;

        MND_CART
            firstCart = new()
            {
                //car_id = APP_PARAMS.instance.UNDEFINED,
                car_usrid = user.id,
                car_crtnb = car_crtnb,
                car_name = name,
                car_desc = description,
            };

        _query.crud.Add<MND_CART>(ref firstCart);
        output = Factory.FillRAW(firstCart, [], []);
        return output;
    }

    //public static void AddToCart(Product newproduct)
    //{
    //    throw new Exception();
    //}

    public Cart[] GetAll(User user)
    {
        Cart[] output;

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

    public Cart? GetBy(User user, int numero)
    {
        Cart? output = null;

        MND_CART?
            mnd_carts = QueryCART(user, numero);
        MND_CART_ITEM[]
            mnd_cartitems = QueryCART_ITEMS(user, numero);
        MND_PRODUCT[]
            products = QueryItemsPRODUCTS(mnd_cartitems);

        if (mnd_carts != null)
        {
            output = Factory.BuildCart(mnd_carts, mnd_cartitems, products);
        }

        return output;
    }
    public Cart? GetBy(int id)
    {
        Cart? output = null;

        MND_CART?
            mnd_carts = QueryCART(id);
        if (mnd_carts is null) return null;

        MND_CART_ITEM[]
            mnd_cartitems = QueryCART_ITEMS(mnd_carts.car_usrid, mnd_carts.car_crtnb);
        MND_PRODUCT[]
            products = QueryItemsPRODUCTS(mnd_cartitems);

        output = Factory.BuildCart(mnd_carts, mnd_cartitems, products);

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
        MND_CART? cart = QueryCART(user, newCart.numero);
        if (cart != null)
        {
            cart = Factory.FromView(newCart);
            _query.crud.Update<MND_CART>(cart);
        }
    }
    public void Update(Cart newCart)
    {
        MND_CART? cart = QueryCART(newCart.id);
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
            mnd_carts = [.. _query.free.Query<MND_CART>(query, param)];
        return mnd_carts;
    }
    private MND_CART? QueryCART(int id)
    {
        Dictionary<string, object>
            param = new() {
                                { "@car_id", id } ,
                            };
        string
            query = "SELECT * FROM  CART WHERE car_id=@car_id";
        MND_CART?
            mnd_carts = _query.free.Query<MND_CART>(query, param).FirstOrDefault();

        return mnd_carts;
    }
    private MND_CART? QueryCART(User user, int cartNumber)
    {
        Dictionary<string, object>
            param = new() {
                                { "@car_usrid", user.id } ,
                                { "@car_crtnb", cartNumber } ,
                            };
        string
            query = "SELECT * FROM  CART WHERE car_usrid=@car_usrid AND car_crtnb=@car_crtnb;";
        MND_CART?
            mnd_carts = _query.free.Query<MND_CART>(query, param).FirstOrDefault();

        return mnd_carts;
    }
    private MND_CART_ITEM[] QueryCART_ITEMS(int userId, int cartNumber)
    {
        Dictionary<string, object>
            param = new() {
                { "@car_usrid", userId },
                { "@car_crtnb", cartNumber },
            };
        string
        query = "SELECT * from CART_ITEMS WHERE crt_usrid=@car_usrid AND crt_crtnb=@car_crtnb;";
        MND_CART_ITEM[]
            mnd_cartitems = [.. _query.free.Query<MND_CART_ITEM>(query, param)
                                            .DistinctBy(item => (item.crt_crtnb, item.crt_prdid))];
        return mnd_cartitems;
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
            mnd_cartitems = [.. _query.free.Query<MND_CART_ITEM>(query, param)
                                            .DistinctBy(item => (item.crt_crtnb, item.crt_prdid))];
        return mnd_cartitems;
    }
    private MND_CART_ITEM[] QueryCART_ITEMS(User user)
    {
        Dictionary<string, object>
            param = new() { { "@car_usrid", user.id }, };
        string
        query = "SELECT * from CART_ITEMS WHERE crt_usrid=@car_usrid;";
        MND_CART_ITEM[]
            mnd_cartitems = [.. _query.free.Query<MND_CART_ITEM>(query, param).DistinctBy(item => (item.crt_crtnb, item.crt_prdid))];
        return mnd_cartitems;
    }
    private MND_PRODUCT[] QueryItemsPRODUCTS(MND_CART_ITEM[] mnd_cartitems)
    {
        if (mnd_cartitems.Length == 0) return [];

        Dictionary<string, object>
            param = new(){
                         {"@prd_id",(from itms in mnd_cartitems
                                    select itms.crt_prdid
                                    ).Distinct().ToArray() },
                         };
        string
            query = "SELECT * FROM PRODUCTS WHERE prd_id IN @prd_id";

        MND_PRODUCT[]
            output = [.. _query.free.Query<MND_PRODUCT>(query, param)];
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
        public static Cart FillRAW(MND_CART mnd_cart, MND_CART_ITEM[] mnd_cartitems, MND_PRODUCT[] products)
        {
            return new Cart
            {
                id = mnd_cart.car_id,
                numero = mnd_cart.car_crtnb,
                userid = mnd_cart.car_usrid,
                name = mnd_cart.car_name,
                description = mnd_cart.car_desc,
                items = [.. (from MND_CART_ITEM itm in mnd_cartitems
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
                         })]
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
            return [.. (from MND_CART_ITEM itm in mnd_cartitems
                    where itm.crt_crtnb == car_crtnb.car_crtnb
                    select new CartItem
                    {
                        id = itm.crt_id,
                        isdone = itm.crt_isdone,
                        product = BuildProductInList(itm, products),
                        quantity = itm.crt_qtty
                    })];
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
