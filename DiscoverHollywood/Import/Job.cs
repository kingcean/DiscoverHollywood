using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Import
{
    public static class Job
    {
        /// <summary>
        /// Gets or sets a value indicating whether disable tags.
        /// </summary>
        public static bool DisableTags { get; set; } = false;

        public static void CreateDbTables()
        {
            var rec = DateTime.Now;
            Data.DbHelper.ExecuteFromResource("Init.sql");
            ConsoleUtils.Log(ref rec, "creating tables");
        }

        /// <summary>
        /// Clears database.
        /// </summary>
        public static void Clear()
        {
            var rec = DateTime.Now;
            Data.DbHelper.ClearTable(Data.DbHelper.MoviesTableName, Data.DbHelper.RatingSummaryTableName, Data.DbHelper.MovieGenresTableName);
            ConsoleUtils.Log(ref rec, "clearing tables");
            if (!DisableTags) Data.DbHelper.ClearTable(Data.DbHelper.TagsTableName);
            ConsoleUtils.Log(ref rec, "clearing tags table");
        }

        public static void Init()
        {
            Clear();
            var reader = new MovieLensFileLoader();
            reader.LoadLinks = true;
            var rec = DateTime.Now;

            FileToDb(reader.Load, Data.DbHelper.MoviesTableName, "loading basic information of movie", ref rec);
            FileToDb(reader.LoadGenres, Data.DbHelper.MovieGenresTableName, "loading genres for movie", ref rec);
            FileToDb(reader.LoadRatings, Data.DbHelper.RatingSummaryTableName, "loading rating summary", ref rec);

            Data.DbHelper.ExecuteFromResource("ApplyRatings.sql");
            ConsoleUtils.Log(ref rec, "updating ratings to movies");

            if (DisableTags) return;
            FileToDb(reader.LoadTags, Data.DbHelper.TagsTableName, "loading tags", ref rec);
        }

        public static void FileToDb<T>(Func<IEnumerable<T>> loader, string tableName, string message, ref DateTime rec)
        {
            var list = loader();
            var copy = new Data.BatchCopy<T>();
            copy.TableName = tableName;
            copy.Process(list);
            ConsoleUtils.Log(ref rec, message);
        }
    }
}
