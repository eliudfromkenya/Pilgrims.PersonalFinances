using Pilgrims.PersonalFinances.Core.Utilities;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Pilgrims.PersonalFinances.Core
{
    public static class GlobalFunctions
    {
        public static string StrimLineObjectName(string name)
        {
            var _name = string.Empty;

            return name.Where(chr => char.IsLetterOrDigit(chr) || chr == ' ' || chr == '_')
                .Aggregate(_name, (current, chr) => current + chr);
        }

        public static void RunOnBackground(Action action)
        {
            RunOnBackground(out BackgroundWorker worker, action);
        }

        public static void RunOnBackground(out BackgroundWorker worker, Action action)
        {
            var helper = new BackgroundWorkHelper();
            worker = helper.BackgroundWorker;
            lock (ApplicationName)
            {
                try
                {
                    var action1 = action;

                    action = () =>
                    {
                        try
                        {
                            action1();
                        }
                        catch (Exception ex)
                        {
                            NotifyError("Unhandled Background Error", ex);
                        }
                    };

                    var actions = new List<Action> { action };
                    helper.SetActionsTodo(actions);
                    helper.IsParallel = true;

                    if (helper.BackgroundWorker.IsBusy)
                        helper.SetActionsTodo(actions);
                    else
                        helper.BackgroundWorker.RunWorkerAsync();
                }
                catch (Exception e)
                {
                    NotifyError("Unhandled Background Error", e);
                }
                finally
                {
                    worker?.Dispose();
                }
            }
        }

        public static string ReadManifestData<T>(string fileName) where T : class
        {
            try
            {
                var assembly = typeof(T).GetTypeInfo().Assembly;

                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(s => s.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                using var stream = assembly.GetManifestResourceStream(resourceName!) ?? throw new InvalidOperationException("Could not load the specified resource " + fileName);
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Exception? ExtractInnerException(Exception? exception)
        {
            try
            {
                if (exception == null)
                    return exception;

                static Exception getInnerError(Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return ex.InnerException;
                    }

                    return ex;
                }

                var mEx = exception;

                while (true)
                {
                    var mm = getInnerError(mEx);
                    if (mm == mEx) break;
                    mEx = mm;
                }

                return mEx;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void NotifyError(string title, Exception ex)
        {
            AppMessagingService?.ShowErrorAsync(ex, title);
        }

        public static void NotifyError(string title, string message)
        {
            NotifyError(title, new Exception(message));
        }

        public static void RunOnBackground(Action action, int sleepTime)
        {
            RunOnBackground(() =>
            {
                try
                {
                    Thread.Sleep(sleepTime);
                    action();
                }
                catch (Exception ex)
                {
                    NotifyError("Unhandled Background Error", ex);
                }
            });
        }

        private static IDbDataAdapter? GetAdapter(IDbConnection connection)
        {
            var assembly = connection.GetType().Assembly;
            var @namespace = connection.GetType().Namespace;

            // Assumes the factory is in the same namespace
            var factoryType = assembly.GetTypes()
                                .Where(x => x.Namespace == @namespace)
                                .Where(x => x.IsSubclassOf(typeof(DbProviderFactory)))
                                .Single();

            // SqlClientFactory and OleDbFactory both have an Instance field.
            var instanceFieldInfo = factoryType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
            var factory = (DbProviderFactory?)instanceFieldInfo?.GetValue(null);

            return factory?.CreateDataAdapter();
        }

        public static DataSet GetDbDataSet(IDbConnection con, string sql)
        {
            using var cmd = con.CreateCommand();
            if (con.State != ConnectionState.Open)
                con.Open();

            var ds = new DataSet();
            var adapter = GetAdapter(con) ?? throw new ArgumentNullException("Unable to create data adapter");
            IDbCommand dbCommand = con.CreateCommand();
            dbCommand.CommandText = sql;
            dbCommand.CommandType = CommandType.Text;
            adapter.SelectCommand = dbCommand;
            adapter.Fill(ds);
            return ds;
        }

        public static string GetRandomString(int length, int trial = 0)
        {
            var rnd = new Random();
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var nn = rnd.Next(0, 35);

                if (nn < 10)
                    sb.Append(nn);
                else
                {
                    const int CON = (byte)'A' - 10;
                    sb.Append((char)(CON + nn));
                }
            }

            var ans = sb.ToString();

            if (trial++ < 100 && (ans.Contains('I') || ans.Contains('O')))
            {
                return GetRandomString(length, trial);
            }

            return ans;
        }
    }
}