using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Import
{
    /// <summary>
    /// The MovieLens csv files loader.
    /// </summary>
    public class MovieLensFileLoader
    {
        /// <summary>
        /// Gets or sets a value indicating whether need load references information.
        /// </summary>
        public bool LoadLinks { get; set; }

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        public string DirPath { get; set; } = "data";

        /// <summary>
        /// Loads the list from file.
        /// </summary>
        /// <returns>A collection of movie loaded.</returns>
        public IEnumerable<Models.Movie> Load()
        {
            IEnumerator<string> linkEnum = null;
            var loadLinks = LoadLinks;
            if (loadLinks)
            {
                var links = ReadLines("links.csv");
                if (links != null) linkEnum = links.GetEnumerator();
            }

            foreach (var line in ReadLines("movies.csv"))
            {
                var model = new Models.Movie();
                var fields = Data.CsvParser.Instance.ParseLine(line);
                if (fields.Count < 2)
                {
                    if (loadLinks) loadLinks = linkEnum.MoveNext();
                    continue;
                }

                int modelId;
                if (!int.TryParse(fields[0], out modelId))
                {
                    if (loadLinks) loadLinks = linkEnum.MoveNext();
                    continue;
                }

                model.Id = modelId;
                model.Name = fields[1];
                if (model.Name.Length > 6 && model.Name[model.Name.Length - 6] == '(' && model.Name[model.Name.Length - 1] == ')')
                {
                    int year;
                    if (int.TryParse(model.Name.Substring(model.Name.Length - 5, 4), out year))
                    {
                        model.Name = model.Name.Substring(0, model.Name.Length - 6);
                        model.Year = year;
                    }
                }

                if (fields.Count == 3) model.GenresStr = fields[2].Replace("|", ";");
                if (!loadLinks)
                {
                    yield return model;
                    continue;
                }

                loadLinks = linkEnum.MoveNext();
                var linkStr = linkEnum.Current;
                if (string.IsNullOrWhiteSpace(linkStr))
                {
                    yield return model;
                    continue;
                }

                var linkFields = Data.CsvParser.Instance.ParseLine(linkStr);
                if (linkFields.Count < 3 || !int.TryParse(linkFields[0], out modelId) || modelId != model.Id)
                {
                    yield return model;
                    continue;
                }

                if (int.TryParse(linkFields[1], out modelId)) model.ImdbId = modelId;
                if (int.TryParse(linkFields[2], out modelId)) model.TmdbId = modelId;
                yield return model;
            }
        }

        /// <summary>
        /// Loads the list from file.
        /// </summary>
        /// <returns>A collection of movie loaded.</returns>
        public IEnumerable<Models.MovieGenres> LoadGenres()
        {
            foreach (var line in ReadLines("movies.csv"))
            {
                var fields = Data.CsvParser.Instance.ParseLine(line);
                if (fields.Count < 3)
                {
                    continue;
                }

                int modelId;
                if (!int.TryParse(fields[0], out modelId))
                {
                    continue;
                }

                var GenresArr = fields[2].Trim().Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var genre in GenresArr)
                {
                    var model = new Models.MovieGenres();
                    model.MovieId = modelId;
                    model.Genre = genre;
                    yield return model;
                }
            }
        }

        public IEnumerable<Models.RatingSummary> LoadRatings()
        {
            var dict = new Dictionary<string, Models.RatingSummary>();
            foreach (var line in ReadLines("ratings.csv"))
            {
                var fields = Data.CsvParser.Instance.ParseLine(line);
                if (fields.Count < 4) continue;
                var item = new Models.Rating();
                int num;
                if (!int.TryParse(fields[0], out num)) continue;
                item.UserId = num;
                if (!int.TryParse(fields[1], out num)) continue;
                item.MovieId = num;
                float num2;
                if (!float.TryParse(fields[2], out num2)) continue;
                item.Value = num2;
                item.Created = Data.DateTimeUtils.ParseFromJava(fields[3], 1000);
                var dictId = string.Format("{0}-{1}", item.MovieId, item.Created.Year);
                if (!dict.ContainsKey(dictId)) dict[dictId] = new Models.RatingSummary();
                var record = dict[dictId];
                record.MovieId = item.MovieId;
                record.CreatedYear = item.Created.Year;
                record.Value += item.Value;
                record.Count++;
            }

            return dict.Values;
        }

        public IEnumerable<Models.Tag> LoadTags()
        {
            foreach (var line in ReadLines("tags.csv"))
            {
                var fields = Data.CsvParser.Instance.ParseLine(line);
                if (fields.Count < 4) continue;
                int num;
                if (!int.TryParse(fields[0], out num)) continue;
                var item = new Models.Tag();
                item.UserId = num;
                if (!int.TryParse(fields[1], out num)) continue;
                item.MovieId = num;
                item.Value = fields[2];
                item.Created = Data.DateTimeUtils.ParseFromJava(fields[3], 1000);
                yield return item;
            }
        }

        IEnumerable<string> ReadLines(string fileName)
        {
            return File.ReadLines(DirPath + Path.DirectorySeparatorChar + fileName);
        }
    }
}
