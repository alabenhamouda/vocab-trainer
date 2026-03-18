namespace VocabTrainer.Infrastructure.Seeding;

public class ClassificationResult
{
    public required string Type { get; set; } // "Noun" or "Expression"
    public string? Gender { get; set; } // "Masculine", "Feminine", "Neuter"
    public string? PluralForm { get; set; }
    public bool IsSingularOnly { get; set; }
    public bool IsPluralOnly { get; set; }
    public required string EnglishTranslation { get; set; }
    public required string Example { get; set; }
}
