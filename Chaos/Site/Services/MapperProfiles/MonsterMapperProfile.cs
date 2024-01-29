using System.Text;
using AutoMapper;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services.MapperProfiles;

public class MonsterMapperProfile : Profile
{
    private readonly ISimpleCache SimpleCache;

    public MonsterMapperProfile(ISimpleCache simpleCache)
    {
        SimpleCache = simpleCache;

        CreateMap<StatSheet, MonsterDto>()
            .ReverseMap();

        CreateMap<MonsterTemplate, MonsterDto>()
            .ForMember(rhs => rhs.Drops, opt => opt.MapFrom(lhs => StringifyDrops(lhs.LootTables)))
            .ForMember(rhs => rhs.Skills, opt => opt.MapFrom(lhs => GetSkillNames(lhs.SkillTemplateKeys)))
            .ForMember(rhs => rhs.Spells, opt => opt.MapFrom(lhs => GetSpellNames(lhs.SpellTemplateKeys)))
            .IncludeMembers(lhs => lhs.StatSheet);
    }

    private string GetSkillNames(IEnumerable<string> skillTemplateKeys)
    {
        var builder = new StringBuilder();

        foreach (var skillTemplateKey in skillTemplateKeys)
        {
            var skillTemplate = SimpleCache.Get<SkillTemplate>(skillTemplateKey);

            builder.AppendLine(skillTemplate.Name);
        }

        return builder.ToString();
    }

    private string GetSpellNames(IEnumerable<string> spellTemplateKeys)
    {
        var builder = new StringBuilder();

        foreach (var spellTemplateKey in spellTemplateKeys)
        {
            var spellTemplate = SimpleCache.Get<SpellTemplate>(spellTemplateKey);

            builder.AppendLine(spellTemplate.Name);
        }

        return builder.ToString();
    }

    private string StringifyDrops(IEnumerable<LootTable> lootTables)
    {
        var builder = new StringBuilder();

        var tableBuilders = lootTables.Select(
            table =>
            {
                var subBuilder = new StringBuilder();

                switch (table.Mode)
                {
                    case LootTableMode.ChancePerItem:
                        subBuilder.AppendLine("Any number of the following");
                        subBuilder.AppendLine("---------------------------");

                        break;
                    case LootTableMode.PickSingleOrDefault:
                        subBuilder.AppendLine("Up to 1 of the following");
                        subBuilder.AppendLine("---------------------------");

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var drop in table.LootDrops)
                {
                    var itemTemplate = SimpleCache.Get<ItemTemplate>(drop.ItemTemplateKey);

                    subBuilder.AppendLine($"{itemTemplate.Name} ({drop.DropChance:P0})");
                }

                return subBuilder;
            });

        var index = 0;

        foreach (var tableBuilder in tableBuilders)
        {
            if (index++ > 0)
            {
                builder.AppendLine();
                builder.AppendLine();
            }

            builder.Append(tableBuilder);
        }

        return builder.ToString();
    }
}