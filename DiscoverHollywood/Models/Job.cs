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
            var where = new StringBuilder();
            var parameters = new List<object>();
            var hasParam = Data.DbHelper.AppendLikeParameter("name", name, true, true, where, parameters, null);
            if (year.HasValue)
            {
                where.AppendFormat("{1}[year] = {0}", year, hasParam ? "AND " : null);
            }

            Data.DbHelper.AppendLikeParameter("genres", genres, true, true, where, parameters, hasParam ? "AND" : null);
            return Data.DbHelper.List<Movie>(Data.DbHelper.MoviesTableName, where.ToString(), (pageIndex + 1) * pageSize, "rating", false, pageIndex * pageSize, parameters);
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
            return Data.DbHelper.List<Movie>(Data.DbHelper.MoviesTableName, "[id]=" + id.ToString(), 1, null, true, 0).FirstOrDefault();
        }
    }
}
