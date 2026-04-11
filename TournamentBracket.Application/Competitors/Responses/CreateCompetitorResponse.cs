namespace TournamentBracket.Application.Competitors.Responses;

public class CreateCompetitorResponse
{
	public CreateCompetitorResponse(Guid competitorId)
	{
		CompetitorId = competitorId;
	}

	public Guid CompetitorId { get; set; }
}