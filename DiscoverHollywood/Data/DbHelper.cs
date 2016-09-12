using System;
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
            MoviesTableName, RatingsTableName, TagsTableName
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
                            return ConvertToList<T>(reader);
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> List<T>(string table, string where, int top, string sort, bool asc, int skip, IEnumerable<object> parameters)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    var cmd = new StringBuilder();
                    cmd.AppendFormat("SELECT TOP {1} * FROM [dbo].[{0}]", table, top);
                    if (!string.IsNullOrWhiteSpace(where)) cmd.AppendFormat(" WHERE {0}", where);
                    if (!string.IsNullOrEmpty(sort)) cmd.AppendFormat(" ORDERBY [{0}] {1}", sort, asc ? "ASC" : "DESC");
                    command.CommandText = cmd.ToString();
                    FillParameters(command, parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        return ConvertToList<T>(reader, skip);
                    }
                }
            }
        }

        public static IEnumerable<T> List<T>(string table, string where, int top, string sort, bool asc, int skip, params object[] parameters)
        {
            return List<T>(table, where, top, sort, asc, skip, parameters);
        }

        public static void AppendParameter(string columnName, object value, string op, StringBuilder where, ICollection<object> parameters, string prefix = null, string suffix = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return;
            where.AppendFormat("[{0}] {1} @field{2} ", columnName, op, parameters.Count);
            parameters.Add(value);
        }

        public static void AppendLikeParameter(string columnName, string value, bool leftPadding, bool rightPadding, StringBuilder where, ICollection<object> parameters)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(value)) return;
            where.AppendFormat("[{0}] LIKE @field{1} ", columnName, parameters.Count);
            parameters.Add(string.Format("{0}{1}{2}", leftPadding ? "%" : string.Empty, value, rightPadding ? "%" : string.Empty));
        }

        static void FillParameters(DbCommand command, IEnumerable<object> parameters)
        {
            if (parameters == null) return;
            var index = 0;
            foreach (var parameter in parameters)
            {
                var dbp = command.CreateParameter();
                dbp.ParameterName = "@field" + index.ToString();
                dbp.Value = parameters;
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
                    if (colInfo == null) continue;
                    var colValue = reader.GetValue(i);
                    typeof(T).GetProperty(colInfo.PropertyName).SetValue(obj, colValue);
                }

                yield return obj;
            }
        }
    }
}
