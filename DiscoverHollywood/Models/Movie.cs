using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public int Year { get; set; }

        public string GenresStr { get; set; }

        public List<string> Genres
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GenresStr) ? GenresStr.Split(';').ToList() : new List<string>();
            }
        }

        public string Thumbnail { get; set; }

        public int? Rating { get; set; }

        public string Introduction { get; set; }

        public string Region { get; set; }

        public int ImdbId { get; set; }

        public int TmdbId { get; set; }

        public static Data.ColumnMapping TableMapping()
        {
            var mapping = new Data.ColumnMapping();
            mapping.Add("id", "Id");
            mapping.Add("name", "Name");
            mapping.Add("year", "Year");
            mapping.Add("genres", "GenresStr");
            mapping.Add("thumbnail", "Thumbnail");
            mapping.Add("rating", "Rating");
            mapping.Add("intro", "Introduction");
            mapping.Add("region", "Region");
            mapping.Add("imdb", "ImdbId");
            mapping.Add("tmdb", "TmdbId");
            return mapping;
        }
    }
}
