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
    }
}
