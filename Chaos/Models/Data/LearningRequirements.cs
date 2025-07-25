#region
using System.Text;
using Chaos.Extensions.Common;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Models.Data;

public sealed record LearningRequirements
{
    /// <summary>
    ///     The items and their amounts required to learn this ability
    /// </summary>
    public required ICollection<ItemRequirement> ItemRequirements { get; init; } = [];

    /// <summary>
    ///     The skills that must be learned before this ability can be learned
    /// </summary>
    public required ICollection<AbilityRequirement> PrerequisiteSkills { get; init; } = [];

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public required ICollection<AbilityRequirement> PrerequisiteSpells { get; init; } = [];

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public required int? RequiredGold { get; init; }

    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public required Stats? RequiredStats { get; init; }

    public StringBuilder BuildRequirementsString(IItemFactory itemFactory, ISkillFactory skillFactory, ISpellFactory spellFactory)
    {
        var builder = new StringBuilder();

        if (RequiredStats != null)
            builder.AppendLine(
                $"Required Attributes: STR: {RequiredStats.Str}, INT: {RequiredStats.Int}, WIS: {RequiredStats.Wis}, CON: {RequiredStats.Con
                }, DEX: {RequiredStats.Dex}");

        if (RequiredGold.HasValue)
            builder.AppendLine($"Required Gold: {RequiredGold} gold");

        builder.AppendLine();

        var max = Math.Max(PrerequisiteSkills.Count, PrerequisiteSpells.Count);
        max = Math.Max(max, ItemRequirements.Count);

        var skillStrs = Enumerable.Range(0, max)
                                  .Select(_ => string.Empty)
                                  .ToList();

        var spellStrs = Enumerable.Range(0, max)
                                  .Select(_ => string.Empty)
                                  .ToList();

        var itemStrs = Enumerable.Range(0, max)
                                 .Select(_ => string.Empty)
                                 .ToList();

        for (var i = 0; i < PrerequisiteSkills.Count; i++)
        {
            var req = PrerequisiteSkills.ElementAt(i);
            var skill = skillFactory.CreateFaux(req.TemplateKey);
            skillStrs[i] = $"{skill.Template.Name} {req.Level ?? skill.Template.MaxLevel}";
        }

        for (var i = 0; i < PrerequisiteSpells.Count; i++)
        {
            var req = PrerequisiteSpells.ElementAt(i);
            var spell = spellFactory.CreateFaux(req.TemplateKey);
            spellStrs[i] = $"{spell.Template.Name} {req.Level ?? spell.Template.MaxLevel}";
        }

        for (var i = 0; i < ItemRequirements.Count; i++)
        {
            var requiredItem = ItemRequirements.ElementAt(i);
            var item = itemFactory.CreateFaux(requiredItem.ItemTemplateKey);
            itemStrs[i] = $"{requiredItem.AmountRequired}x {item.DisplayName}(s)";
        }

        builder.AppendLine($"{"Required Items".CenterAlign(31)}|{"Required Skills".CenterAlign(31)}|{"Required Spells".CenterAlign(31)}");

        for (var i = 0; i < max; i++)
            builder.AppendLine($"{itemStrs[i].CenterAlign(31)}|{skillStrs[i].CenterAlign(31)}|{spellStrs[i].CenterAlign(31)}");

        return builder;
    }
}