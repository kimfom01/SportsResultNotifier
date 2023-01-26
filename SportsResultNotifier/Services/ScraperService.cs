using HtmlAgilityPack;
using SportsResultNotifier.Models;

namespace SportsResultNotifier.Services;

public class ScraperService
{
    private readonly string _url;

    public ScraperService(string url)
    {
        _url = url;
    }

    private HtmlDocument GetDocument()
    {
        var web = new HtmlWeb();

        HtmlDocument doc = web.Load(_url);

        return doc;
    }

    public List<Result> GetResults()
    {
        var doc = GetDocument();

        var gameRootNode = GetRootNode(doc);

        var gameSummary = gameRootNode.SelectNodes("//table[@class = 'teams']");

        List<Result> results = new(gameSummary.Count);

        foreach (var game in gameSummary)
        {
            var homeTeam = GetTeam(game, Position.TeamA);

            var awayTeam = GetTeam(game, Position.TeamB);

            var result = new Result
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam
            };

            results.Add(result);
        }

        return results;
    }

    private HtmlNode GetRootNode(HtmlDocument document)
    {
        var rootNode = HtmlNode.CreateNode("<div class=\"game_summaries\"></div>");

        // var gameCards = document.DocumentNode.SelectNodes("//table[contains(@class, 'teams')]");
        var gameCards = document.DocumentNode.SelectNodes("//div[contains(@class, 'game_summaries')]");

        rootNode.AppendChildren(gameCards);

        return rootNode;
    }

    private Team GetTeam(HtmlNode game, Position position)
    {
        int index = GetIndex(position);

        var name = GetName(game, "//table[@class = 'teams']/tbody/tr/td/a", (int)position);
        var score = GetScore(game, index);

        var team = CreateTeam(name, score);

        return team;
    }

    private int GetScore(HtmlNode game, int index)
    {
        var scores = GetScores(game, "//tr/td[@class = 'right']");
        return scores[index];
    }

    private Team CreateTeam(string name, int score)
    {
        return new Team
        {
            Name = name,
            Score = score
        };
    }

    private int GetIndex(Position position)
    {
        int index = -1;

        switch (position)
        {
            case Position.TeamA:
                index = 0;
                break;
            case Position.TeamB:
                index = 1;
                break;
        }

        return index;
    }

    private List<int> GetScores(HtmlNode game, string xPath)
    {
        var nodes = game.SelectNodes(xPath);
        List<int> scoresList = new(2);

        foreach (var node in nodes)
        {
            if (int.TryParse(node.InnerText, out int score))
            {
                scoresList.Add(score);
            }
        }

        return scoresList;
    }

    private string GetName(HtmlNode game, string xPath, int position)
    {
        var name = game.SelectNodes(xPath)[position].InnerText;
        return name;
    }
}
