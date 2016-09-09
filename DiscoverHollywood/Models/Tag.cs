using System;

namespace DiscoverHollywood.Models
{
    public class Tag: MovieRelated
    {
        public int UserId { get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; }

        public static Data.ColumnMapping TableMapping()
        {
            var mapping = new Data.ColumnMapping();
            mapping.Add("id", "IdString");
            mapping.Add("movie", "MovieId");
            mapping.Add("user", "UserId");
            mapping.Add("tag", "Value");
            mapping.Add("created", "Created");
            return mapping;
        }
    }
}