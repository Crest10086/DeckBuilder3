using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools
{
    public static class MyInt
    {
        public static int ParseIntOrDefault(this string s, int defValue = default(int))
        {
            return int.TryParse(s, out var i) ? i : defValue;
        }
    }
}
