using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscoverHollywood.Data
{
    class BatchCopy<T>
    {
        public string TableName { get; set; }

        public int BatchSize { get; set; } = 1000;

        public void Process(IEnumerable<T> list)
        {
            var settings = Properties.Settings.Default;
            var mapping = ColumnMapping.Load(typeof(T));
            using (var bulk = new SqlBulkCopy(settings.ConnectionString))
            {
                bulk.BatchSize = BatchSize;
                bulk.DestinationTableName = TableName;
                using (var reader = new ListDataReader<T>(list))
                {
                    reader.Mapping.AddRange(mapping);
                    bulk.WriteToServer(reader);
                }
            }
        }

        public Task ProcessAsync(IEnumerable<Models.Movie> list, CancellationToken cancellationToken)
        {
            var settings = Properties.Settings.Default;
            using (var bulk = new SqlBulkCopy(settings.ConnectionString))
            {
                bulk.BatchSize = BatchSize;
                bulk.DestinationTableName = "Movie";
                using (var reader = new ListDataReader<Models.Movie>(list))
                {
                    return bulk.WriteToServerAsync(reader, cancellationToken);
                }
            }
        }
        public Task ProcessAsync(IEnumerable<Models.Movie> list)
        {
            return ProcessAsync(list, CancellationToken.None);
        }
    }
}
