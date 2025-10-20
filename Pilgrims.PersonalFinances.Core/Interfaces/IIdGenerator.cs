using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Interfaces
{
    public interface IIdGenerator
    {
        int GetLastNumber(string input);
        string GetNewId(string lastId);
        string GetNextId(string table);
        string GetNextId(Type type);
        string GetNextId<T>() where T : BaseEntity;
        string GetNextNumber(string lastId);
        Task<bool?> RefreshKeysAsync(bool forceRefresh = false);
        object RevertBack(string id);
        bool SaveNewId<T>(string id);
        string TryNextId(string table);
        string TryNextId(Type type);
        string TryNextId<T>() where T : BaseEntity;
    }
}