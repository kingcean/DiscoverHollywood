using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Models
{
    public class Rating: MovieRelated
    {
        public int UserId { get; set; }

        public float Value { get; set; }

        public DateTime Created { get; set; }
    }

    public class RatingSummary: MovieRelated
    {
        public double Value { get; set; }

        public int Count { get; set; }

        public int CreatedYear { get; set; }

        public static Data.ColumnMapping TableMapping()
        {
            var mapping = new Data.ColumnMapping();
            mapping.Add("id", "IdString");
            mapping.Add("movie", "MovieId");
            mapping.Add("rating", "Value");
            mapping.Add("count", "Count");
            mapping.Add("year", "CreatedYear");
            return mapping;
        }
    }
}
