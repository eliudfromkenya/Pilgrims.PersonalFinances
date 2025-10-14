using SQLite;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

public interface ISqliteWriteService
{
    Task<int> InsertAsync<T>(T entity) where T : class, new();
    Task<int> UpdateAsync<T>(T entity) where T : class, new();
    Task<int> DeleteAsync<T>(T entity) where T : class, new();
    Task<int> DeleteByIdAsync<T>(string id) where T : class, new();
    Task<int> DeleteManyByIdsAsync<T>(IEnumerable<string> ids) where T : class, new();
}