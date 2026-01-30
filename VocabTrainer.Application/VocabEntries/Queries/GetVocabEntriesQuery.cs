using MediatR;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.VocabEntries.Queries;

public record GetVocabEntriesQuery : IRequest<List<VocabEntry>>;
