using SQLite;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

public interface ISqliteConnectionProvider
{
    SQLiteAsyncConnection GetConnection();
}