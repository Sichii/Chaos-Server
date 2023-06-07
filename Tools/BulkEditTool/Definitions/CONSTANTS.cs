using System.Collections.Immutable;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;

namespace BulkEditTool.Definitions;

internal static class CONSTANTS
{
    internal const string TEMP_PATH = "NEW.json";

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

    internal static readonly ImmutableList<string> PropertyOrder = ImmutableList.Create(
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
        nameof(ItemTemplateSchema.Description));
}