using FluentValidation;
using TournamentBracket.Application.Common.Queries;

namespace TournamentBracket.Application.Common.Validators;

public class PageQueryValidator : AbstractValidator<PageQuery>
{
    public PageQueryValidator()
    {
        RuleFor(p => p.Page)
            .GreaterThanOrEqualTo(0)
            .When(p => p.Page.HasValue);
        RuleFor(p => p.Count)
            .GreaterThan(0);
    }
}