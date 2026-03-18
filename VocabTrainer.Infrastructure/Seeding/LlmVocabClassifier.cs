using Microsoft.Extensions.AI;
using VocabTrainer.Application.Seeding;
using VocabTrainer.Domain.Enums;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Seeding;

public class LlmVocabClassifier(IChatClient chatClient) : IVocabClassifier
{
    private const string SystemPrompt = """
        You are a German language expert. You will receive a German vocabulary entry with a name and a definition.

        Your tasks:
        1. Classify the entry as either "Noun" or "Expression".
           - A Noun is a single German noun (Substantiv). The name often contains gender hints like (m.), (f.), (n.) and plural info like (nur Singular), (nur Plural), or (Pl.: ...).
           - An Expression is everything else: verbs, adjectives, phrases, idioms, etc.
        2. If it is a Noun, extract:
           - Gender: "Masculine" (der / m.), "Feminine" (die / f.), or "Neuter" (das / n.)
           - PluralForm: the plural form if provided (e.g. from "Pl.: Agenturen")
           - IsSingularOnly: true if "nur Singular" is indicated
           - IsPluralOnly: true if "nur Plural" is indicated
        3. Provide an English translation of the entry based on the provided definition.
        4. Provide one example sentence in German that uses the vocabulary entry in a way that illustrates the meaning defined.
        """;

    public async Task<VocabEntry> ClassifyAsync(
        string rawName,
        string rawDefinition,
        string? imageUrl,
        CancellationToken cancellationToken
    )
    {
        var userMessage = $"Name: {rawName}\nDefinition: {rawDefinition}";

        var result = await chatClient.GetResponseAsync<ClassificationResult>(
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, userMessage),
            ],
            cancellationToken: cancellationToken
        );

        var classification =
            result.Result
            ?? throw new InvalidOperationException(
                $"LLM returned no structured result for '{rawName}'"
            );

        return classification.Type == "Noun"
            ? CreateNoun(rawName, rawDefinition, imageUrl, classification)
            : CreateExpression(rawName, rawDefinition, imageUrl, classification);
    }

    private static Noun CreateNoun(
        string rawName,
        string rawDefinition,
        string? imageUrl,
        ClassificationResult result
    )
    {
        var gender = result.Gender switch
        {
            "Masculine" => Gender.Masculine,
            "Feminine" => Gender.Feminine,
            "Neuter" => Gender.Neuter,
            _ => Gender.Masculine,
        };

        return new Noun(
            rawName,
            rawDefinition,
            result.EnglishTranslation,
            imageUrl,
            gender,
            result.PluralForm,
            result.IsSingularOnly,
            result.IsPluralOnly,
            result.Example
        );
    }

    private static Expression CreateExpression(
        string rawName,
        string rawDefinition,
        string? imageUrl,
        ClassificationResult result
    )
    {
        return new Expression(
            rawName,
            rawDefinition,
            result.EnglishTranslation,
            imageUrl,
            result.Example
        );
    }
}
