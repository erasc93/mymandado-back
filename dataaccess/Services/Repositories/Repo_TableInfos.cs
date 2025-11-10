using models.information_schema;
using Services.Dapper.Queries;

namespace Services.Repositories;
public class Repo_TableInfos
{
    private IFreeQuery _freeQuery { get; init; }
    public Repo_TableInfos(IFreeQuery freeQuery)
    {
        _freeQuery = freeQuery;
    }

    public string[] GetTableNames()
    {
        string[] output;
        string sql;
        IEnumerable<DbInfo_Tables> tables;

        sql = $"show tables";
        tables = _freeQuery.Query<DbInfo_Tables>(sql);
        output= (from t in tables select t.Tables_in_mymandado).ToArray();

        return output;
    }
    public DbInfo_Columns[] GetColumns(string tableName)
    {
        string sql;
        DbInfo_Columns[] output;
        sql = $"show columns from {tableName}";

        output = _freeQuery.Query<DbInfo_Columns>(sql)
            .ToArray();

        return output;
    }
}
