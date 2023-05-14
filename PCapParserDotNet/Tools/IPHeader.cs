using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class IPHeader
    {
        public IPHeader(int headerLength, byte protocol, System.Net.IPAddress srcIP, IPAddress destIP)
        {
            HeaderLength = headerLength;
            Protocol = protocol;
            SrcIP = srcIP;
            DestIP = destIP;
        }

        public int HeaderLength { get; }
        public byte Protocol { get; }
        public IPAddress SrcIP { get; }
        public IPAddress DestIP { get; }
    }
}
