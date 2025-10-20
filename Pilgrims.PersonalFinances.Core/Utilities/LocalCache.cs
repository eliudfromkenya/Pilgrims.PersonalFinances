using LiteDB;
using MonkeyCache.LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pilgrims.PersonalFinances.Core.Utilities
{
    public class TableMetaData
    {
        public string Type { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string? LastAssignedValue { get; set; }
    }

    public static class LocalCache
    {
        public static LiteDatabase LiteDatabase { get => liteDatabase ??= GetDbPath(); }

        private static LiteDatabase GetDbPath()
        {
            var dir = new FileInfo(ApplicationDataPath).Directory?.FullName ??
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PPAS");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return new LiteDatabase($@"Filename={ApplicationDataPath};Password={GeneralApplicationHashKey.CreateSHA512Hash()}");
        }

        public class Poco<T>
        {
            public string Id { get; set; } = string.Empty;
            public T? Object { get; set; } = default;

            public Poco(string id, T obj)
            {
                Id = id; Object = obj;
            }

            public Poco()
            { }
        }

        static LocalCache()
        {
            lock (LiteDatabase)
            {
                try
                {
                    if (!File.Exists(ApplicationDataPath))
                    {
                        var file = new FileInfo(ApplicationDataPath);
                        var appFolder = file.Directory?.FullName!;

                        if (!Directory.Exists(appFolder))
                            Directory.CreateDirectory(appFolder);
                    }
                 }
                catch { }
            }
        }

        public static void Save<T>(string id, T obj)
        {
            if (obj is TableMetaData tf)
            {
                SaveMetadata(id, tf);
                return;
            }

            RunOnBackground(() =>
            {
                lock (LiteDatabase)
                {
                    try
                    {
                        id = id.StrimLineObjectName().ToLower();

                        lock (LiteDatabase)
                        {
                            // Open database(or create if doesn't exist)
                            //using var db = new LiteDatabase(connectionString);
                            // Get a collection (or create, if doesn't exist)
                            var col = LiteDatabase.GetCollection<Poco<T>>(GetCollectionName<T>());
                 
                            col.Upsert(new Poco<T>(id, obj));
                        }
                    }
                    catch (Exception ex)
                    {
                        NotifyError("Error saving local data", ex);
                    }
                }
            });
        }

        public static void Delete<T>(string id)
        {
            var idObj = new BsonValue(id.StrimLineObjectName().ToLower());

            lock (LiteDatabase)
            {
                // Open database(or create if doesn't exist)
                //using var db = new LiteDatabase(connectionString);
                // Get a collection (or create, if doesn't exist)
                var col = LiteDatabase.GetCollection<Poco<T>>(GetCollectionName<T>());
                col.Delete(idObj);
            }
        }

        private static void SaveMetadata(string id, TableMetaData obj)
        {
            try
            {
                lock (LiteDatabase)
                {
                    id = id.StrimLineObjectName().ToLower();
                    var oldId = Get<TableMetaData>(id)?.LastAssignedValue?.GetNumber();
                    if (obj?.LastAssignedValue?.GetNumber() <= oldId) return;

                    // Open database(or create if doesn't exist)
                    //using var db = new LiteDatabase(connectionString);
                    // Get a collection (or create, if doesn't exist)
                    var col = LiteDatabase.GetCollection<Poco<TableMetaData>>
                                 (GetCollectionName<TableMetaData>());
      
                    col.Upsert(new Poco<TableMetaData>(id, obj!));
                }
            }
            catch (Exception ex)
            {
                NotifyError("Error saving local data", ex);
            }
        }

        private static readonly SortedDictionary<string, string> collectionNames = new();
        private static LiteDatabase? liteDatabase;

        private static string GetCollectionName<T>()
        {
            lock (LiteDatabase)
            {
                var type = typeof(T).FullName!;
                if (!collectionNames.ContainsKey(type))
                    collectionNames.Add(type, $"data_{new string(typeof(T).FullName!.Where(char.IsLetter).ToArray())}");
                return collectionNames[type];
            }
        }

        public static Tuple<ILiteQueryable<Poco<T>>, LiteDatabase> Get<T>()
        {
            lock (LiteDatabase)
            {   // Open database(or create if doesn't exist)
                //var db = new LiteDatabase(connectionString);
                // Get a collection (or create, if doesn't exist)
                var col = LiteDatabase.GetCollection<Poco<T>>(GetCollectionName<T>());
                return new Tuple<ILiteQueryable<Poco<T>>, LiteDatabase>(col.Query(), LiteDatabase);
            }
        }

        public static Tuple<ILiteCollection<Poco<T>>, LiteDatabase> GetCollection<T>
            (Func<KeyValuePair<string, string>, bool> predicate)
        {
            lock (LiteDatabase)
            {
                // Open database(or create if doesn't exist)
                //using var db = new LiteDatabase(connectionString);
                // Get a collection (or create, if doesn't exist)
                return new Tuple<ILiteCollection<Poco<T>>, LiteDatabase>
                    (LiteDatabase.GetCollection<Poco<T>>(GetCollectionName<T>()), LiteDatabase);
            }
        }

        public static T? Get<T>(string id)
        {
            id = id.StrimLineObjectName().ToLower();

            lock (LiteDatabase)
            {
                // Open database(or create if doesn't exist)
                //using var db = new LiteDatabase(connectionString);

                //var dd = db.GetCollection<Poco<T>>(GetCollectionName<T>()).FindAll().ToList();
                // Get a collection (or create, if doesn't exist)
                var obj = LiteDatabase.GetCollection<Poco<T>>(GetCollectionName<T>()).FindOne(x => x.Id == id);
                return obj == null ? default : obj.Object;
            }
        }

        public static async Task<T> GetTemporaryAsync<T>(string name, Func<Task<T>> getData, TimeSpan time = default, bool forceRefresh = false)
        {
            try
            {
                lock (LiteDatabase)
                {
                    if (!forceRefresh && !Barrel.Current.IsExpired(name))
                        return Barrel.Current.Get<T>(name);
                }

                T result = await getData();
                Barrel.Current.Add(name, result, time);
                return result;
            }
            catch (Exception)
            {
                lock (LiteDatabase)
                {
                    try
                    {
                        return Barrel.Current.Get<T>(name);
                    }
                    catch { }
                }
                throw;
            }
        }
    }
}