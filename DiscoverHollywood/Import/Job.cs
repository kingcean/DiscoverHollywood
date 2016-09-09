using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Import
{
    class Job
    {
        public static void Process()
        {
            var rec = DateTime.Now;
            var reader = new MovieLensFileLoader();
            reader.LoadLinks = true;
            Data.DbHelper.ClearTable(Data.DbHelper.MoviesTableName, Data.DbHelper.RatingSummaryTableName, Data.DbHelper.TagsTableName);
            ConsoleUtils.Log(ref rec, "preparation and clearing database");

            var movies = reader.Load();
            var movieCopy = new Data.BatchCopy<Models.Movie>();
            movieCopy.TableName = Data.DbHelper.MoviesTableName;
            movieCopy.Process(movies, Models.Movie.TableMapping());
            ConsoleUtils.Log(ref rec, "loading basic information of movie");

            var ratings = reader.LoadRatings();
            var ratingCopy = new Data.BatchCopy<Models.RatingSummary>();
            ratingCopy.TableName = Data.DbHelper.RatingSummaryTableName;
            ratingCopy.Process(ratings, Models.RatingSummary.TableMapping());
            ConsoleUtils.Log(ref rec, "loading rating summary");

            Data.DbHelper.ExecuteFromResource("ApplyRatings.sql");
            ConsoleUtils.Log(ref rec, "updating ratings to movies");

            var tags = reader.LoadTags();
            var tagCopy = new Data.BatchCopy<Models.Tag>();
            tagCopy.TableName = Data.DbHelper.TagsTableName;
            tagCopy.Process(tags, Models.Tag.TableMapping());
            ConsoleUtils.Log(ref rec, "loading tags");
        }
    }
}
