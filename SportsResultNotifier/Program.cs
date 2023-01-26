using SportsResultNotifier.Services;

Console.WriteLine("Hello, World!");


var scraper = new ScraperService("https://www.basketball-reference.com/boxscores/");

var results = scraper.GetResults();

foreach (var result in results)
{
    Console.WriteLine($"{result.HomeTeam.Name} - {result.HomeTeam.Score}");
    Console.WriteLine($"{result.AwayTeam.Name} - {result.AwayTeam.Score}");
    Console.WriteLine();
}