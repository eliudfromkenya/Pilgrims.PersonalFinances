using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using System.Collections.Concurrent;

namespace Pilgrims.PersonalFinances.Tests.Utilities
{
    // Minimal test implementation to provide stable ID generation for SaveChanges
    public class TestIdGenerator : IIdGenerator
    {
        private readonly ConcurrentDictionary<string, string> _lastPerTable = new();
        private readonly object _lock = new();

        public string GetNextId<T>() where T : BaseEntity => GetNextId(typeof(T));

        public string GetNextId(System.Type type)
        {
            var table = Normalize(type.Name);
            return GetNextId(table);
        }

        public string GetNextId(string? table)
        {
            if (string.IsNullOrWhiteSpace(table))
                table = "entity";

            table = Normalize(table);

            lock (_lock)
            {
                var last = _lastPerTable.GetOrAdd(table, "AAA-00");
                var next = GetNewId(last);
                _lastPerTable[table] = next;
                return next;
            }
        }

        public bool SaveNewId<T>(string id) => true;

        public string TryNextId<T>() where T : BaseEntity => GetNextId<T>();

        public string TryNextId(System.Type type) => GetNextId(type);

        public string TryNextId(string table) => GetNextId(table);

        public Task<bool?> RefreshKeysAsync(bool forceRefresh = false) => Task.FromResult<bool?>(true);

        public int GetLastNumber(string input)
        {
            // Extract last number group; if none, return 0
            var nums = System.Text.RegularExpressions.Regex.Matches(input ?? string.Empty, "\\d+")
                .OfType<System.Text.RegularExpressions.Match>()
                .Select(m => int.Parse(m.Value))
                .ToList();
            return nums.Count == 0 ? 0 : nums[^1];
        }

        public string GetNextNumber(string lastId)
        {
            var number = GetLastNumber(lastId);
            var orig = number.ToString("00");
            var next = (number + 1).ToString("00");
            var idx = lastId.LastIndexOf(orig);
            if (idx < 0) return lastId + next; // fallback
            return lastId.Remove(idx, orig.Length).Insert(idx, next);
        }

        public string GetNewId(string? lastId)
        {
            if (string.IsNullOrWhiteSpace(lastId)) return "AAA-01";

            // Keep prefix, increment last 2-digit segment
            var prefix = lastId.Split('-')[0];
            var next = GetNextNumber(lastId);
            // Ensure we preserve prefix and dash
            var dashIdx = lastId.IndexOf('-');
            if (dashIdx >= 0)
                return prefix + "-" + next.Split('-').Last();
            return prefix + "-" + next;
        }

        public object RevertBack(string id)
        {
            var number = GetLastNumber(id);
            var orig = number.ToString("00");
            var prev = (number - 1).ToString("00");
            var idx = id.LastIndexOf(orig);
            if (idx < 0) return id;
            return id.Remove(idx, orig.Length).Insert(idx, prev);
        }

        private static string Normalize(string name)
        {
            // Lowercase pluralized table-like key
            name = name.Trim();
            if (name.StartsWith("I") && name.Length > 1) name = name[1..];
            if (!name.EndsWith("s")) name += "s";
            return name.ToLowerInvariant();
        }
    }
}