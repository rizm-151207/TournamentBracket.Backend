using FluentValidation;
using TournamentBracket.Application.Common.Validators;
using TournamentBracket.Application.Competitions.Queries;

namespace TournamentBracket.Application.Competitions.Validators;

public class CompetitionsPageQueryValidator : AbstractValidator<CompetitionsPageQuery>
{
    public CompetitionsPageQueryValidator(PageQueryValidator pageQueryValidator)
    {
        Include(pageQueryValidator);
        RuleFor(q => q.Name)
            .MaximumLength(256);
    }
}