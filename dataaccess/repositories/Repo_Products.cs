using core_mandado.models;
using core_mandado.repositories;
using dataaccess.Factories;
using dataaccess.information_schema.tables;
using dbaccess;
using MySql.Data.MySqlClient;
using repositories.Abstractions;
using repositories.infoSchema;

namespace repositories;
public class Repo_Products : ARepository, IRepo_Products
{
    private Repo_DbTable<MND_PRODUCT> _repo_PRD { get; init; }
    private FactoryProducts _facProd { get; init; }
    public Repo_Products(ICRUDQuery query, Repo_DbTable<MND_PRODUCT> repoTables, FactoryProducts facProducts) : base(query)
    {
        _repo_PRD = repoTables;
        _facProd = facProducts;
    }

    public Product[] GetAll()
    {
        Product[] output;

        output = (from p in _repo_PRD.GetAll()
                  select _facProd.ToView(p))
                  .ToArray();
        return output;
    }
    public void Add(ref Product product)
    {
        MND_PRODUCT prod;
        prod = _facProd.FromView(product);
        _repo_PRD.Add(ref prod);
        product.id = prod.prd_id;
    }

    public Product GetById(int id)
    {
        Product output;
        MND_PRODUCT prd;

        prd = _repo_PRD.GetById(id)!;
        output = _facProd.ToView(prd);

        return output;
    }

    public void RemoveItem(int id)
    {

        //string sql;
        //sql = $"select * from MND_PRODUCTS WHERE prd_name={name};";

        //MND_PRODUCT p = new MND_PRODUCT() { prd_id = id, prd_name = "fuck" };
        MND_PRODUCT p = _query.GetById<MND_PRODUCT>(id)!;
        bool success = _query.Delete(p);

        if (!success)
        {
            string msg = $"item {p} could not be removed";
            throw new Exception(msg);
        }
    }
    public void RemoveItem(string name)
    {
        string sql;
        sql = $"select * from MND_PRODUCTS WHERE prd_name={name};";
        MND_PRODUCT? p = _query.Query<MND_PRODUCT>(sql).FirstOrDefault();

    }

    public void Update(Product value)
    {
        //string sql;
        //sql = $"select * from MND_PRODUCTS WHERE prd_name={value.name};";
        //MND_PRODUCT? p = _query.Query<MND_PRODUCT>(sql).FirstOrDefault();

        MND_PRODUCT? p = ToMND_PRODUCT(value);

        _repo_PRD.Update(p);
    }
    private MND_PRODUCT ToMND_PRODUCT(Product p)
    {
        return new MND_PRODUCT() { prd_name = p.name, prd_id = p.id, prd_unit = p.unit };
    }
}
