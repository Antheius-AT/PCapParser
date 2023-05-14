using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class PortInformation
    {
        public PortInformation(ushort srcPort, ushort destPort)
        {
            SrcPort = srcPort;
            DestPort = destPort;
        }

        public ushort SrcPort { get; }
        public ushort DestPort { get; }
    }
}
