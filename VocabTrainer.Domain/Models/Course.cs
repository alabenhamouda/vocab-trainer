using System;
using System.Collections.Generic;

namespace VocabTrainer.Domain.Models;

public class Course
{
    private readonly List<Lesson> _lessons = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    public Course(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }

    public void AddLesson(Lesson lesson)
    {
        if (lesson.CourseId != Id)
            throw new ArgumentException("Lesson does not belong to this course", nameof(lesson));

        _lessons.Add(lesson);
    }
}
