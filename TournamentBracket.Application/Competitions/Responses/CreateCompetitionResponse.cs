namespace TournamentBracket.Application.Competitions.Responses;

public class CreateCompetitionResponse
{
	public CreateCompetitionResponse(Guid competitionId)
	{
		CompetitionId = competitionId;
	}

	public Guid CompetitionId { get; set; }
}