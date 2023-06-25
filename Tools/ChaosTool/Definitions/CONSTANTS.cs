using System.Collections.Immutable;
using Chaos.Models.Data;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;

namespace ChaosTool.Definitions;

internal static class CONSTANTS
{
    internal const string TEMP_PATH = "NEW.json";

    internal static readonly ImmutableList<string> MerchantTemplatePropertyOrder = ImmutableList.Create(
        nameof(MerchantTemplateSchema.TemplateKey),
        nameof(MerchantTemplateSchema.Name),
        nameof(MerchantTemplateSchema.Sprite),
        nameof(MerchantTemplateSchema.RestockPct),
        nameof(MerchantTemplateSchema.RestockIntervalHrs),
        nameof(MerchantTemplateSchema.WanderIntervalMs),
        nameof(MerchantTemplateSchema.ItemsForSale),
        nameof(MerchantTemplateSchema.ItemsToBuy),
        nameof(MerchantTemplateSchema.SkillsToTeach),
        nameof(MerchantTemplateSchema.SpellsToTeach),
        nameof(MerchantTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> MonsterTemplatePropertyOrder = ImmutableList.Create(
        nameof(MonsterTemplateSchema.TemplateKey),
        nameof(MonsterTemplateSchema.Name),
        nameof(MonsterTemplateSchema.Sprite),
        nameof(MonsterTemplateSchema.Type),
        nameof(MonsterTemplateSchema.AggroRange),
        nameof(MonsterTemplateSchema.ExpReward),
        nameof(MonsterTemplateSchema.MinGoldDrop),
        nameof(MonsterTemplateSchema.MaxGoldDrop),
        nameof(MonsterTemplateSchema.AssailIntervalMs),
        nameof(MonsterTemplateSchema.SkillIntervalMs),
        nameof(MonsterTemplateSchema.SpellIntervalMs),
        nameof(MonsterTemplateSchema.MoveIntervalMs),
        nameof(MonsterTemplateSchema.WanderIntervalMs),
        nameof(StatSheetSchema.Ability),
        nameof(StatSheetSchema.Level),
        nameof(StatSheetSchema.AtkSpeedPct),
        nameof(StatSheetSchema.Ac),
        nameof(StatSheetSchema.MagicResistance),
        nameof(StatSheetSchema.Hit),
        nameof(StatSheetSchema.Dmg),
        nameof(StatSheetSchema.FlatSkillDamage),
        nameof(StatSheetSchema.FlatSpellDamage),
        nameof(StatSheetSchema.SkillDamagePct),
        nameof(StatSheetSchema.SpellDamagePct),
        nameof(StatSheetSchema.MaximumHp),
        nameof(StatSheetSchema.MaximumMp),
        nameof(StatSheetSchema.Str),
        nameof(StatSheetSchema.Int),
        nameof(StatSheetSchema.Wis),
        nameof(StatSheetSchema.Con),
        nameof(StatSheetSchema.Dex),
        nameof(MonsterTemplateSchema.SpellTemplateKeys),
        nameof(MonsterTemplateSchema.SkillTemplateKeys),
        nameof(MonsterTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> MapInstancePropertyOrder = ImmutableList.Create("TODO");

    internal static readonly ImmutableList<string> ReactorTileTemplatePropertyOrder = ImmutableList.Create(
        nameof(ReactorTileTemplateSchema.TemplateKey),
        nameof(ReactorTileTemplateSchema.ShouldBlockPathfinding),
        nameof(ReactorTileTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> MapTemplatePropertyOrder = ImmutableList.Create(
        nameof(MapTemplateSchema.TemplateKey),
        nameof(MapTemplateSchema.Width),
        nameof(MapTemplateSchema.Height),
        nameof(MapTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> DialogTemplatePropertyOrder = ImmutableList.Create(
        nameof(DialogTemplateSchema.TemplateKey),
        nameof(DialogTemplateSchema.Type),
        nameof(DialogTemplateSchema.Text),
        nameof(DialogTemplateSchema.NextDialogKey),
        nameof(DialogTemplateSchema.PrevDialogKey),
        nameof(DialogTemplateSchema.Contextual),
        nameof(DialogTemplateSchema.TextBoxLength),
        nameof(DialogTemplateSchema.Options),
        nameof(DialogTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> ItemTemplatePropertyOrder = ImmutableList.Create(
        nameof(ItemTemplateSchema.TemplateKey),
        nameof(ItemTemplateSchema.Name),
        nameof(ItemTemplateSchema.PanelSprite),
        nameof(ItemTemplateSchema.DisplaySprite),
        nameof(ItemTemplateSchema.Color),
        nameof(ItemTemplateSchema.PantsColor),
        nameof(ItemTemplateSchema.MaxStacks),
        nameof(ItemTemplateSchema.AccountBound),
        nameof(ItemTemplateSchema.BuyCost),
        nameof(ItemTemplateSchema.SellValue),
        nameof(ItemTemplateSchema.Weight),
        nameof(ItemTemplateSchema.MaxDurability),
        nameof(ItemTemplateSchema.Class),
        nameof(ItemTemplateSchema.AdvClass),
        nameof(ItemTemplateSchema.Level),
        nameof(ItemTemplateSchema.RequiresMaster),
        nameof(AttributesSchema.AtkSpeedPct),
        nameof(AttributesSchema.Ac),
        nameof(AttributesSchema.MagicResistance),
        nameof(AttributesSchema.Hit),
        nameof(AttributesSchema.Dmg),
        nameof(AttributesSchema.FlatSkillDamage),
        nameof(AttributesSchema.FlatSpellDamage),
        nameof(AttributesSchema.SkillDamagePct),
        nameof(AttributesSchema.SpellDamagePct),
        nameof(AttributesSchema.MaximumHp),
        nameof(AttributesSchema.MaximumMp),
        nameof(AttributesSchema.Str),
        nameof(AttributesSchema.Int),
        nameof(AttributesSchema.Wis),
        nameof(AttributesSchema.Con),
        nameof(AttributesSchema.Dex),
        nameof(ItemTemplateSchema.CooldownMs),
        nameof(ItemTemplateSchema.EquipmentType),
        nameof(ItemTemplateSchema.Gender),
        nameof(ItemTemplateSchema.IsDyeable),
        nameof(ItemTemplateSchema.IsModifiable),
        nameof(ItemTemplateSchema.Category),
        nameof(ItemTemplateSchema.Description),
        nameof(ItemTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> LearningRequirementProperties = ImmutableList.Create(
        nameof(LearningRequirementsSchema.RequiredGold),
        nameof(LearningRequirementsSchema.ItemRequirements),
        nameof(LearningRequirementsSchema.PrerequisiteSkillTemplateKeys),
        nameof(LearningRequirementsSchema.PrerequisiteSpellTemplateKeys));

    internal static readonly ImmutableList<string> ModifierProperties = ImmutableList.Create(
        nameof(AttributesSchema.AtkSpeedPct),
        nameof(AttributesSchema.Ac),
        nameof(AttributesSchema.MagicResistance),
        nameof(AttributesSchema.Hit),
        nameof(AttributesSchema.Dmg),
        nameof(AttributesSchema.FlatSkillDamage),
        nameof(AttributesSchema.FlatSpellDamage),
        nameof(AttributesSchema.SkillDamagePct),
        nameof(AttributesSchema.SpellDamagePct),
        nameof(AttributesSchema.MaximumHp),
        nameof(AttributesSchema.MaximumMp),
        nameof(AttributesSchema.Str),
        nameof(AttributesSchema.Int),
        nameof(AttributesSchema.Wis),
        nameof(AttributesSchema.Con),
        nameof(AttributesSchema.Dex));

    internal static readonly ImmutableList<string> StatSheetProperties = ImmutableList.Create(
        nameof(StatSheetSchema.Ability),
        nameof(StatSheetSchema.Level),
        nameof(StatSheetSchema.AtkSpeedPct),
        nameof(StatSheetSchema.Ac),
        nameof(StatSheetSchema.MagicResistance),
        nameof(StatSheetSchema.Hit),
        nameof(StatSheetSchema.Dmg),
        nameof(StatSheetSchema.FlatSkillDamage),
        nameof(StatSheetSchema.FlatSpellDamage),
        nameof(StatSheetSchema.SkillDamagePct),
        nameof(StatSheetSchema.SpellDamagePct),
        nameof(StatSheetSchema.MaximumHp),
        nameof(StatSheetSchema.MaximumMp),
        nameof(StatSheetSchema.Str),
        nameof(StatSheetSchema.Int),
        nameof(StatSheetSchema.Wis),
        nameof(StatSheetSchema.Con),
        nameof(StatSheetSchema.Dex));

    internal static readonly ImmutableList<string> SkillTemplatePropertyOrder = ImmutableList.Create(
        nameof(SkillTemplateSchema.TemplateKey),
        nameof(SkillTemplateSchema.Name),
        nameof(SkillTemplateSchema.IsAssail),
        nameof(SkillTemplateSchema.PanelSprite),
        nameof(SkillTemplateSchema.Level),
        nameof(SkillTemplateSchema.Class),
        nameof(SkillTemplateSchema.AdvClass),
        nameof(SkillTemplateSchema.RequiresMaster),
        nameof(LearningRequirements.RequiredGold),
        nameof(StatsSchema.Str),
        nameof(StatsSchema.Int),
        nameof(StatsSchema.Wis),
        nameof(StatsSchema.Con),
        nameof(StatsSchema.Dex),
        nameof(SkillTemplateSchema.CooldownMs),
        nameof(SkillTemplateSchema.Description),
        nameof(LearningRequirements.ItemRequirements),
        nameof(LearningRequirements.PrerequisiteSkillTemplateKeys),
        nameof(LearningRequirements.PrerequisiteSpellTemplateKeys),
        nameof(SkillTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> SpellTemplatePropertyOrder = ImmutableList.Create(
        nameof(SpellTemplateSchema.TemplateKey),
        nameof(SpellTemplateSchema.Name),
        nameof(SpellTemplateSchema.SpellType),
        nameof(SpellTemplateSchema.Prompt),
        nameof(SpellTemplateSchema.CastLines),
        nameof(SpellTemplateSchema.PanelSprite),
        nameof(SpellTemplateSchema.Level),
        nameof(SpellTemplateSchema.Class),
        nameof(SpellTemplateSchema.AdvClass),
        nameof(SpellTemplateSchema.RequiresMaster),
        nameof(LearningRequirements.RequiredGold),
        nameof(StatsSchema.Str),
        nameof(StatsSchema.Int),
        nameof(StatsSchema.Wis),
        nameof(StatsSchema.Con),
        nameof(StatsSchema.Dex),
        nameof(SpellTemplateSchema.CooldownMs),
        nameof(SpellTemplateSchema.Description),
        nameof(LearningRequirements.ItemRequirements),
        nameof(LearningRequirements.PrerequisiteSkillTemplateKeys),
        nameof(LearningRequirements.PrerequisiteSpellTemplateKeys),
        nameof(SpellTemplateSchema.ScriptKeys));

    internal static readonly ImmutableList<string> StatProperties = ImmutableList.Create(
        nameof(StatsSchema.Str),
        nameof(StatsSchema.Int),
        nameof(StatsSchema.Wis),
        nameof(StatsSchema.Con),
        nameof(StatsSchema.Dex));
}