using System.Text;
using AutoMapper;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Site.Extensions;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services.MapperProfiles;

public sealed class SkillMapperProfile : Profile
{
    private readonly ISimpleCache Cache;

    public SkillMapperProfile(ISimpleCache cache)
    {
        Cache = cache;

        CreateMap<Stats, SkillDto>()
            .ReverseMap();

        CreateMap<LearningRequirements, SkillDto>()
            .ForMember(rhs => rhs.Gold, opt => opt.MapFrom(lhs => lhs.RequiredGold))
            .ForMember(rhs => rhs.ItemRequirements, opt => opt.MapFrom<string>(lhs => FlattenItemRequirements(lhs.ItemRequirements)))
            .ForMember(rhs => rhs.SkillRequirements, opt => opt.MapFrom<string>(lhs => FlattenSkillRequirements(lhs.PrerequisiteSkills)))
            .ForMember(rhs => rhs.SpellRequirements, opt => opt.MapFrom<string>(lhs => FlattenSpellRequirements(lhs.PrerequisiteSpells)))
            .IncludeNullableMember(rhs => rhs.RequiredStats)
            .ReverseMap();

        CreateMap<SkillTemplate, SkillDto>()
            .IncludeNullableMember(lhs => lhs.LearningRequirements)
            .ReverseMap();
    }

    public string FlattenItemRequirements(IEnumerable<ItemRequirement> itemRequirements)
    {
        var builder = new StringBuilder();

        foreach (var req in itemRequirements)
        {
            var template = Cache.Get<ItemTemplate>(req.ItemTemplateKey);
            builder.AppendLine($"{template.Name}: {req.AmountRequired}");
        }

        return builder.ToString();
    }

    public string FlattenSkillRequirements(IEnumerable<AbilityRequirement> skillRequirements)
    {
        var builder = new StringBuilder();

        foreach (var req in skillRequirements)
        {
            var template = Cache.Get<SkillTemplate>(req.TemplateKey);
            builder.AppendLine($"{template.Name}: {req.Level?.ToString() ?? "N/A"}");
        }

        return builder.ToString();
    }

    public string FlattenSpellRequirements(IEnumerable<AbilityRequirement> spellRequirements)
    {
        var builder = new StringBuilder();

        foreach (var req in spellRequirements)
        {
            var template = Cache.Get<SpellTemplate>(req.TemplateKey);
            builder.AppendLine($"{template.Name}: {req.Level?.ToString() ?? "N/A"}");
        }

        return builder.ToString();
    }
}