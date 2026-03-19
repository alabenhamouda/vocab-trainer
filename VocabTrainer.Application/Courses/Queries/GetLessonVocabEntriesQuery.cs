using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.Courses.Queries;

public record GetLessonVocabEntriesQuery(Guid LessonId, int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<VocabEntryDto>>,
        IPaginatedQuery;
