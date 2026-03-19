namespace VocabTrainer.Application.Courses.Dtos;

public record LessonDto(Guid Id, string Title, int Order, Guid CourseId);
