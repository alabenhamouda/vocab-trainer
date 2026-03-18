using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using VocabTrainer.Application.Seeding;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Seeding;

public partial class DwNicosWegB1Provider(HttpClient httpClient) : ICourseProvider
{
    private const int CourseId = 36519718;
    private const string CourseName = "Nicos Weg B1";

    private const string CoursePageHash =
        "a5e023d88a6ba9d87206fbbba6311fc296eecddce87a30ad0568f4aad108c481";

    private const string LessonVocabHash =
        "cb132c62c5aa700fdf5e0c635a4abeb718688474a0a1d999dc608ace88015bed";

    private const string DefaultImageFormat = "601";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public async Task<Course> FetchAsync(CancellationToken cancellationToken)
    {
        var course = new Course(CourseName, null);
        var contentLinks = await FetchContentLinksAsync(cancellationToken);

        var order = 0;
        foreach (var link in contentLinks)
        {
            if (link.AdditionalInformation == "final_test")
                continue;

            var lesson = new Lesson(link.Target.ShortTitle, order, course.Id);
            course.AddLesson(lesson);

            var glossaries = await FetchGlossariesAsync(link.Target.Id, cancellationToken);
            foreach (var glossary in glossaries)
            {
                var definition = StripHtml(glossary.Text);
                var imageUrl = ResolveImageUrl(glossary);

                var entry = new Expression(
                    glossary.Name,
                    definition,
                    englishTranslation: null,
                    imageUrl
                );
                lesson.AddVocabEntry(entry);
            }

            order++;
        }

        return course;
    }

    private async Task<List<DwContentLink>> FetchContentLinksAsync(
        CancellationToken cancellationToken
    )
    {
        var url = BuildGraphQlUrl(
            "CoursePage",
            new
            {
                id = CourseId,
                lang = "GERMAN",
                appName = "mdl",
            },
            CoursePageHash
        );

        var request = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Content = new StringContent(
                string.Empty,
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            ),
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result =
            JsonSerializer.Deserialize<DwCoursePageResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize course page response");

        return result.Data.Content.ContentLinks;
    }

    private async Task<List<DwGlossary>> FetchGlossariesAsync(
        long lessonId,
        CancellationToken cancellationToken
    )
    {
        var url = BuildGraphQlUrl(
            "LessonVocabulary",
            new
            {
                lessonId,
                lessonLang = "GERMAN",
                appName = "mdl",
            },
            LessonVocabHash
        );

        var request = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Content = new StringContent(
                string.Empty,
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            ),
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result =
            JsonSerializer.Deserialize<DwLessonVocabResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize vocabulary response for lesson {lessonId}"
            );

        return result.Data.Content.Glossaries;
    }

    private static string BuildGraphQlUrl(string operationName, object variables, string sha256Hash)
    {
        var variablesJson = Uri.EscapeDataString(JsonSerializer.Serialize(variables, JsonOptions));
        var extensionsJson = Uri.EscapeDataString(
            JsonSerializer.Serialize(
                new { persistedQuery = new { version = 1, sha256Hash } },
                JsonOptions
            )
        );

        return $"/graphql?operationName={operationName}&variables={variablesJson}&extensions={extensionsJson}";
    }

    private static string? ResolveImageUrl(DwGlossary glossary)
    {
        var image = glossary.Audios?.FirstOrDefault()?.MainContentImage;
        return image?.StaticUrl.Replace("${formatId}", DefaultImageFormat);
    }

    private static string StripHtml(string html)
    {
        var stripped = HtmlTagRegex().Replace(html, "");
        return WebUtility.HtmlDecode(stripped).Trim();
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    #region DW API response DTOs

    private sealed record DwCoursePageResponse(DwCoursePageData Data);

    private sealed record DwCoursePageData(DwCoursePageContent Content);

    private sealed record DwCoursePageContent(List<DwContentLink> ContentLinks);

    private sealed record DwContentLink(
        long Id,
        string? AdditionalInformation,
        string? GroupName,
        DwLessonTarget Target
    );

    private sealed record DwLessonTarget(
        long Id,
        string ShortTitle,
        string? LearningTargetHeadline
    );

    private sealed record DwLessonVocabResponse(DwLessonVocabData Data);

    private sealed record DwLessonVocabData(DwLessonVocabContent Content);

    private sealed record DwLessonVocabContent(List<DwGlossary> Glossaries);

    private sealed record DwGlossary(string Name, string Text, List<DwAudio>? Audios);

    private sealed record DwAudio(DwImage? MainContentImage);

    private sealed record DwImage(long Id, string StaticUrl);

    #endregion
}
