using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Pilgrims.PersonalFinances.Core.Utilities.SerializeDifference;

namespace Pilgrims.PersonalFinances.Core
{
    public static class GlobalExtensions
    {
        public static string StrimLineObjectName(this string name)
        {
            var _name = string.Empty;
            return name.Where(chr => char.IsLetterOrDigit(chr) || chr == ' ' || chr == '_')
                .Aggregate(_name, (current, chr) => current + chr);
        }
        public static void RunOnBackground(this Action action)
        {
            GlobalFunctions.RunOnBackground(action);
        }
        public static void RunOnBackground(this Action action, out System.ComponentModel.BackgroundWorker worker)
        {
            GlobalFunctions.RunOnBackground(out worker, action);
        }

        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("C2");
        }

        public static List<PropertyCompareResult> GetDifferenece<T>(this T oldObject, T newObject) where T : BaseEntity
        {
            var differences = GetDifference(oldObject, newObject);
            return differences;
        }       

        public static string ConvertNumberToString(this long n)
        {
            return NumberToWordsConverter.ConvertNumberToString(n);
        }

        public static string ConvertAmountToString(this object n)
        {
            return CurrencyToWordsConverter.ConvertToWords(n);
        }

        public static Exception InnerError(this Exception error)
        {
            return ExtractInnerException(error);
        }

        public static string GetNextId<T>() where T : BaseEntity
        {
            return CurrentServiceProvider?.GetService<IIdGenerator>()?.GetNextId<T>() ?? string.Empty;
        }

        public static string GetNextId(Type type)
        {
            return CurrentServiceProvider?.GetService<IIdGenerator>()?.GetNextId(type) ?? string.Empty;
        }


        public static string MakeSingular(this string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            return name.Singularize().Replace("Datum", "Data");
        }

        public static decimal GetNumber(this string str)
        {
            if (str == null)
                return 0;

            if (decimal.TryParse(new string([.. str.Where(char.IsDigit)]), out decimal ans))
                return ans;
            return 0;
        }

        public static decimal[] GetNumbers(this string str)
        {
            if (str == null)
                return [];

            return [.. Regex.Matches(str, @"[0-9]+([0-9]+)*").OfType<Match>().Select(x => decimal.TryParse(x.Value, out decimal obj) ? obj : 0m)];
        }

        public static string CreateMd5Hash(this string password)
        {
            byte[] retVal = MD5.HashData(Encoding.Unicode.GetBytes(password));
            var sb = new StringBuilder();

            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));

            return sb.ToString();
        }
        public static string CreateSHA512Hash(this string password)
        {
            var message = Encoding.UTF8.GetBytes(password);
            string hex = "";

            var hashValue = SHA512.HashData(message);
            foreach (byte x in hashValue)
                hex += string.Format("{0:x2}", x);
            return hex;
        }

        public static void InitApp(IServiceCollection serviceCollection)
        {
            AppMessagingService = serviceCollection.BuildServiceProvider().GetService<IMessagingService>() ?? throw new InvalidOperationException("IMessagingService not registered in DI container.");
            CurrentServiceCollection = serviceCollection;
            CurrentServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static string? GetModelName(this string? name)
        {
            name = name?.Replace(" ", "");
            var newName = name?.ToLower()?.Trim();

            newName = newName?.EndsWith("Entity") ?? false
                ? name?.Trim()[..^"Entity".Length]
                : StrimLineObjectName(name ?? string.Empty);

            return string.IsNullOrWhiteSpace(newName) ? "Entity" : newName;
        }
    }
}
