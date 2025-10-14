using Humanizer;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Utilities;
using System.Text.RegularExpressions;

namespace Pilgrims.PersonalFinances.Services
{


    internal class IdGenerator : IIdGenerator
    {
        private static SortedDictionary<string, string> IdNames { get; } = [];
        private static readonly SortedDictionary<string, string> _primaryKeysSqls = [];
        private static readonly SortedDictionary<string, string> _lastAssignedPrimary = [];
        private static readonly SortedDictionary<string, string> _idsReg = [];
        private static bool _hasRefreshedKeys = false;
        private static readonly object _idLock = new();

        public IdGenerator()
        {
            if (!_hasRefreshedKeys && SystemLoggedIn.Value)
            {
                RunOnBackground(async () => await RefreshKeysAsync());
            }

            if (string.IsNullOrWhiteSpace(CurrentDeviceNumber))
            {
                CurrentDeviceNumber = LocalCache.Get<string>("DeviceNumber") ?? "AAA";
            }
        }

        public async Task<bool?> RefreshKeysAsync(bool forceRefresh = false)
        {
            try
            {
                lock (_lastAssignedPrimary)
                {
                    if (_hasRefreshedKeys && !forceRefresh)
                        return null;
                }

                _hasRefreshedKeys = true;
                List<TableMetaData?>? existings = null;
                try
                {
                    existings = LocalCache.Get<TableMetaData>()?.Item1?.Select(c => c.Object)?.ToList() ?? [];
                }
                catch { throw; }
                existings ??= [];

                try
                {
                    if (!string.IsNullOrWhiteSpace(CurrentDeviceNumber))
                        LocalCache.Save("LastUsedPrefix", CurrentDeviceNumber);
                }
                catch { }

                var objs = await new IdRefresher().RefreshKeys(CurrentDeviceNumber).ConfigureAwait(false);

                objs = objs.Select(b =>
                {
                    var nm = b.Type.Pluralize().ToLower();
                    b.Type = b.TableName;
                    b.TableName = nm;
                    return b;
                }).ToArray();

                foreach (var (obj, name) in from obj in objs
                                            let name = obj.Type.GetModelName()?.StrimLineObjectName().Pluralize().Replace(" ", "").ToLower()
                                            select (obj, name))
                {
                    // name = obj.Type.ToLower();
                    if (IdNames.ContainsKey(name))
                        IdNames[name] = obj.TableName;
                    else
                        IdNames.Add(name, obj.TableName);

                    if ((obj.LastAssignedValue ?? string.Empty).Length < 1)
                        continue;


                    //var mm = obj.TableName?.Replace("tbl_", "")?.Replace("sys_", "")?.Replace("_", "");
                    var olds = existings
                      .Where(c => c.TableName == obj.TableName
                        && (c.LastAssignedValue?.StartsWith(CurrentDeviceNumber) ?? false));

                    var oldValue = olds?.FirstOrDefault();

                    string? lastVal = (oldValue?.LastAssignedValue?.Length == obj.LastAssignedValue?.Length) ?
                       new string?[] { oldValue?.LastAssignedValue, obj.LastAssignedValue }.Max() :
                       (oldValue?.LastAssignedValue?.Length > obj.LastAssignedValue?.Length ? oldValue?.LastAssignedValue : obj.LastAssignedValue);

                    if (oldValue == null || (lastVal != oldValue?.LastAssignedValue))
                        obj.LastAssignedValue = lastVal;

                    _lastAssignedPrimary[name] = GetNewId(obj?.LastAssignedValue);
                    LocalCache.Save(name, obj);
                }
                LocalCache.Save("DeviceNumber", CurrentDeviceNumber);
                return true;
            }
            catch (Exception ex)
            {
                NotifyError("Refreshing keys Error", $"Refreshing keys Error\r\n{ex.Message}");
                return false;
            }
        }

        public string GetNextId<T>() where T : BaseEntity => GetNextId(typeof(T));

        public string GetNextId(Type type)
        {
            var tt = type.Name.GetModelName();
            if (!type.IsAssignableTo(typeof(BaseEntity)))
                throw new InvalidCastException("Only models inheriting from 'IBaseModel' are allowed in the database context");

            if (type.IsInterface && tt?.StartsWith('I') == true)
            {
                tt = tt[1..];
                if (tt.EndsWith("Model"))
                    tt = tt[0..^5];
            }
            return GetNextId(tt);
        }

        public bool SaveNewId<T>(string id) => SaveNewId(id, typeof(T).Name.GetModelName()!);

        public string GetNextId(string? table)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new NullReferenceException("Unable to get table name");

            if (string.IsNullOrEmpty(CurrentDeviceNumber))
            {
                try
                {
                    CurrentDeviceNumber = CurrentDeviceNumber;
                    if (string.IsNullOrEmpty(CurrentDeviceNumber))
                        CurrentDeviceNumber = LocalCache.Get<string>("LastUsedPrefix");
                }
                catch { }
            }

            var key = TryNextId(table);
            if (!string.IsNullOrEmpty(CurrentDeviceNumber) && (!key.StartsWith(CurrentDeviceNumber)))
                key = CurrentDeviceNumber + "-01";

            if (!string.IsNullOrEmpty(key))
            {
                if (_idsReg.ContainsKey(table))
                {
                    if (_idsReg[table] == key)
                        key = GetNewId(key);
                    _idsReg[table] = key;
                }
                else
                    _idsReg.Add(table, key);
            }

            SaveNewId(key, table);
            return key;
        }

        public object RevertBack(string id)
        {
            var nos = id.GetNumbers();
            if (nos.Any())
            {
                var last = nos.Last();
                var nn = last.ToString();
                var index = id.LastIndexOf(nn);
                var index2 = id.LastIndexOf(nn) + nn.Length;
                var ans = $"{id.Substring(0, index)}{last - 1}";
                if (index2 < id.Length)
                    ans = $"{ans}{id.Substring(index2)}";
                return ans;
            }
            else return null;
        }

        private bool SaveNewId(string key, string table)
        {
            try
            {
                table = table.StrimLineObjectName().Pluralize().Replace(" ", "").ToLower();
                if (_lastAssignedPrimary.ContainsKey(table))
                    _lastAssignedPrimary[table] = key;
                else
                    _lastAssignedPrimary.Add(table, key);

                LocalCache.Save(table, new TableMetaData
                {
                    LastAssignedValue = key,
                    TableName = table
                });
                return true;
            }
            catch (Exception ex)
            {
                NotifyError("Saving New Id Error", $"Saving New Id Error\r\n{ex.Message}");
            }
            return false;
        }

        public string TryNextId<T>() where T : BaseEntity => TryNextId(typeof(T));

        public string TryNextId(Type type)
        {
            var tt = type.Name.GetModelName();
            if (!type.IsAssignableTo(typeof(BaseEntity)))
                throw new InvalidCastException("Only models inheriting from 'IBaseModel' are allowed in the database context");

            if ((type.IsInterface) && tt.StartsWith('I'))
            {
                tt = tt[1..];
                if (tt.EndsWith("Model"))
                    tt = tt[0..^5];
            }
            return TryNextId(tt);
        }

        public string TryNextId(string table)
        {
            lock (_idLock)
            {
                if (string.IsNullOrWhiteSpace(table))
                    throw new NullReferenceException("Unable to get table name");

                table = table.StrimLineObjectName().Replace(" ", "").Pluralize().ToLower();
                try
                {
                    if (_lastAssignedPrimary.TryGetValue(table, out var value2))
                        return GetNewId(value2);

                    var id = LocalCache.Get<TableMetaData>(table)?.LastAssignedValue;

                    try
                    {
                        if (id != null && !id.StartsWith(CurrentDeviceNumber))
                        {
                            if (IdNames.TryGetValue(table, out var value))
                            {
                                var tblName = value;
                                if (!_primaryKeysSqls.TryGetValue(tblName, out var sql))
                                {
                                    // lazily populate SQLs for primary keys based on current prefix
                                    var q = new IdRefresher().GetQuerySqls(CurrentDeviceNumber).GetAwaiter().GetResult();
                                    foreach (var kv in q)
                                        _primaryKeysSqls[kv.Key] = kv.Value;
                                    _primaryKeysSqls.TryGetValue(tblName, out sql);
                                }
                                if (!string.IsNullOrEmpty(sql))
                                {
                                    using var ds = GetDbDataSet(GetDbConnection(), sql);
                                    var row = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
                                    id = row == null ? $"{CurrentDeviceNumber}-01" : (row["CKey"].ToString() ?? $"{CurrentDeviceNumber}-01");
                                }
                            }
                        }
                        _lastAssignedPrimary[table] = id ?? $"{CurrentDeviceNumber}-01";
                    }
                    catch { }
                    return id == null ? $"{CurrentDeviceNumber}-01" : GetNewId(id);
                }
                catch (Exception ex)
                {
                    NotifyError("Getting next id error", $"Getting next id error\r\n{ex.Message}");
                    return $"{CurrentDeviceNumber}-01";
                }
            }
            // Fallback should never hit due to returns in branches, but keep safe
            return $"{CurrentDeviceNumber}-01";
        }

        /// <summary>
        /// The ReplaceFirstOccurrence.
        /// </summary>
        /// <param name="Source">The Source<see cref="string"/>.</param>
        /// <param name="Find">The Find<see cref="string"/>.</param>
        /// <param name="Replace">The Replace<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            try
            {
                int Place = Source.IndexOf(Find);
                var result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
                return result;
            }
            catch (Exception)
            {
                return Source;
            }
        }

        /// <summary>
        /// The ReplaceLastOccurrence.
        /// </summary>
        /// <param name="Source">The Source<see cref="string"/>.</param>
        /// <param name="Find">The Find<see cref="string"/>.</param>
        /// <param name="Replace">The Replace<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            try
            {
                int Place = Source.LastIndexOf(Find);
                var result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
                return result;
            }
            catch (Exception)
            {
                return Source;
            }
        }

        public int GetLastNumber(string input) =>
            Regex.Matches(input ?? string.Empty, @"\d+").OfType<Match>()
            .Select(m => int.Parse(m.Value)).LastOrDefault();

        /// <summary>
        /// The GetNewId.
        /// </summary>
        /// <param name="tableName">The tableName<see cref="string"/>.</param>
        /// <param name="lastId">The lastId<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetNextNumber(string lastId)
        {
            var nss = lastId?.GetNumbers();
            var num = lastId?.GetNumbers()?.LastOrDefault();
            if (num != null)
            {
                var nextNum = num + 1;
                string origNum = num.ToString(), newNum = nextNum.ToString();

                var isLonger = origNum.Length < newNum.Length;
                int place = lastId.LastIndexOf(origNum);
                if (isLonger && lastId?.Length > origNum.Length && lastId[place - 1] == '0')
                    return lastId.Remove(place - 1, origNum.Length).Insert(place, newNum);
                else
                    return lastId.Remove(place, origNum.Length).Insert(place, newNum);
            }
            return lastId;
        }

        /// <summary>
        /// The GetNewId.
        /// </summary>
        /// <param name="tableName">The tableName<see cref="string"/>.</param>
        /// <param name="lastId">The lastId<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetNewId(string? lastId)
        {
            var id = string.Empty;
            try
            {
                var dn = CurrentDeviceNumber;
                if (string.IsNullOrWhiteSpace(dn))
                    dn = LocalCache.Get<string>("DeviceNumber") ?? "AAA";
                // normalize to 3 characters
                dn = dn.PadRight(3, 'A');
                CurrentDeviceNumber = dn[..3];

                var lastIdObjs = lastId?.Split('-');

                if (string.IsNullOrWhiteSpace(lastId) || lastId.Trim().Length < 1)
                    id = $"{CurrentDeviceNumber}-01";
                else
                {
                    var input = lastId;
                    var number = GetLastNumber(input);
                    id = number.ToString() == null ?
                        $"{CurrentDeviceNumber}-01" :
                        ReplaceLastOccurrence(input, number.ToString("00"), (number + 1).ToString("00"));
                }
            }
            catch (Exception) { }
            return id;
        }
    }

}
