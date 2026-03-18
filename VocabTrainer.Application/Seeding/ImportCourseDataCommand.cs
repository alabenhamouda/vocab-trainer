using MediatR;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public record ImportCourseDataCommand(Course Course) : IRequest;
