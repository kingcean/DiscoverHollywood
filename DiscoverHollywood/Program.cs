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
            if (args.Length > 0 && !Process(args)) return;
            while (true)
            {
                Console.WriteLine("You can search the movies by name, year and genres.");
                Console.Write("> ");
                var line = Console.ReadLine().Split(' ');
                if (line.Length < 1 || !Process(line)) return;
            }
        }

        static bool Process(string[] args)
        {
            if (args.Length < 1) return false;
            var key = args[0].ToUpper();
            switch (key)
            {
                case "INIT":
                case "SETUP":
                    Import.Job.Init();
                    break;
                case "CLEAR":
                    Import.Job.Clear();
                    break;
                case "SEARCH":
                case "LIST":
                case "QUERY":
                case "ITEMS":
                    {
                        var rec = DateTime.Now;
                        var list = Models.Job.ListMovieByCommandLine(string.Join(" ", args.Skip(1)));
                        foreach (var item in list)
                        {
                            Console.WriteLine("{0} \t{1} ({2}) \t★{3:0.0}", item.Id, item.Name, item.Year, item.Rating);
                        }

                        ConsoleUtils.Log(ref rec, "listing movies");
                        break;
                    }

                case "ENABLE":
                case "DISABLE":
                    {
                        var enable = key == "ENABLE" ? true : false;
                        if (args.Length < 2) break;
                        var field = args[1];
                        if (string.IsNullOrWhiteSpace(field)) break;
                        switch (field.Trim().ToUpper())
                        {
                            case "TAGS":
                                Import.Job.DisableTags = !enable;
                                break;
                        }

                        break;
                    }
                case "GET":
                case "ONE":
                case "ITEM":
                case "ENTRY":
                    {
                        var rec = DateTime.Now;
                        if (args.Length < 1) break;
                        int id;
                        if (!int.TryParse(args[1], out id)) break;
                        var entry = Models.Job.Movie(id);
                        if (entry != null)
                        {
                            Console.WriteLine("{1} ({2})\t ★{3:0.0}{0}{4}", Environment.NewLine, entry.Name, entry.Year, entry.Rating, string.Join("; ", entry.Genres));
                        }
                        else
                        {
                            ConsoleUtils.Log(ref rec, "No such movie.");
                        }

                        ConsoleUtils.Log(ref rec, "getting a movie");
                        break;
                    }

                case "EXIT":
                case "QUIT":
                case "Q":
                case "BYE":
                case "GODDBYE":
                case "OFF":
                    Console.WriteLine("Bye!");
                    return false;
            }

            Console.WriteLine();
            return true;
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
