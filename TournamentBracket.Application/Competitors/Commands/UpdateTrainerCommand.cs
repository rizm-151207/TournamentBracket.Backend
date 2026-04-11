namespace TournamentBracket.Application.Competitors.Commands;

public class UpdateTrainerCommand : CreateTrainerCommand
{
	public UpdateTrainerCommand(
		Guid id,
		string firstName,
		string lastName,
		string? middleName,
		string? club,
		string? subject)
		: base(firstName, lastName, middleName, club, subject)
	{
		Id = id;
	}

	public Guid Id { get; set; }
}