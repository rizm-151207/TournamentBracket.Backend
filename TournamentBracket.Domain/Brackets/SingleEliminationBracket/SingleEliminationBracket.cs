using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.Helpers;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets.SingleEliminationBracket;

public class SingleEliminationBracket : Bracket
{
    public override BracketType Type => BracketType.SingleElimination;
    public BracketNode Root { get; set; }
    public BracketNode ThirdPlace { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //links
    public Guid RootId { get; set; }
    public Guid ThirdPlaceId { get; set; }

    public override bool TryAddCompetitorAuto(Competitor competitor)
    {
        var nodesWithCompetitorsMatches = GetAllNodesWithCompetitorsMatches().ToList();
        var competitors = nodesWithCompetitorsMatches
            .SelectMany(n => new List<Competitor?>([n.Match.FirstCompetitor, n.Match.SecondCompetitor]))
            .Where(c => c is not null)
            .Select(c => c!)
            .Distinct()
            .ToList();

        if (competitors.Contains(competitor))
            return false;

        
        return TryAddToFreeMatch(nodesWithCompetitorsMatches, competitor, competitors.Count);
    }

    public override bool HasFreeMatch()
    {
        var nodesWithMatches = GetAllNodesWithCompetitorsMatches().ToList();
        return nodesWithMatches.Any(n => n.Match.IsByeMatch);
    }

    public override List<Competitor> GetAllCompetitors()
    {
        var nodesWithMatches = GetAllNodesWithCompetitorsMatches().ToList();
        return nodesWithMatches
            .SelectMany(n => new List<Competitor?>([n.Match.FirstCompetitor, n.Match.SecondCompetitor]))
            .Where(c => c is not null)
            .Select(c => c!)
            .Distinct()
            .ToList();
    }
    
    public override Dictionary<int, IReadOnlyCollection<Match>> GetGroupedMatchesByRounds()
    {
        return GetAllNodes()
            .GroupBy(n => n.RoundFromFinal)
            .ToDictionary(g => g.Key,
                g => GetSortedMatchesInRound(g.Key, g.ToList()));

        IReadOnlyCollection<Match> GetSortedMatchesInRound(int round, List<BracketNode> nodes)
        {
            if(round == 0) // Если это финал/полуфинал сортируем в обратном порядке
                return nodes.OrderByDescending(n => n.IndexInRound).Select(n => n.Match).ToList();
            return nodes.OrderBy(n => n.IndexInRound).Select(n => n.Match).ToList();
        }
    }

    public override void RefreshBracketAfterMatchUpdate(Match match)
    {
        var nodeWithUpdatedMatch = GetAllNodesWithCompetitorsMatches()
            .FirstOrDefault(n => n.Match == match);
        if(nodeWithUpdatedMatch is null)
            return;
        if (nodeWithUpdatedMatch.Parent is null)
            return;
        
        if(match.TryGetWinner(out var winner))
            nodeWithUpdatedMatch.Parent.Match.AddCompetitor(winner!);

        if (nodeWithUpdatedMatch.RoundFromFinal == 1 && !match.IsByeMatch) //semifinal
        {
            if(match.TryGetLoser(out var loser))
                ThirdPlace.Match.AddCompetitor(loser!);
        }
    }

    public IEnumerable<BracketNode> GetAllNodesWithCompetitorsMatches()
    {
        var nodesQueue = new Queue<BracketNode>([Root, ThirdPlace]);
        while (nodesQueue.Count > 0)
        {
            var currentNode = nodesQueue.Dequeue();
            if (currentNode.Match.FirstCompetitor is not null
                || currentNode.Match.SecondCompetitor is not null)
                yield return currentNode;
            if (currentNode.Children is not null)
                currentNode.Children.ForEach(child => nodesQueue.Enqueue(child));
        }
    }

    public IEnumerable<BracketNode> GetAllNodes()
    {
        yield return Root;
        yield return ThirdPlace;
        foreach (var child in Root.GetAllChildren())
            yield return child;
    }

    private bool TryAddToFreeMatch(List<BracketNode> nodesWithMatches, Competitor competitor, int competitorsCount)
    {
        var orderedNodes = nodesWithMatches.OrderBy(n => n.IndexInRound).ToList();
        var seed = BracketsHelpers.GetSeed(competitorsCount + 1);
        foreach (var n in seed)
        {
            var nodeIndex = (n - 1) / 2;
            var match = orderedNodes[nodeIndex].Match;
            if (match.Status is MatchStatus.Finished && match.MatchProcess.WinReason is WinReason.Bye)
            {
                match.SecondCompetitor = competitor;
                match.Status = MatchStatus.Planned;
                match.MatchProcess.WinReason = WinReason.Bye;
                match.MatchProcess.Winner = null;
                return true;
            }
        }

        return false;
    }
}