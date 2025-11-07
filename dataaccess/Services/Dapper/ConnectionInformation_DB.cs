namespace Services.Dapper;

public class ConnectionInformation_DB: IConnectionInformation_DB
{
    public string ConnectionString { get; private set; }
    public ConnectionInformation_DB(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
