using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class TcpSession
    {
        public TcpSession(IPAddress srcIP, IPAddress destIP, ushort sourcePort, ushort destinationPort, int totalBytesTransferred = 0)
        {
            SrcIP = srcIP;
            DestIP = destIP;
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
            TotalBytesTransferred = totalBytesTransferred;
        }

        public IPAddress SrcIP { get; }
        public IPAddress DestIP { get; }
        public ushort SourcePort { get; }
        public ushort DestinationPort { get; }

        public int TotalBytesTransferred { get; set; }
    }
}
