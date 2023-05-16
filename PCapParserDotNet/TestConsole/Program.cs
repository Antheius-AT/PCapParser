using ConsoleUserControls;
using Tools;

Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

var parser = new PCapParser();
var maliciousUrls = new List<string>()
{
    "akamaiedge.net",
};

await parser.Parse(@"F:\FH_Technikum_Wien\2_Se\Firewalls\PcapFiles\Dump1.pcap");

var maliciousMatches = parser.ResolvedDNSQueries.Where(p => maliciousUrls.Any(t => p.Contains(t)));

var window = new Window(2, 2, Console.WindowHeight - 5, Console.WindowWidth - 5, 1, "DNQ queries");

var listView = new ListView<TextBlock>(Console.WindowWidth - 10, Console.WindowHeight - 10, new ConsoleCoordinates(5, 5));
listView.Content = parser.ResolvedDNSQueries.Select(p => new ListViewItem<TextBlock>(new TextBlock()
{
    Height = 1,
    Width = 30,
    Text = p,
    InnerArea = maliciousMatches.Any(v => v == p)
    ? new InnerArea(ConsoleColor.DarkRed, ConsoleColor.Yellow)
    : new InnerArea(ConsoleColor.Black, ConsoleColor.White)
})).ToList();

window.AddChildren(listView);
window.Show();
//listView.Show();
Console.ReadKey(true);

//Console.WriteLine("Tuples:");
//foreach (var item in parser.TcpSessions)
//{
//    Console.WriteLine($"Source: {item.SrcIP}:{item.SourcePort}\nDestination: {item.DestIP}:{item.DestinationPort}");
//}

//foreach (var item in maliciousMatches)
//{
//    Console.WriteLine($"Malicious url found in pcap file {item}");
//}

//Console.ReadKey(true);