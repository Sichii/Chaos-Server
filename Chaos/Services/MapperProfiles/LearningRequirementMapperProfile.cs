using Chaos.Models.Data;
using Chaos.Schemas.Data;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class LearningRequirementMapperProfile(ITypeMapper mapper) : IMapperProfile<LearningRequirements, LearningRequirementsSchema>
{
    private readonly ITypeMapper Mapper = mapper;

    /// <inheritdoc />
    public LearningRequirements Map(LearningRequirementsSchema obj)
        => new()
        {
            RequiredStats = obj.RequiredStats == null ? null : Mapper.Map<Stats>(obj.RequiredStats),
            PrerequisiteSkillTemplateKeys = obj.PrerequisiteSkillTemplateKeys,
            PrerequisiteSpellTemplateKeys = obj.PrerequisiteSpellTemplateKeys,
            ItemRequirements = Mapper.MapMany<ItemRequirement>(obj.ItemRequirements)
                                     .ToList(),
            RequiredGold = obj.RequiredGold
        };

    /// <inheritdoc />
    public LearningRequirementsSchema Map(LearningRequirements obj) => throw new NotImplementedException();
}