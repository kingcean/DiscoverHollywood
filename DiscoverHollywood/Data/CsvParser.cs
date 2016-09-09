using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverHollywood.Data
{
    public class CsvParser
    {
        private static CsvParser instance;

        public static CsvParser Instance
        {
            get
            {
                if (instance == null) instance = new CsvParser();
                return instance;
            }
        }

        public List<string> ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return new List<string>();
            var arr = line.Split(',');
            if (line.IndexOf("\"") < 0) return arr.ToList();
            var list = new List<string>();
            var inScope = false;
            foreach (var item in arr)
            {
                if (!inScope)
                {
                    if (item.Length > 0 && item[0] == '"')
                    {
                        list.Add(item.Substring(1));
                        inScope = true;
                    }
                    else
                    {
                        list.Add(item);
                    }

                    continue;
                }

                if (item.Length > 0 && item[item.Length - 1] == '"' && (item.Length == 1 || item[item.Length - 2] != '\\'))
                {
                    list[list.Count - 1] += "," + item.Substring(0, item.Length - 1);
                    inScope = false;
                }
                else
                {
                    list[list.Count - 1] += "," + item;
                }
            }

            return list;
        }
    }
}
