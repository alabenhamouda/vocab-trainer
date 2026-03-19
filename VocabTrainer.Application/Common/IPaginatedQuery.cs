using MediatR;

namespace VocabTrainer.Application.Common;

public interface IPaginatedQuery
{
    int Page { get; }
    int PageSize { get; }
}
