namespace Services.Dapper;

public class ConnectionInformation_DB(string _connectionString) : IConnectionInformation_DB
{
    public string ConnectionString { get; } = _connectionString;
}
