using System.Data;

namespace Services.Dapper.Interfaces;

public interface ICRUDAsync
{
    public Task<T[]> GetAll<T>() where T : class;
    public Task<T?> GetById<T>(int id) where T : class;
    public Task<int?> Add<T>(T entityToInsert) where T : class;
    public Task<bool> Update<T>(T entityToUpdate) where T : class;
    public Task<bool> Delete<T>(T entityToUpdate) where T : class;
}
