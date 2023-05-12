using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class Extensions
    {
        public static byte[] ReverseEndianness(this byte[] source, int firstIndex, int secondIndex)
        {
            var tmp = source[firstIndex];
            source[firstIndex] = source[secondIndex];
            source[secondIndex] = tmp;

            return source;
        }
    }
}
