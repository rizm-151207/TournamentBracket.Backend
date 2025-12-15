namespace TournamentBracket.Application.Competitors.Commands;

public class CreateCompetitorCommand
{
    public CreateCompetitorCommand(
        string firstName,
        string lastName,
        string? middleName,
        bool gender,
        DateTime dateOfBirth,
        float weight,
        string? rank,
        string? kyu,
        string subject,
        List<Guid> trainersIds)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Weight = weight;
        Rank = rank;
        Kyu = kyu;
        Subject = subject;
        TrainersIds = trainersIds;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public float Weight { get; set; }
    public string? Rank { get; set; }
    public string? Kyu { get; set; }
    public string Subject { get; set; }
    public List<Guid>? TrainersIds { get; set; }
}