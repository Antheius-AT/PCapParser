using System.Net;

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
            };
        }

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
                        ParsePacketData(packetContents);
                    }
                    catch (Exception)
                    {
                        //TODO
                    }
                }
            }
        }

        private void ParsePacketData(byte[] packetData)
        {
            if (!(packetData[12] == 0x008 && packetData[13] == 0x00))
            {
                throw new NotSupportedException("Only IPv4 is supported");
            }

            packetData = packetData.Skip(14).ToArray();

            ParseIPv4Packet(packetData);
        }

        private void ParseIPv4Packet(byte[] packetData)
        {
            var ihl = (byte)(packetData[0] & 0x0F);
            var headerLength = ihl * 4;
            var protocol = packetData[9];

            // Check if protocol is supported
            if (!this.supportedProtocols.ContainsKey(protocol))
                throw new ArgumentException(nameof(packetData), $"Protocol {protocol} not supported");

            // IP Address
            var srcIP = new IPAddress(packetData.Skip(12).Take(4).ToArray());
            var destIP = new IPAddress(packetData.Skip(16).Take(4).ToArray());

            // TCP packet data
            packetData = packetData.Skip(headerLength).ToArray();
            var sourcePort = BitConverter.ToUInt16(packetData.ReverseEndianness(0, 1), 0);
            var destinationPort = BitConverter.ToUInt16(packetData.ReverseEndianness(2, 3), 2);

            var currentSession = TcpSessions.FirstOrDefault(p => p.SrcIP.Equals(srcIP) && p.DestIP.Equals(destIP) && p.SourcePort == sourcePort && p.DestinationPort == destinationPort);

            if (currentSession == null)
            {
                currentSession = new TcpSession(srcIP, destIP, sourcePort, destinationPort);
                TcpSessions.Add(currentSession);
            }

            currentSession.TotalBytesTransferred = packetData.Length + headerLength;
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