using dbaccess;
using information_schema.models;

namespace repositories.infoSchema;
public class Repo_Tables
{
    private IFreeQuery _freeQuery { get; init; }
    public Repo_Tables(IFreeQuery freeQuery)
    {
        _freeQuery = freeQuery;
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
    public string[] GetTableNames(string tableName)
    {
        string[] output;
        string sql;
        IEnumerable<DbInfo_Tables> tables;

        sql = $"show tables";
        tables = _freeQuery.Query<DbInfo_Tables>(sql);
        output= (from t in tables select t.Tables_in_mymandado).ToArray();

        return output;
    }
}
