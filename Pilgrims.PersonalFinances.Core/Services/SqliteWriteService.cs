using SQLite;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services;

public class SqliteWriteService : ISqliteWriteService
{
    private readonly ISqliteConnectionProvider _provider;

    public SqliteWriteService(ISqliteConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task<int> InsertAsync<T>(T entity) where T : class, new()
    {
        var conn = _provider.GetConnection();
        return await conn.InsertAsync(entity).ConfigureAwait(false);
    }

    public async Task<int> UpdateAsync<T>(T entity) where T : class, new()
    {
        var conn = _provider.GetConnection();
        return await conn.UpdateAsync(entity).ConfigureAwait(false);
    }

    public async Task<int> DeleteAsync<T>(T entity) where T : class, new()
    {
        var conn = _provider.GetConnection();
        return await conn.DeleteAsync(entity).ConfigureAwait(false);
    }

    public async Task<int> DeleteByIdAsync<T>(string id) where T : class, new()
    {
        var conn = _provider.GetConnection();
        // Use direct SQL to avoid attribute requirements
        var table = typeof(T).Name;
        var sql = $"DELETE FROM \"{table}\" WHERE \"Id\" = ?";
        return await conn.ExecuteAsync(sql, id).ConfigureAwait(false);
    }

    public async Task<int> DeleteManyByIdsAsync<T>(IEnumerable<string> ids) where T : class, new()
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return 0;
        var conn = _provider.GetConnection();
        var table = typeof(T).Name;
        var placeholders = string.Join(",", Enumerable.Repeat("?", idList.Count));
        var sql = $"DELETE FROM \"{table}\" WHERE \"Id\" IN ({placeholders})";
        return await conn.ExecuteAsync(sql, idList.ToArray()).ConfigureAwait(false);
    }
}