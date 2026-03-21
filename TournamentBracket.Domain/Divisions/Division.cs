using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Divisions;

public class Division : IEntity<Guid>
{
    public const int MinDivisionAge = 10;
    public const int KidAgeStep = 2;
    public const int MaxKidAge = 18;
    public const int VeteranAge = 45;

    public Guid Id { get; set; }
    public Guid CompetitionId { get; set; }
    public string Name { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public float? MinWeight { get; set; }
    public float? MaxWeight { get; set; }
    public bool Gender { get; set; }
    public int? Tatami { get; set; }
    public Guid TournamentBracketId { get; set; }
    public BracketType BracketType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Competitor> Competitors { get; private set; } = new();

    //links
    public Competition Competition { get; private set; }

    private Division()
    {
    }

    public Division(
        Guid competitionId,
        bool gender,
        int? minAge = null,
        int? maxAge = null,
        float? minWeight = null,
        float? maxWeight = null)
    {
        if (minAge is null && maxAge is null)
            throw new ArgumentNullException($"At least one of {nameof(minAge)} or  {nameof(maxAge)} must be provided");

        CompetitionId = competitionId;
        Gender = gender;
        MinAge = minAge;
        MaxAge = maxAge;
        MinWeight = minWeight;
        MaxWeight = maxWeight;
        Name = CreateDivisionName();
        UpdatedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public bool TryAddCompetitor(Competitor competitor)
    {
        if (Competitors.Contains(competitor))
            return false;

        if (IsCompetitorFits(competitor))
        {
            Competitors.Add(competitor);
            competitor.Divisions.Add(this);
            return true;
        }

        return false;
    }

    public bool TryRemoveCompetitor(Competitor competitor)
    {
        if (!Competitors.Contains(competitor))
            return false;

        Competitors.Remove(competitor);
        competitor.Divisions.Remove(this);

        return true;
    }

    public bool IsCompetitorFits(Competitor competitor)
    {
        return Gender == competitor.Gender
               && (MinWeight is null || competitor.Weight >= MinWeight)
               && (MaxWeight is null || competitor.Weight <= MaxWeight)
               && (MinAge is null || competitor.Age >= MinAge)
               && (MaxAge is null || competitor.Age <= MaxAge);
    }

    private string CreateDivisionName()
    {
        return $"{GetGenderPart()} {GetAgePart()}{GetWeightPart()}";

        string GetGenderPart() => Gender switch
        {
            false when MaxAge is not null && MaxAge < MaxKidAge => "Девочки",
            false when MaxAge is null || MaxAge >= MaxKidAge => "Женщины",
            true when MaxAge is not null && MaxAge < MaxKidAge => "Мальчики",
            true when MaxAge is null || MaxAge >= MaxKidAge => "Мужчины",
            _ => throw new NotImplementedException(
                $"Can't resolve gender part for Gender: {Gender}, MinAge: {MinAge}, MaxAge: {MaxAge}")
        };

        string GetAgePart()
        {
            if (MinAge is not null && MaxAge is not null)
                return $"{MinAge} - {MaxAge} {GetPluralYears(MaxAge.Value)}";
            if (MinAge is not null)
                return $"от {MinAge} {GetPluralYears(MinAge.Value)}";
            if (MaxAge is not null)
                return $"до {MaxAge} {GetPluralYears(MaxAge.Value)}";
            throw new InvalidOperationException($"Can't construct age part for MinAge: {MinAge}, MaxAge: {MaxAge}");
        }

        string GetWeightPart()
        {
            if (MinWeight is not null && MaxWeight is not null)
                return $" ({MinWeight} - {MaxWeight} Кг.)";
            if (MinWeight is not null)
                return $" (от {MinWeight} Кг.)";
            if (MaxWeight is not null)
                return $" (до  {MaxWeight + 1})";
            return "";
        }

        string GetPluralYears(int years) => years % 10 == 1 ? "года" : "лет";
    }
}