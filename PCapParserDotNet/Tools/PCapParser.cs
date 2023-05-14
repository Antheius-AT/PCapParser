using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Tools
{
    public class PCapParser
    {
        private Dictionary<int, string> supportedProtocols;

        public PCapParser()
        {
            this.TcpSessions = new List<TcpSession>();
            supportedProtocols = new Dictionary<int, string>
            {
                {6, "tcp" },
                {17, "udp" },
            };
        }

        public List<string> ResolvedDNSQueries { get; set; } = new List<string>();

        public List<TcpSession> TcpSessions
        {
            get;
        }

        public async Task Parse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException(nameof(filePath), "File doesnt exist");
            }

            using (var fs = File.OpenRead(filePath))
            using (var reader = new BinaryReader(fs))
            {
                var header = ParseFileHeader(reader);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var length = ParsePacketLength(reader);
                    var packetContents = reader.ReadBytes(Convert.ToInt32(length));

                    try
                    {
                        var packetData = ParsePacketData(packetContents);
                        var ipHeader = ParseIPHeader(packetData);

                        // Check if protocol is supported
                        if (!this.supportedProtocols.TryGetValue(ipHeader.Protocol, out string protocolName))
                        {
                            throw new Exception();
                        }

                        switch (protocolName)
                        {
                            case "tcp":
                                HandleTCPPacket(ipHeader, packetData);
                                break;
                            case "udp":
                                this.HandleUDPPacket(ipHeader, packetData);
                                break;
                            default:
                                break;
                        }


                        //currentSession.TotalBytesTransferred = ipHeader.HeaderLength + packetData.Length;
                    }
                    catch (Exception)
                    {
                        //TODO
                    }
                }
            }
        }

        private void HandleUDPPacket(IPHeader ipHeader, byte[] packetData)
        {
            this.ParseUDPPacket(ipHeader, packetData.Skip(ipHeader.HeaderLength).ToArray());
        }

        private void ParseUDPPacket(IPHeader ipHeader, byte[] packetData)
        {
            var portInfo = packetData.GetPortInformation();

            if (portInfo.DestPort != 53)
                return;

            var udpData = packetData.Skip(8).Take(packetData.Length - 8).ToArray();
            var dnsEntries = ParseDnsInformation(udpData);
            ResolvedDNSQueries.AddRange(dnsEntries);
        }

        private IEnumerable<string> ParseDnsInformation(byte[] packetData)
        {
            var domainNames = new List<string>();
            var dnsQuestionsAmount = BitConverter.ToUInt16(packetData.ReverseEndianness(4, 5), 4);

            var startPosition = 12;

            for (int i = 0; i < dnsQuestionsAmount; i++)
            {
                domainNames.Add(ReadDomainName(packetData, startPosition));
            }

            return domainNames;
        }

        private string ReadDomainName(byte[] data, int currentPosition)
        {
            int labelLength;
            var domainName = string.Empty;

            do
            {
                labelLength = data[currentPosition];
                var isCompressed = (labelLength & 0xC0) == 0xC0;

                domainName += $"{Encoding.ASCII.GetString(data, currentPosition + 1, labelLength)}.";

                currentPosition = isCompressed
                    ? ((labelLength & 0x3F) << 8) + data[currentPosition]
                    : currentPosition + labelLength + 1;
            }
            while (labelLength != 0);

            return domainName.TrimEnd('.');
        }

        private void HandleTCPPacket(IPHeader ipHeader, byte[] packetData)
        {
            var tcpSession = this.ParseTCPPacket(ipHeader, packetData.Skip(ipHeader.HeaderLength).ToArray());
            tcpSession.TotalBytesTransferred += packetData.Length;

            if (tcpSession.SourcePort == 53 || tcpSession.DestinationPort == 53)
            {
                ParseDnsInformation(packetData[22..]);
            }
        }

        private byte[] ParsePacketData(byte[] packetData)
        {
            if (!(packetData[12] == 0x008 && packetData[13] == 0x00))
            {
                throw new NotSupportedException("Only IPv4 is supported");
            }

            packetData = packetData.Skip(14).ToArray();

            return packetData;
        }

        private TcpSession ParseTCPPacket(IPHeader ipHeader, byte[] packetData)
        {
            var portInfo = packetData.GetPortInformation();

            var currentSession = TcpSessions.FirstOrDefault(p => p.SrcIP.Equals(ipHeader.SrcIP) && p.DestIP.Equals(ipHeader.DestIP) && p.SourcePort == portInfo.SrcPort && p.DestinationPort == portInfo.DestPort);

            if (currentSession == null)
            {
                currentSession = new TcpSession(ipHeader.SrcIP, ipHeader.DestIP, portInfo.SrcPort, portInfo.DestPort);
                TcpSessions.Add(currentSession);
            }

            return currentSession;
        }

        private IPHeader ParseIPHeader(byte[] packetData)
        {
            var ihl = (byte)(packetData[0] & 0x0F);
            var headerLength = ihl * 4;
            var protocol = packetData[9];

            // IP Address
            var srcIP = new IPAddress(packetData.Skip(12).Take(4).ToArray());
            var destIP = new IPAddress(packetData.Skip(16).Take(4).ToArray());

            return new IPHeader(headerLength, protocol, srcIP, destIP);           
        }

        private uint ParsePacketLength(BinaryReader reader)
        {
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            var length = reader.ReadUInt32();

            return length;
        }

        private GlobalHeader ParseFileHeader(BinaryReader reader)
        {
            var magicNumber = reader.ReadUInt32();

            if (magicNumber != 2712847316)
            {
                throw new ArgumentException(nameof(reader), "Reader not set to file start");
            }

            var majorVersion = reader.ReadUInt16();
            var minorVersion = reader.ReadUInt16();
            var timeZone = reader.ReadInt32();
            var sigFigs = reader.ReadUInt32();
            var snapLen = reader.ReadUInt32();
            var network = reader.ReadUInt32();

            return new GlobalHeader(magicNumber, majorVersion, minorVersion, timeZone, sigFigs, snapLen, network);
        }
    }
}