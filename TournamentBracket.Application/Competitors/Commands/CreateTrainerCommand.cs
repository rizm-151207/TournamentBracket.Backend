namespace TournamentBracket.Application.Competitors.Commands;

public class CreateTrainerCommand
{
    public CreateTrainerCommand(
        string firstName,
        string lastName,
        string? middleName,
        string? club,
        string? subject)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Club = club;
        Subject = subject;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Club { get; set; }
    public string? Subject { get; set; }
}