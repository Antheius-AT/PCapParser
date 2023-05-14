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

        public static PortInformation GetPortInformation(this byte[] source)
        {
            return new PortInformation(BitConverter.ToUInt16(source.ReverseEndianness(0, 1), 0),
                BitConverter.ToUInt16(source.ReverseEndianness(2, 3), 2));
        }
    }
}
