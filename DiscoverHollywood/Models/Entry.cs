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

            set
            {
                Guid guid;
                if (!string.IsNullOrWhiteSpace(value) && Guid.TryParse(value.Trim(), out guid)) Id = guid;
                else Id = Guid.Empty;
            }
        }
    }

    public class MovieRelated: Entry
    {
        public int MovieId { get; set; }
    }
}
