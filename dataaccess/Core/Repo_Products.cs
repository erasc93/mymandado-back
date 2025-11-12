using core_mandado.Products;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using Services.Repositories.Abstractions;
using System.Data;

namespace core;
public class Repo_Products(
                            IQueries query,
                            Repo_AnyTable<MND_PRODUCT> _repo_PRD
                            ) : ARepository(query), IRepo_Products
{

    public Product[] GetAll(IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        Product[]
            output = (from p in _repo_PRD.GetAll(connection, transaction)
                      select Factory.ToView(p))
                      .ToArray();
        return output;
    }
    public Product? GetById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        Product?
            output = null;
        MND_PRODUCT?
            prd = _repo_PRD.GetById(id, connection, transaction)!;

        if (prd is not null)
        {
            output = Factory.ToView(prd);
        }
        return output;
    }

    public void Add(ref Product product, IDbConnection? conn = null, IDbTransaction? trans = null)
    {
        MND_PRODUCT
            prod = Factory.FromView(product);
        _repo_PRD.Add(ref prod, conn, trans);
        product.id = prod.prd_id;
    }
    public void RemoveItem(int id, IDbConnection? conn = null, IDbTransaction? transac = null)
    {

        //string sql;
        //sql = $"select * from MND_PRODUCTS WHERE prd_name={name};";

        //MND_PRODUCT p = new MND_PRODUCT() { prd_id = id, prd_name = "fuck" };
        MND_PRODUCT
            prd = _query.crud.GetById<MND_PRODUCT>(id, conn, transac)!;
        bool success = _query.crud.Delete(prd, conn, transac);

        if (!success)
        {
            string
                msg = $"item {prd} could not be removed";
            throw new Exception(msg);
        }
    }

    public void Update(Product value, IDbConnection? conn = null, IDbTransaction? transac = null)
    {
        MND_PRODUCT
            p = Factory.FromView(value);
        _repo_PRD.Update(p, conn, transac);
    }


    // --- --- ---
    private static class Factory
    {
        public static MND_PRODUCT FromView(Product product)
        {
            MND_PRODUCT output;
            output = new MND_PRODUCT()
            {
                prd_id = product.id,
                prd_name = product.name,
                prd_unit = product.unit
            };
            return output;
        }
        public static Product ToView(MND_PRODUCT p)
        {
            return new Product()
            {
                id = p.prd_id,
                name = p.prd_name,
                unit = p.prd_unit
            };
        }
    }
}
