using ConsoleUserControls;
using Tools;

Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

var parser = new PCapParser();
var maliciousUrls = new List<string>()
{
    "tryhackme",
    "google"
};

await parser.Parse(@"C:\Users\Grego\Downloads\wireshark\dump2.pcap");

var maliciousMatches = parser.ResolvedDNSQueries.Where(p => maliciousUrls.Any(t => p.Contains(t)));

var window = new Window();
window.ConsoleCoordinate.Left = 1;
window.ConsoleCoordinate.Top = 1;
window.Height = Console.WindowHeight - 5;
window.Width = Console.WindowWidth - 5;
window.Border.TopCharacter = '-';
window.Border.BottomCharacter = '-';


var listView = new ListView();
listView.Width = Console.WindowWidth - 10;
listView.Height = Console.WindowHeight - 10;
listView.ConsoleCoordinate = new ConsoleCoordinate();
listView.ConsoleCoordinate.Left = 5;
listView.ConsoleCoordinate.Top = 5;

Console.ReadKey(true);

VisualizeDNSQueries();

Console.ReadKey(true);

VisualizeTCPConnetions();

Console.ReadKey(true);

void VisualizeDNSQueries()
{
    listView.Content = parser.ResolvedDNSQueries.Distinct().Select(p => new TextBlock(p)
    {
        Height = 1,
        Width = 30,
        TrimText = true,
        InnerArea = maliciousMatches.Any(v => v == p)
    ? new ComponentInnerArea { ForegroundColor = ConsoleColor.DarkRed, BackgroundColor = ConsoleColor.Yellow }
    : new ComponentInnerArea { ForegroundColor = ConsoleColor.Black, BackgroundColor = ConsoleColor.White }
    }).ToList();


    window.AddChildren(listView);

    window.Draw();
    window.DrawChildren();
}

void VisualizeTCPConnetions()
{
    listView.Content = parser.TcpSessions.Take(22).Select(p => new TextBlock($"TCP flow von {p.SrcIP}:{p.SourcePort} zu {p.DestIP}:{p.DestinationPort}. Insgesamt {p.TotalBytesTransferred} bytes geschickt")
    {
        Height = 1,
        Width = Console.WindowWidth - 10,
        TrimText = false,
    }).ToList();

    window.Draw();
    window.DrawChildren();
}