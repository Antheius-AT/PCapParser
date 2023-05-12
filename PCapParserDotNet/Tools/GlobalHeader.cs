using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class GlobalHeader
    {
        public GlobalHeader(uint magicNumber, ushort majorVersion, ushort minorVersion, int timeZone, uint sigFigs, uint snapLen, uint network)
        {
            MagicNumber = magicNumber;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            TimeZone = timeZone;
            SigFigs = sigFigs;
            SnapLen = snapLen;
            Network = network;
        }

        public uint MagicNumber { get; }
        public ushort MajorVersion { get; }
        public ushort MinorVersion { get; }
        public int TimeZone { get; }
        public uint SigFigs { get; }
        public uint SnapLen { get; }
        public uint Network { get; }
    }
}
