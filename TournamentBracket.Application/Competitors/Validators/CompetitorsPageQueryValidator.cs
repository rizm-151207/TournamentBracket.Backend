using FluentValidation;
using TournamentBracket.Application.Common.Validators;
using TournamentBracket.Application.Competitors.Queries;

namespace TournamentBracket.Application.Competitors.Validators;

public class CompetitorsPageQueryValidator : AbstractValidator<CompetitorsPageQuery>
{
    public CompetitorsPageQueryValidator(PageQueryValidator pageQueryValidator)
    {
        Include(pageQueryValidator);
        RuleFor(q => q.FIO)
            .MaximumLength(256);
    }
}