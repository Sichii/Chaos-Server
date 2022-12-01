using System.Text;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Data;

public sealed record LearningRequirements
{
    /// <summary>
    ///     The items and their amounts required to learn this ability
    /// </summary>
    public required ICollection<ItemRequirement> ItemRequirements { get; init; } = Array.Empty<ItemRequirement>();

    /// <summary>
    ///     The skills that must be learned before this ability can be learned
    /// </summary>
    public required ICollection<string> PrerequisiteSkillTemplateKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public required ICollection<string> PrerequisiteSpellTemplateKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     The class required to learn this ability
    /// </summary>
    public required BaseClass? RequiredClass { get; init; }

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public required int? RequiredGold { get; init; }

    /// <summary>
    ///     The level required to learn this ability
    /// </summary>
    public required int? RequiredLevel { get; init; }
    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public required Stats? RequiredStats { get; init; }

    public StringBuilder BuildRequirementsString(
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
    {
        var builder = new StringBuilder();

        if (RequiredLevel.HasValue)
            builder.AppendLine($"Required Level: Level {RequiredLevel}");

        if (RequiredStats != null)
            builder.AppendLine(
                $"Required Attributes: STR: {RequiredStats.Str}, INT: {RequiredStats.Int}, WIS: {RequiredStats.Wis}, CON: {RequiredStats.Con}, DEX: {RequiredStats.Dex}");

        if (RequiredGold.HasValue)
            builder.AppendLine($"Required Gold: {RequiredGold} gold");

        builder.AppendLine();

        var max = Math.Max(PrerequisiteSkillTemplateKeys.Count, PrerequisiteSpellTemplateKeys.Count);
        max = Math.Max(max, ItemRequirements.Count);

        var skillStrs = Enumerable.Range(0, max).Select(_ => string.Empty).ToList();
        var spellStrs = Enumerable.Range(0, max).Select(_ => string.Empty).ToList();
        var itemStrs = Enumerable.Range(0, max).Select(_ => string.Empty).ToList();

        for (var i = 0; i < PrerequisiteSkillTemplateKeys.Count; i++)
        {
            var skill = skillFactory.CreateFaux(PrerequisiteSkillTemplateKeys.ElementAt(i));
            skillStrs[i] = skill.Template.Name;
        }

        for (var i = 0; i < PrerequisiteSpellTemplateKeys.Count; i++)
        {
            var spell = spellFactory.CreateFaux(PrerequisiteSpellTemplateKeys.ElementAt(i));
            spellStrs[i] = spell.Template.Name;
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