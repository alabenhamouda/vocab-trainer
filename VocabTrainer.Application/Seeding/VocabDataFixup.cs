using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Enums;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public class VocabDataFixup(
    IVocabClassifier vocabClassifier,
    IVocabTrainerDbContext dbContext,
    ILogger<VocabDataFixup> logger
)
{
    private const int MaxBatchSize = 10;
    private static readonly TimeSpan DelayBetweenBatches = TimeSpan.FromSeconds(5);

    private static readonly Regex GenderOnlyPattern = new(
        @"\s*\((m\./f\.|[mfn]\.)\)",
        RegexOptions.Compiled
    );

    private static readonly Regex GenderWithOtherInfoPattern = new(
        @"\((m\./f\.|[mfn]\.),\s*",
        RegexOptions.Compiled
    );

    public async Task FixupAsync(CancellationToken cancellationToken)
    {
        var nouns = await dbContext.Nouns.ToListAsync(cancellationToken);

        if (nouns.Count == 0)
            return;

        // Identify entries needing reclassification BEFORE stripping gender indications.
        // Two cases:
        //   1. Term contains "(m./f.)" — explicit dual-gender indication
        //   2. Term has no gender indication (no m., f., n.) but contains ", "
        //      → dual-form entries like "Aussiedler, -/Aussiedlerin, -nen"
        // Skip entries that already have the correct MasculineOrFeminine gender.
        var needsReclassification = nouns
            .Where(n =>
                n.Gender != Gender.MasculineOrFeminine
                && (
                    n.Term.Contains("(m./f.)")
                    || (
                        !n.Term.Contains("m.")
                        && !n.Term.Contains("f.")
                        && !n.Term.Contains("n.")
                        && n.Term.Contains(", ")
                    )
                )
            )
            .ToList();

        // Reclassify via LLM before stripping so the LLM sees the original terms with full context
        if (needsReclassification.Count > 0)
        {
            logger.LogInformation(
                "{Count} nouns identified for gender reclassification",
                needsReclassification.Count
            );

            var batches = needsReclassification.Chunk(MaxBatchSize).ToList();
            for (var i = 0; i < batches.Count; i++)
            {
                var batch = batches[i];
                try
                {
                    var classified = await vocabClassifier.ClassifyBatchAsync(
                        batch,
                        cancellationToken
                    );

                    for (var j = 0; j < batch.Length && j < classified.Count; j++)
                    {
                        if (classified[j] is Noun reclassified)
                        {
                            batch[j].Gender = reclassified.Gender;
                            batch[j].PluralForm = reclassified.PluralForm;
                        }
                    }

                    logger.LogInformation(
                        "Reclassified batch {Batch}/{Total} ({Count} entries)",
                        i + 1,
                        batches.Count,
                        batch.Length
                    );
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogWarning(
                        ex,
                        "Reclassification failed for batch {Batch}/{Total}, skipping",
                        i + 1,
                        batches.Count
                    );
                }

                if (i < batches.Count - 1)
                    await Task.Delay(DelayBetweenBatches, cancellationToken);
            }
        }

        // Strip gender indication from ALL noun terms
        var strippedCount = 0;
        foreach (var noun in nouns)
        {
            var stripped = StripGenderIndication(noun.Term);
            if (stripped != noun.Term)
            {
                noun.Term = stripped;
                strippedCount++;
            }
        }

        if (strippedCount > 0)
            logger.LogInformation(
                "Stripped gender indication from {Count} noun terms",
                strippedCount
            );

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string StripGenderIndication(string term)
    {
        var result = GenderOnlyPattern.Replace(term, "");
        if (result != term)
            return result;

        return GenderWithOtherInfoPattern.Replace(term, "(");
    }
}
