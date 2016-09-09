using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Discovery Hollywood" + Environment.NewLine);
            Import.Job.Process();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    public class ConsoleUtils
    {
        public static void Log(ref DateTime start, string message)
        {
            Console.WriteLine("Cost {0:0,0}ms for {1}.", (DateTime.Now - start).TotalMilliseconds, message);
            start = DateTime.Now;
        }
    }
}
