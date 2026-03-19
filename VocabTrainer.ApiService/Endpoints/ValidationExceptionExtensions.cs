using FluentValidation;

namespace VocabTrainer.ApiService.Endpoints;

public static class ValidationExceptionExtensions
{
    public static IDictionary<string, string[]> ToValidationErrors(this ValidationException ex)
    {
        return ex
            .Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
