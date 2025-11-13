namespace Services.Dapper.DBWire;

public class ConnectionInformation_DB(string _connectionString) : IConnectionInformation_DB
{
    public string ConnectionString { get; } = _connectionString;
}
