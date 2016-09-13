﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Data
{
    static class DbHelper
    {
        public const string MoviesTableName = "Movies";

        public const string RatingsTableName = "Ratings";

        public const string RatingSummaryTableName = "RatingSummary";

        public const string TagsTableName = "Tags";

        static IEnumerable<string> Tables = new List<string>
        {
            MoviesTableName, RatingsTableName, RatingSummaryTableName, TagsTableName
        };

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(Properties.Settings.Default.ConnectionString);
        }

        public static int ClearTable(params string[] names)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var commandText = new StringBuilder();
                foreach (var name in names)
                {
                    if (!Tables.Contains(name)) continue;
                    commandText.AppendFormat("DELETE FROM [dbo].[{0}]", name);
                    commandText.AppendLine();
                }

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = commandText.ToString();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteFromResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = new StreamReader(assembly.GetManifestResourceStream("DiscoverHollywood.SQL." + fileName)))
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = stream.ReadToEnd();
                        return command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static IEnumerable<T> ListByResource<T>(string fileName, params object[] parameters)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = new StreamReader(assembly.GetManifestResourceStream("DiscoverHollywood.SQL." + fileName)))
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = stream.ReadToEnd();
                        FillParameters(command, parameters);
                        using (var reader = command.ExecuteReader())
                        {
                            return ConvertToList<T>(reader).ToList();
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> List<T>(string table, string where, int top, string sort, bool asc, string sort2, bool asc2, int skip, IEnumerable<object> parameters)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    var cmd = new StringBuilder();
                    cmd.AppendFormat("SELECT TOP {1} * FROM [dbo].[{0}]", table, top);
                    if (!string.IsNullOrWhiteSpace(where)) cmd.AppendFormat(" WHERE {0}", where);
                    if (!string.IsNullOrEmpty(sort))
                    {
                        cmd.AppendFormat(" ORDER BY [{0}] {1}", sort, asc ? "ASC" : "DESC");
                        if (!string.IsNullOrEmpty(sort2))
                        {
                            cmd.AppendFormat(", [{0}] {1}", sort2, asc2 ? "ASC" : "DESC");
                        }
                    }

                    command.CommandText = cmd.ToString();
                    FillParameters(command, parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        return ConvertToList<T>(reader, skip).ToList();
                    }
                }
            }
        }

        public static IEnumerable<T> List<T>(string table, string where, int top, string sort, bool asc, string sort2, bool asc2, int skip, params object[] parameters)
        {
            return List<T>(table, where, top, sort, asc, sort2, asc2, skip, parameters.ToList());
        }

        public static bool AppendParameter(string columnName, object value, string op, StringBuilder where, ICollection<object> parameters, string prefix = null, string suffix = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return false;
            where.AppendFormat("[{0}] {1} @field{2} ", columnName, op, parameters.Count);
            parameters.Add(value);
            return true;
        }

        public static bool AppendLikeParameter(string columnName, string value, bool leftPadding, bool rightPadding, StringBuilder where, ICollection<object> parameters, string opBefore)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(value)) return false;
            where.AppendFormat("{2}[{0}] LIKE @field{1} ", columnName, parameters.Count, !string.IsNullOrWhiteSpace(opBefore) ? opBefore + " " : null);
            parameters.Add(string.Format("{0}{1}{2}", leftPadding ? "%" : string.Empty, value, rightPadding ? "%" : string.Empty));
            return true;
        }

        static void FillParameters(DbCommand command, IEnumerable<object> parameters)
        {
            if (parameters == null) return;
            var index = 0;
            foreach (var parameter in parameters)
            {
                var dbp = command.CreateParameter();
                dbp.ParameterName = "@field" + index.ToString();
                dbp.Value = parameter;
                command.Parameters.Add(dbp);
                index++;
            }
        }

        static IEnumerable<T> ConvertToList<T>(DbDataReader reader, int skip = 0)
        {
            var mapping = ColumnMapping.Load(typeof(T));
            var index = -1;
            while (reader.Read())
            {
                index++;
                if (index < skip) continue;
                var obj = Activator.CreateInstance<T>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var colName = reader.GetName(i);
                    var colInfo = mapping.FirstOrDefault((item) => { return item.ColumnName == colName; });
                    if (colInfo == null || reader.IsDBNull(i)) continue;
                    var colValue = reader.GetValue(i);
                    typeof(T).GetProperty(colInfo.PropertyName).SetValue(obj, colValue);
                }

                yield return obj;
            }
        }
    }
}
