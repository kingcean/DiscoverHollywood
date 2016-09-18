using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Models
{
    public static class Job
    {
        public static IEnumerable<Movie> Movies(string name = null, int? year = null, string genres = null, int pageIndex = 0, int pageSize = 20)
        {
            var search = new Data.ExecuteQuery<Movie>(Data.DbHelper.MoviesTableName);
            var where = search.Where;
            var parameters = search.Parameters;
            var hasParam = false;
            if (year.HasValue)
            {
                where.AppendFormat("[year] = {0}", year);
                hasParam = true;
            }

            if (!string.IsNullOrWhiteSpace(genres))
            {
                where.AppendFormat(" {1}[id] in (SELECT [movie] FROM [dbo].[{0}] WHERE [genre] = @field0)", Data.DbHelper.MovieGenresTableName, hasParam ? "AND " : null);
                parameters.Add(genres.Trim());
                hasParam = true;
            }

            if (Data.DbHelper.AppendLikeParameter("name", name, true, true, where, parameters, hasParam ? " AND" : null)) hasParam = true;
            search.Sort1 = "rating";
            search.Sort2 = "ratingCount";
            search.Sort3 = "year";
            return search.Process((pageIndex + 1) * pageSize, pageIndex * pageSize);
        }

        public static IEnumerable<Movie> ListMovieByCommandLine(string cmd)
        {
            const int pageSize = 50;
            var nameEndAt = !string.IsNullOrWhiteSpace(cmd) ? (cmd.IndexOf("-") == 0 ? 0 : cmd.IndexOf(" -")) : -1;
            if (nameEndAt < 0)
            {
                return Movies(cmd, null, null, 0, pageSize);
            }

            var name = cmd.Substring(0, nameEndAt);
            var rest = cmd.Substring(nameEndAt).Split(new[] { " -" }, StringSplitOptions.RemoveEmptyEntries);
            var year = -1;
            string genres = null;
            foreach (var item in rest)
            {
                var keyPos = item.IndexOf(' ');
                if (keyPos < 0) continue;
                var key = item.Substring(0, item.IndexOf(' ')).Replace("-", string.Empty).ToUpper();
                var value = item.Substring(keyPos + 1);
                switch (key)
                {
                    case "YEAR":
                    case "Y":
                        int.TryParse(value, out year);
                        break;
                    case "GENRES":
                    case "GENRE":
                    case "G":
                        genres = value;
                        break;
                }
            }

            int? yearValue = null;
            if (year > 0) yearValue = year;
            return Movies(name, yearValue, genres, 0, pageSize);
        }

        public static Movie Movie(int id)
        {
            var search = new Data.ExecuteQuery<Movie>(Data.DbHelper.MoviesTableName);
            search.Where.AppendFormat("[id] = {0}", id);
            return search.Process(1).FirstOrDefault();
        }

        public static IEnumerable<RatingSummary> MovieRatings(int id, int pageIndex = 0, int pageSize = 20)
        {
            var search = new Data.ExecuteQuery<RatingSummary>(Data.DbHelper.RatingSummaryTableName);
            search.Where.AppendFormat("[movie] = {0}", id);
            search.Sort1 = "year";
            return search.Process((pageIndex + 1) * pageSize, pageIndex * pageSize);
        }

        public static IEnumerable<Tag> MovieTags(int id, int pageIndex = 0, int pageSize = 20)
        {
            var search = new Data.ExecuteQuery<Tag>(Data.DbHelper.TagsTableName);
            search.Where.AppendFormat("[movie] = {0}", id);
            search.Sort1 = "created";
            return search.Process((pageIndex + 1) * pageSize, pageIndex * pageSize);
        }
    }
}
