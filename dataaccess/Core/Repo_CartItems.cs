using core_mandado.Cart;
using core_mandado.parameters;
using core_mandado.Products;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using Services.Repositories.Abstractions;
using System.Data;
using System.Transactions;

namespace core;
public class Repo_CartItems(IQueries _query,
                            IRepo_Products _repoProducts,
                            Repo_AnyTable<MND_CART_ITEM> _CRT_ITEMS,
                            Repo_AnyTable<MND_USERS> _repoUsers,
                            Repo_AnyTable<MND_PRODUCT> _PRDODUCTS
                            ) : ARepository(_query),
                                IRepo_CartItems
{
    public void AddItem(User user, int cartnumber, ref CartItem item, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        MND_CART_ITEM model;

        bool itemExists = item.id != APP_PARAMS.instance.UNDEFINED;
        if (!itemExists)
        {
            Product prd;

            prd = item.product;
            _repoProducts.Add(ref prd, conn, transac);
            item.product = prd;
        }

        model = Factory.FromView(cartnumber, item, user);
        model.crt_itid = _CRT_ITEMS.Add(ref model, conn, transac);
        item.id = model.crt_itid;
    }
    //public CartItem AddProduct(User user, Cart cart, Product product, IDbConnection? conn = null, IDbTransaction? transac = null)
    //{
    //    CartItem output;
    //    MND_CART_ITEM item;
    //    //Product prd;

    //    bool itemExists = product.id != APP_PARAMS.instance.UNDEFINED;
    //    if (!itemExists)
    //    {
    //        _repoProducts.Add(ref product, conn, transac);
    //    }

    //    item = Factory.ToCART_ITEMS(user, cart, product, qtt: 0, isdone: false);
    //    _CRT_ITEMS.Add(ref item, conn, transac);

    //    output = Factory.ToView(item, product);
    //    return output;
    //}
    //public void AddAll(int userid, int cartnumber, IDbConnection? c = null, IDbTransaction? t = null)
    //{

    //    MND_PRODUCT[]
    //        products = _PRDODUCTS.GetAll(c, t);
    //    MND_CART_ITEM[]
    //        newitems = Factory.ToCART_ITEMS(userid, cartnumber, products, isdone: false);
    //    //foreach(newitems)
    //    _CRT_ITEMS.Add(ref newitems, c, t);
    //}

    public void Update(User user, int cartnumber, CartItem item, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        MND_CART_ITEM mnd_cart_item;

        mnd_cart_item = new MND_CART_ITEM()
        {
            crt_crtnb = cartnumber,
            crt_itid = item.id,
            crt_usrid = user.id,
            crt_prdid = item.product.id,
            crt_isdone = item.isdone,
            crt_qtty = item.quantity
        };

        _CRT_ITEMS.Update(mnd_cart_item, conn, transac);
    }
    public CartItem[] GetAll(User user, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        CartItem[] output;

        MND_CART_ITEM[] crts = _CRT_ITEMS.GetAll(conn, transac);
        //MND_PRODUCT[] prds = _repo_PRD.GetAll();
        Product[] prds = _repoProducts.GetAll(conn, transac);

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
    public void RemoveById(int id, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        MND_CART_ITEM
            item = new()
            {
                crt_itid = id,
                crt_crtnb = APP_PARAMS.instance.UNDEFINED,
                crt_qtty = APP_PARAMS.instance.UNDEFINED,
                crt_prdid = APP_PARAMS.instance.UNDEFINED,
                crt_usrid = APP_PARAMS.instance.UNDEFINED,
                crt_isdone = false
            };
        bool
            success = _CRT_ITEMS.Delete(item, conn, transac);
        if (!success)
        {
            string msg;
            msg = $"Item with id ${id} could not be deleted";
            throw new Exception(msg);
        }
    }


    // --- --- ---
    internal static class Factory
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

        internal static MND_CART_ITEM ToCART_ITEMS(User user, int cartnumber, Product product, int qtt = 0, bool isdone = false)
        {
            MND_CART_ITEM output;
            output = new MND_CART_ITEM()
            {
                crt_itid = APP_PARAMS.instance.UNDEFINED,
                crt_crtnb = cartnumber,
                crt_usrid = user.id,
                crt_prdid = product.id,
                crt_qtty = qtt,
                crt_isdone = isdone,
            };
            return output;
        }

        internal static CartItem ToView(MND_CART_ITEM item, Product product)
        {
            CartItem output = new()
            {
                id = item.crt_itid,
                product = product,
                isdone = item.crt_isdone,
                quantity = item.crt_qtty
            };
            return output;
        }
    }

}
