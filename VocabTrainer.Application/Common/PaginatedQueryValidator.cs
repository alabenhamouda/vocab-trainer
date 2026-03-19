using FluentValidation;

namespace VocabTrainer.Application.Common;

public class PaginatedQueryValidator : AbstractValidator<IPaginatedQuery>
{
    private const int MaxPageSize = 100;

    public PaginatedQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize must be at least 1.")
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"PageSize cannot exceed {MaxPageSize}.");
    }
}
