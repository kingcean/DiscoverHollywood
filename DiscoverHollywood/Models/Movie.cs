﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Models
{
    public class Movie
    {
        private string name;
        private string genres;

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
            }
        }


        public int Year { get; set; }

        public string GenresStr
        {
            get
            {
                return genres;
            }

            set
            {
                var g = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
                genres = g == "(no genres listed)" ? string.Empty : g;
            }
        }


        public List<string> Genres
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GenresStr) ? GenresStr.Trim().Split(';').ToList() : new List<string>();
            }
        }

        public string Thumbnail { get; set; }

        public double? Rating { get; set; }

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
