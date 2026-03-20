namespace VocabTrainer.Infrastructure.Seeding;

public class BatchClassificationResult
{
    public required List<ClassificationResult> Entries { get; set; }
}

public class ClassificationResult
{
    public required int Index { get; set; }
    public required string Type { get; set; } // "Noun" or "Expression"
    public string? Gender { get; set; } // "Masculine", "Feminine", "Neuter", "MasculineOrFeminine"
    public string? PluralForm { get; set; }
    public bool IsSingularOnly { get; set; }
    public bool IsPluralOnly { get; set; }
    public required string EnglishTranslation { get; set; }
    public required string Example { get; set; }
}
