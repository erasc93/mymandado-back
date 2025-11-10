using models.information_schema;
using Mysqlx.Crud;
using Services.Dapper.Queries;

namespace Services.Repositories;

public class Repo_StoredProcedures
{
    private ITransactionQueries _query { get; init; }
    public Repo_StoredProcedures(ITransactionQueries freeQuery)
    {
        _query = freeQuery;
    }

    public StoredProcedure[] GetStoredProcedures()
    {
        string sql;
        sql = "select  ROUTINE_SCHEMA , ROUTINE_NAME , ROUTINE_TYPE , SQL_PATH from information_schema.routines where routine_type like('PROCEDURE') and routine_name like('sp_%');";
        return _query.free.Query<StoredProcedure>(sql).ToArray();
    }
}
