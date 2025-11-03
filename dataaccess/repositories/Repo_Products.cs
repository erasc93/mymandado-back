using core_mandado.models;
using core_mandado.repositories;
using dataaccess.information_schema.tables;
using dbaccess;
using MySql.Data.MySqlClient;
using repositories.Abstractions;
using repositories.infoSchema;

namespace repositories;
public class Repo_Products : ARepository, IRepo_Products
{
    private Repo_DbTable<MND_PRODUCT> _repo_PRD { get; init; }
    public Repo_Products(ICRUDQuery query, Repo_DbTable<MND_PRODUCT> repoTables) : base(query) { _repo_PRD = repoTables; }

    public Product[] GetAll()
    {
        Product[] output;

        output = (from p in _repo_PRD.GetAll()
                  select new Product()
                  {
                      id = p.prd_id,
                      name = p.prd_name,
                      unit = p.prd_unit
                  }).ToArray();
        return output;
    }
    public void Add(ref Product product)
    {
        MND_PRODUCT p = new MND_PRODUCT()
        {
            prd_name = product.name,
            prd_unit = product.unit
        };
        _repo_PRD.Add(ref p);
        product.id = p.prd_id;
    }

    public Product GetById(int id)
    {
        Product output;
        MND_PRODUCT p = _repo_PRD.GetById(id);

        output =
                   new Product()
                   {
                       id = p.prd_id,
                       name = p.prd_name,
                       unit = p.prd_unit
                   };

        return output;
    }

    public void RemoveItem(int id)
    {

        //string sql;
        //sql = $"select * from MND_PRODUCTS WHERE prd_name={name};";

        MND_PRODUCT p = new MND_PRODUCT() { prd_id = id, prd_name = "fuck" };
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
    private Product FromMND_PRODUCT(MND_PRODUCT p)
    {
        return new Product() { id = p.prd_id, name = p.prd_name, unit = p.prd_unit };
    }
}
