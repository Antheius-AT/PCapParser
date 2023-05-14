using Tools;

var parser = new PCapParser();
var maliciousUrls = new List<string>()
{
    "akamaiedge.net",
};

await parser.Parse(@"F:\FH_Technikum_Wien\2_Se\Firewalls\PcapFiles\Dump1.pcap");

var maliciousMatches = parser.ResolvedDNSQueries.Where(p => maliciousUrls.Any(t => p.Contains(t)));

Console.WriteLine("Tuples:");
foreach (var item in parser.TcpSessions)
{
    Console.WriteLine($"Source: {item.SrcIP}:{item.SourcePort}\nDestination: {item.DestIP}:{item.DestinationPort}");
}

foreach (var item in maliciousMatches)
{
    Console.WriteLine($"Malicious url found in pcap file {item}");
}

Console.ReadKey(true);