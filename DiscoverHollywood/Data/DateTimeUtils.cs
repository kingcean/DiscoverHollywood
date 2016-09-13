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

        public static DateTime ParseFromJava(int milliseconds, int x = 1)
        {
            var span = x == 1000 ? new TimeSpan(0, 0, milliseconds) : new TimeSpan(0, 0, 0, 0, milliseconds * x);
            return begin + span;
        }

        public static DateTime ParseFromJava(string milliseconds, int x = 1)
        {
            int num;
            return int.TryParse(milliseconds, out num) ? ParseFromJava(num, x) : DateTime.MinValue;
        }
    }
}
