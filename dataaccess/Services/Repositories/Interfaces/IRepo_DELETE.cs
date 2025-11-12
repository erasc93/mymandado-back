using System.Data;

namespace Services.Repositories.Interfaces;

public interface IRepo_DELETE<T> { bool Delete(T item, IDbConnection? connection = null, IDbTransaction? transaction = null); }
