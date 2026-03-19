using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.VocabEntries.Dtos;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Courses.Queries;

public class GetLessonVocabEntriesQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetLessonVocabEntriesQuery, PaginatedList<VocabEntryDto>>
{
    public async Task<PaginatedList<VocabEntryDto>> Handle(
        GetLessonVocabEntriesQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = dbContext
            .VocabEntries.AsNoTracking()
            .Where(v => v.LessonId == request.LessonId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(v => v.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items
            .Select<VocabEntry, VocabEntryDto>(v =>
                v switch
                {
                    Noun n => n.Adapt<NounDto>(),
                    Expression e => e.Adapt<ExpressionDto>(),
                    _ => v.Adapt<VocabEntryDto>(),
                }
            )
            .ToList();

        return new PaginatedList<VocabEntryDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
