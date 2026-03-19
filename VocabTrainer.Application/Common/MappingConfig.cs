using Mapster;
using VocabTrainer.Application.VocabEntries.Dtos;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Common;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<VocabEntry, VocabEntryDto>
            .NewConfig()
            .Map(dest => dest.Type, src => src.GetType().Name);

        TypeAdapterConfig<Noun, NounDto>.NewConfig().Map(dest => dest.Type, _ => nameof(Noun));

        TypeAdapterConfig<Expression, ExpressionDto>
            .NewConfig()
            .Map(dest => dest.Type, _ => nameof(Expression));
    }
}
