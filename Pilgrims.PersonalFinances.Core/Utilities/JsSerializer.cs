using Newtonsoft.Json;

namespace Pilgrims.PersonalFinances.Core.Utilities
{
    public static class JsSerializer
    {
        public static string? Serialize(object myobj)
        {
            if (myobj == null) return null;

            var returnstring = JsonConvert.SerializeObject(myobj, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return returnstring;
        }

        public static T? Deserialize<T>(string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
                return default;

            if (typeof(T).IsInterface)
            {
                Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                     .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                     .FirstOrDefault() ?? throw new Exception($"Unable to resolve {typeof(T).Name}");

                var deserializedObject = Deserialize(obj, type);
                return (T?)deserializedObject;
            }

            var returnstring = JsonConvert.DeserializeObject<T>(obj);
            return returnstring;
        }

        public static object? Deserialize(string obj, Type type)
        {
            if (string.IsNullOrWhiteSpace(obj)) return null;

            var returnstring = JsonConvert.DeserializeObject(obj, type);
            return returnstring;
        }

    }
}
    
