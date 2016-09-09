using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Models
{
    public class Entry
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public string IdString
        {
            get
            {
                return Id.ToString();
            }
        }
    }

    public class MovieRelated: Entry
    {
        public int MovieId { get; set; }
    }
}
