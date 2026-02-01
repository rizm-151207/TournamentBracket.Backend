using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.Helpers;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets.SingleEliminationBracket;

public class SingleEliminationBracketFactory : IBracketFactory
{
    private readonly BracketNodeFactory bracketNodeFactory;
    private readonly MatchFactory matchFactory;

    public SingleEliminationBracketFactory(
        BracketNodeFactory bracketNodeFactory,
        MatchFactory matchFactory)
    {
        this.bracketNodeFactory = bracketNodeFactory;
        this.matchFactory = matchFactory;
    }

    public Bracket CreateBracket(IList<Competitor> competitors)
    {
        //if (competitors.Count < 4)
        //    throw new ArgumentOutOfRangeException(nameof(competitors), "Division must have at least 4 competitors");

        var trainersToCompetitors = GetGroupedOrderByTrainersCompetitors(competitors)
            .SelectMany(kvp => kvp.Value)
            .ToList();

        var seed = BracketsHelpers.GetSeed(trainersToCompetitors.Count);
        var placedCompetitors = PlaceCompetitorsBySeed(seed, trainersToCompetitors);

        var roundFromFinal = (int)Math.Log2(placedCompetitors.Count) - 1;
        var bracketLeafs = CreateTreeLeafs(placedCompetitors, roundFromFinal);
        var rootNode = CreateTournamentTree(bracketLeafs, roundFromFinal);

        var bracket = new SingleEliminationBracket
        {
            Id = Guid.NewGuid(),
            Root = rootNode,
            ThirdPlace = bracketNodeFactory.Create(matchFactory.CreateEmptyUnplannedMatch(), 0, 1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        foreach (var node in bracket.GetAllNodes())
            node.BracketId = bracket.Id;

        return bracket;
    }

    public SingleEliminationBracket EnrichBracketWithNodes(SingleEliminationBracket baseBracket,
        List<BracketNode> nodes)
    {
        baseBracket.ThirdPlace = nodes.Single(n => n.Id == baseBracket.ThirdPlaceId);
        baseBracket.Root = ConstructTournamentTreeFromNodes(nodes, baseBracket.RootId);
        return baseBracket;
    }

    public void ExtendBracket(Bracket bracket)
    {
        var singleEliminationBracket = bracket as SingleEliminationBracket ?? throw new ArgumentException();
        var sortedLeafsWithMatches = singleEliminationBracket.GetAllNodesWithCompetitorsMatches()
            .Where(n => n.Children is null || n.Children.Count == 0
                                           || n is { IndexInRound: 1, RoundFromFinal: 0 })
            .OrderBy(n => n.IndexInRound)
            .ToList();
        var competitors = sortedLeafsWithMatches.Select(n => n.Match)
            .SelectMany(m => new List<Competitor?> { m.FirstCompetitor, m.SecondCompetitor })
            .Where(c => c is not null)
            .Distinct()
            .ToList();
        var sortedCompetitors = GetGroupedOrderByTrainersCompetitors(competitors!)
            .SelectMany(kvp => kvp.Value)
            .ToList();

        var seed = BracketsHelpers.GetSeed(sortedCompetitors.Count);
        var placedCompetitors = PlaceCompetitorsBySeed(seed, sortedCompetitors);

        var roundFromFinal = (int)Math.Log2(placedCompetitors.Count);
        var bracketLeafs = CreateTreeLeafsWithByeMatch(placedCompetitors!, roundFromFinal);
        bracketLeafs.ForEach(l => l.BracketId = bracket.Id);

        for (var i = 0; i < sortedLeafsWithMatches.Count; i++)
        {
            var leafWithMatch = sortedLeafsWithMatches[i];
            leafWithMatch.Match.Clear();

            var firstNewLeaf = bracketLeafs[2 * i];
            var secondNewLeaf = bracketLeafs[2 * i + 1];
            firstNewLeaf.SetParent(leafWithMatch);
            secondNewLeaf.SetParent(leafWithMatch);
            leafWithMatch.Children = [firstNewLeaf, secondNewLeaf];
        }
    }

    private Dictionary<string, List<Competitor>> GetGroupedOrderByTrainersCompetitors(IList<Competitor> competitors)
    {
        //Группируем по субъектам и сортируем по размеру полученных групп
        return competitors
            .GroupBy(c => c.Subject)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderByDescending(kvp => kvp.Value.Count)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        //string GetTrainersListId(IReadOnlyList<Trainer> trainers) =>
        //    string.Join(",", trainers.OrderBy(t => t.Id).Select(t => t.Id));
    }

    private List<Competitor?> PlaceCompetitorsBySeed(int[] seed, List<Competitor> competitors)
    {
        var placedCompetitors = new List<Competitor?>(seed.Length);
        foreach (var place in seed)
        {
            var index = place - 1;
            placedCompetitors.Add(index >= competitors.Count ? null : competitors[index]);
        }

        return placedCompetitors;
    }

    private List<BracketNode> CreateTreeLeafs(List<Competitor?> placedCompetitors, int roundFromFinal)
    {
        if (placedCompetitors.Count == 1)
        {
            var match = matchFactory.CreateLeafMatch(placedCompetitors[0], null);
            return [bracketNodeFactory.Create(match, 0, 0)];
        }

        var indexInRound = 0;
        var bracketLeafs = new List<BracketNode>();
        for (var i = 0; i < placedCompetitors.Count; i += 2, indexInRound++)
        {
            var firstCompetitor = placedCompetitors[i];
            var secondCompetitor = placedCompetitors[i + 1];
            if (firstCompetitor is null && secondCompetitor is null)
                throw new Exception("Error while create bracket. Two competitors are null");

            var match = matchFactory.CreateLeafMatch(firstCompetitor, secondCompetitor);
            bracketLeafs.Add(bracketNodeFactory.Create(match, roundFromFinal, indexInRound));
        }

        return bracketLeafs;
    }

    private List<BracketNode> CreateTreeLeafsWithByeMatch(List<Competitor> placedCompetitors, int roundFromFinal)
    {
        var bracketLeafs = new List<BracketNode>();
        for (var i = 0; i < placedCompetitors.Count; i++)
        {
            var match = matchFactory.CreateLeafMatch(placedCompetitors[i], null);
            bracketLeafs.Add(bracketNodeFactory.Create(match, roundFromFinal, i));
        }

        return bracketLeafs;
    }

    private BracketNode CreateTournamentTree(List<BracketNode> bracketLeafs, int roundFromFinal)
    {
        var currentRoundNodes = bracketLeafs.ToList();
        while (roundFromFinal > 0)
        {
            var nextRoundNodes = new List<BracketNode>();
            var indexInRound = 0;
            roundFromFinal--;

            for (var i = 0; i < currentRoundNodes.Count; i += 2, indexInRound++)
            {
                var leftNode = currentRoundNodes[i];
                var rightNode = currentRoundNodes[i + 1];

                var match = matchFactory.CreateEmptyUnplannedMatch();
                var node = bracketNodeFactory.Create(match, roundFromFinal, indexInRound,
                    children: [leftNode, rightNode]);
                leftNode.SetParent(node);
                rightNode.SetParent(node);

                nextRoundNodes.Add(node);
            }

            currentRoundNodes = nextRoundNodes;
        }

        return currentRoundNodes.Count != 1
            ? throw new Exception(
                $"Error while create bracket. Nodes after bracket creation is {currentRoundNodes.Count}, but 1 expected")
            : currentRoundNodes[0];
    }

    private BracketNode ConstructTournamentTreeFromNodes(List<BracketNode> nodes, Guid rootId)
    {
        // var nodesIds = nodes.ToDictionary(n => n.Id, n => n);
        // foreach (var node in nodes)
        // {
        //     if (node.ParentNodeId is null)
        //         continue;
        //
        //     var parent = nodesIds[node.ParentNodeId.Value];
        //     if (parent.Children is null)
        //         parent.Children = new List<BracketNode>(2) { node };
        //     else
        //         parent.Children.Add(node);
            //node.SetParent(parent);
        //}

        return nodes.Single(n => n.Id == rootId);
    }
}