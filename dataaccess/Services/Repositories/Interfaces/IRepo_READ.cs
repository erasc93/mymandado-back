using System.Data;

namespace Services.Repositories.Interfaces;

public interface IRepo_READ<T>
{
    T? GetUserByName(string name,IDbConnection? connection,IDbTransaction? transaction);
    T[] GetAll(IDbConnection? connection,IDbTransaction? transaction);
}
