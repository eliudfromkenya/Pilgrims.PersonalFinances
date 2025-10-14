using LiteDB;
using Pilgrims.PersonalFinances.Core.Utilities;
using Pilgrims.PersonalFinances.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pilgrims.PersonalFinances.Services
{
    internal class IdRefresher
    {
        public async Task<Dictionary<string, string>> GetQuerySqls(string prefix)
        {
            try
            {
                using var db = CurrentServiceProvider!.GetService<PersonalFinanceContext>();
                var ans = await Task.FromResult(db.Model.GetEntityTypes()
                     .Select(x =>
                     {
                         var tableName = x.GetTableName();
                         var pri = x.FindPrimaryKey().Properties.Select(x => x.GetColumnName(Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier.Table(tableName, null))).First();
                         var attribs = new { Table = x.GetTableName(), Key = pri, Type = x.ClrType.Name };
                         return attribs;
                     }).ToDictionary(y => y.Table, x =>
                      string.Format(@"SELECT '{1}' as Name, '{3}' as type, (SELECT MAX({0}) FROM {1}
                                WHERE {0} LIKE '%{2}%' AND LENGTH({0}) = (SELECT MAX(LENGTH({0})) FROM {1} WHERE {0} LIKE '%{2}%')) as CKey",
                                     x.Key, x.Table, prefix, x.Type)));
                return ans;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TableMetaData[]> RefreshKeys(string prefix)
        {
            try
            {

                using var db = CurrentServiceProvider!.GetService<PersonalFinanceContext>();
                var sql = string.Join("\r\nUNION DISTINCT ", db!.Model.GetEntityTypes()
                    .Select(x =>
                    {
                        var tableName = x.GetTableName();
                        var pri = x.FindPrimaryKey().Properties.Select(x => x.GetColumnName(Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier.Table(tableName, null))).First();
                        var attribs = new { Table = x.GetTableName(), Key = pri, Type = x.ClrType.Name };
                        return attribs;
                    })
                    .Select(x =>
                     string.Format(@"SELECT '{1}' as Name, '{3}' as type, (SELECT MAX({0}) FROM {1}
                                WHERE {0} LIKE '%{2}%' AND LENGTH({0}) = (SELECT MAX(LENGTH({0})) FROM {1} WHERE {0} LIKE '%{2}%')) as CKey",
                                    x.Key, x.Table, prefix, x.Type)));

                using var con = GetDbConnection();
                if (con.State != ConnectionState.Open) con.Open();

                var tables = new List<TableMetaData>();

                var logger = CurrentServiceProvider!.GetService<Serilog.ILogger>();
                logger?.Information(sql);
                try
                {
                    using var cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    lock (con)
                    {
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            //var value = reader.GetValue(2).ToString();
                            var tblName = reader.GetValue(0).ToString();
                            var type = reader.GetValue(1).ToString();
                            var value = reader.GetValue(2).ToString();
                            //if (!string.IsNullOrWhiteSpace(value))
                            {
                                var obj = new TableMetaData
                                {
                                    TableName = tblName,
                                    Type = type,
                                    LastAssignedValue = value
                                };
                                tables.Add(obj);
                            }
                        }
                    }
                }
                catch { }
                return await Task.FromResult(tables.ToArray());
            }
            catch (Exception ex)
            {
                throw ex.InnerError();
            }
        }
    }
}
