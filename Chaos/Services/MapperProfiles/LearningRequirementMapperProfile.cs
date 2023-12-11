using Chaos.Models.Data;
using Chaos.Schemas.Data;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class LearningRequirementMapperProfile(ITypeMapper mapper) : IMapperProfile<LearningRequirements, LearningRequirementsSchema>,
                                                                    IMapperProfile<AbilityRequirement, AbilityRequirementSchema>
{
    private readonly ITypeMapper Mapper = mapper;

    /// <inheritdoc />
    public LearningRequirements Map(LearningRequirementsSchema obj)
        => new()
        {
            RequiredStats = obj.RequiredStats == null ? null : Mapper.Map<Stats>(obj.RequiredStats),
            PrerequisiteSkills = Mapper.MapMany<AbilityRequirement>(obj.PrerequisiteSkills)
                                       .ToList(),
            PrerequisiteSpells = Mapper.MapMany<AbilityRequirement>(obj.PrerequisiteSpells)
                                       .ToList(),
            ItemRequirements = Mapper.MapMany<ItemRequirement>(obj.ItemRequirements)
                                     .ToList(),
            RequiredGold = obj.RequiredGold
        };

    /// <inheritdoc />
    public AbilityRequirement Map(AbilityRequirementSchema obj)
        => new()
        {
            Level = obj.Level,
            TemplateKey = obj.TemplateKey
        };

    /// <inheritdoc />
    public AbilityRequirementSchema Map(AbilityRequirement obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public LearningRequirementsSchema Map(LearningRequirements obj) => throw new NotImplementedException();
}