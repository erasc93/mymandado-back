namespace Services.Dapper;

public interface IConnectionInformation_DBSafe
{
    public string ConnectionString { get; }
}


public interface IConnectionInformation_DB
{
    public string ConnectionString { get; }


}
public class TConnectionInformation_DB(string connectionString) : IConnectionInformation_DB
{
    public string ConnectionString { get; } = connectionString;
}
