using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using VocabTrainer.Application.Seeding;
using VocabTrainer.Domain.Enums;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Seeding;

public class LlmVocabClassifier(IChatClient chatClient, ILogger<LlmVocabClassifier> logger)
    : IVocabClassifier
{
    private const string SystemPrompt = """
        You are a German language expert. You will receive a numbered list of German vocabulary entries, each with a name and a definition.

        For each entry:
        1. Classify as either "Noun" or "Expression".
           - A Noun is a single German noun (Substantiv). The name often contains gender hints like (m.), (f.), (n.) and plural info like (nur Singular), (nur Plural), or (Pl.: ...).
           - An Expression is everything else: verbs, adjectives, phrases, idioms, etc.
        2. If it is a Noun, extract:
           - Gender: "Masculine" (der / m.), "Feminine" (die / f.), or "Neuter" (das / n.)
           - PluralForm: the plural form if provided (e.g. from "Pl.: Agenturen")
           - IsSingularOnly: true if "nur Singular" is indicated
           - IsPluralOnly: true if "nur Plural" is indicated
        3. Provide an English translation of the entry based on the provided definition.
        4. Provide one example sentence in German that uses the vocabulary entry in a way that illustrates the meaning defined.
        5. Return the index of the entry, to make it easy to match classification results back to the original entries.

        Return the results in the exact same order as the input entries.
        """;

    public async Task<List<VocabEntry>> ClassifyBatchAsync(
        IReadOnlyList<VocabEntry> entries,
        CancellationToken cancellationToken
    )
    {
        var sb = new StringBuilder();
        for (var i = 0; i < entries.Count; i++)
        {
            sb.AppendLine($"Entry {i + 1}:");
            sb.AppendLine($"  Name: {entries[i].Term}");
            sb.AppendLine($"  Definition: {entries[i].Definition}");
        }

        var result = await chatClient.GetResponseAsync<BatchClassificationResult>(
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, sb.ToString()),
            ],
            cancellationToken: cancellationToken
        );

        var batch =
            result.Result
            ?? throw new InvalidOperationException(
                "LLM returned no structured result for batch classification"
            );

        if (batch.Entries.Count != entries.Count)
        {
            logger.LogWarning(
                "LLM returned {ActualCount} results but expected {ExpectedCount}",
                batch.Entries.Count,
                entries.Count
            );
        }

        var classified = new List<VocabEntry>(entries.Count);
        foreach (var classification in batch.Entries)
        {
            var source = entries[classification.Index - 1];

            classified.Add(
                classification.Type == "Noun"
                    ? CreateNoun(source, classification)
                    : CreateExpression(source, classification)
            );
        }

        return classified;
    }

    private static Noun CreateNoun(VocabEntry source, ClassificationResult result)
    {
        var gender = result.Gender switch
        {
            "Masculine" => Gender.Masculine,
            "Feminine" => Gender.Feminine,
            "Neuter" => Gender.Neuter,
            _ => Gender.Masculine,
        };

        return new Noun(
            source.Term,
            source.Definition,
            result.EnglishTranslation,
            source.ImageUrl,
            gender,
            result.PluralForm,
            result.IsSingularOnly,
            result.IsPluralOnly,
            result.Example
        );
    }

    private static Expression CreateExpression(VocabEntry source, ClassificationResult result)
    {
        return new Expression(
            source.Term,
            source.Definition,
            result.EnglishTranslation,
            source.ImageUrl,
            result.Example,
            isClassified: true
        );
    }
}
