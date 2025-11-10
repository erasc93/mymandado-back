using models.information_schema;
using Services.Dapper.Queries;

namespace Services.Repositories;
public class Repo_TableInfos
{
    private ITransactionQueries _query { get; init; }
    public Repo_TableInfos(ITransactionQueries queries)
    {
        _query = queries;
    }

    public string[] GetTableNames()
    {
        string[] output;
        string sql;
        IEnumerable<DbInfo_Tables> tables;

        sql = $"show tables";
        tables = _query.free.Query<DbInfo_Tables>(sql);
        output= (from t in tables select t.Tables_in_mymandado).ToArray();

        return output;
    }
    public DbInfo_Columns[] GetColumns(string tableName)
    {
        string sql;
        DbInfo_Columns[] output;
        sql = $"show columns from {tableName}";

        output = _query.free.Query<DbInfo_Columns>(sql)
            .ToArray();

        return output;
    }
}
