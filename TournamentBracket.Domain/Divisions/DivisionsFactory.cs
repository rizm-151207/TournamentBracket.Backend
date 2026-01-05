namespace TournamentBracket.Domain.Divisions;

public class DivisionsFactory
{
    public List<Division> CreateDefaultDivisions(Guid competitionId)
    {
        var divisions = new List<Division>
        {
            new(competitionId, false, maxAge: Division.MinDivisionAge),
            new(competitionId, true, maxAge: Division.MinDivisionAge)
        };

        for (var min = Division.MinDivisionAge; min < Division.MaxKidAge; min += Division.KidAgeStep)
        {
            divisions.Add(new(competitionId, false, minAge: min, maxAge: min + Division.KidAgeStep));
            divisions.Add(new(competitionId, true, minAge: min, maxAge: min + Division.KidAgeStep));
        }

        divisions.Add(new(competitionId, false, minAge: Division.MaxKidAge, maxAge: Division.VeteranAge));
        divisions.Add(new(competitionId, true, minAge: Division.MaxKidAge, maxAge: Division.VeteranAge));
        divisions.Add(new(competitionId, false, minAge: Division.VeteranAge));
        divisions.Add(new(competitionId, true, minAge: Division.VeteranAge));

        return divisions;
    }
}