using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Data
{
    public static class DateTimeUtils
    {
        static readonly DateTime begin = new DateTime(1970, 1, 1);

        public static DateTime ParseFromJava(long ticks, int x = 1)
        {
            var span = new TimeSpan(ticks * x);
            return begin + span;
        }

        public static DateTime ParseFromJava(string ticks, int x = 1)
        {
            long num;
            return long.TryParse(ticks, out num) ? ParseFromJava(num, x) : DateTime.MinValue;
        }
    }
}
