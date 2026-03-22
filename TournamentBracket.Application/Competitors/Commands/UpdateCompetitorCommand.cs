namespace TournamentBracket.Application.Competitors.Commands;

public class UpdateCompetitorCommand : CreateCompetitorCommand
{
    public UpdateCompetitorCommand(
        Guid id,
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
        : base(firstName, lastName, middleName, gender, dateOfBirth, weight, rank, kyu, subject, trainersIds)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}