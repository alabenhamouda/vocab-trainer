using System;
using System.Collections.Generic;

namespace VocabTrainer.Domain.Models;

public class Lesson
{
    private readonly List<VocabEntry> _vocabEntries = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public int Order { get; private set; }
    public Guid CourseId { get; private set; }
    public IReadOnlyCollection<VocabEntry> VocabEntries => _vocabEntries.AsReadOnly();

    public Lesson(string title, int order, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be non-negative");
        if (courseId == Guid.Empty)
            throw new ArgumentException("CourseId cannot be empty", nameof(courseId));

        Id = Guid.NewGuid();
        Title = title;
        Order = order;
        CourseId = courseId;
    }

    public void AddVocabEntry(VocabEntry entry)
    {
        _vocabEntries.Add(entry);
    }
}
