using SQLite;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services;

public class SqliteConnectionProvider : ISqliteConnectionProvider
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection? _connection;

    public SqliteConnectionProvider(string dbPath)
    {
        _dbPath = dbPath;
    }

    public SQLiteAsyncConnection GetConnection()
    {
        _connection ??= new SQLiteAsyncConnection(_dbPath);
        return _connection;
    }
}