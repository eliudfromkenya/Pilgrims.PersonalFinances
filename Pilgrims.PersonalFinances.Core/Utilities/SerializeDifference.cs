using Pilgrims.PersonalFinances.Core.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Pilgrims.PersonalFinances.Core.Utilities;

public static class SerializeDifference
{
    public class PropertyCompareResult(string? name, object? oldValue, object? newValue)
    {
        public string? Name { get; private set; } = name;
        public object? OldValue { get; private set; } = oldValue;
        public object? NewValue { get; private set; } = newValue;
    }

    private static List<PropertyCompareResult> Compare<T>(T oldObject, T newObject)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();
        List<PropertyCompareResult> result = [];

        foreach (PropertyInfo pi in properties)
        {
            if (pi.Name == "DateUpdated" || pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(JsonIgnoreAttribute)))
            {
                continue;
            }

            object? oldValue = pi.GetValue(oldObject), newValue = pi?.GetValue(newObject);

            if (!object.Equals(oldValue, newValue))
            {
                result.Add(new PropertyCompareResult(pi?.Name, oldValue, newValue));
            }
        }

        return result;
    }

    internal static List<PropertyCompareResult> GetDifference(BaseEntity model1, BaseEntity model2)
    {
        return Compare(model1, model2);
    }
}
