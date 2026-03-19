using MediatR;

namespace VocabTrainer.Application.Common;

public class PaginationValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPaginatedQuery
{
    private const int MaxPageSize = 100;

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request.Page < 1)
            throw new ArgumentOutOfRangeException(nameof(request.Page), "Page must be at least 1.");

        if (request.PageSize < 1)
            throw new ArgumentOutOfRangeException(
                nameof(request.PageSize),
                "PageSize must be at least 1."
            );

        if (request.PageSize > MaxPageSize)
            throw new ArgumentOutOfRangeException(
                nameof(request.PageSize),
                $"PageSize cannot exceed {MaxPageSize}."
            );

        return next();
    }
}
