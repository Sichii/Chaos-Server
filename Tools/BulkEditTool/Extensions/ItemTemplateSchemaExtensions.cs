using Chaos.Schemas.Templates;

namespace BulkEditTool.Extensions;

internal static class ItemTemplateSchemaExtensions
{
    internal static IEnumerable<string?> EnumerateProperties(this ItemTemplateSchema schema)
    {
        var modifiers = schema.Modifiers;

        yield return schema.TemplateKey;
        yield return schema.Name;
        yield return schema.PanelSprite.ToString();
        yield return schema.DisplaySprite?.ToString();
        yield return schema.Color.ToString();
        yield return schema.PantsColor?.ToString();
        yield return schema.MaxStacks.ToString();
        yield return schema.AccountBound.ToString();
        yield return schema.BuyCost.ToString();
        yield return schema.SellValue.ToString();
        yield return schema.Weight.ToString();
        yield return schema.MaxDurability?.ToString();
        yield return schema.Class?.ToString();
        yield return schema.AdvClass?.ToString();
        yield return schema.Level.ToString();
        yield return schema.RequiresMaster.ToString();
        yield return modifiers?.AtkSpeedPct.ToString();
        yield return modifiers?.Ac.ToString();
        yield return modifiers?.MagicResistance.ToString();
        yield return modifiers?.Hit.ToString();
        yield return modifiers?.Dmg.ToString();
        yield return modifiers?.FlatSkillDamage.ToString();
        yield return modifiers?.FlatSpellDamage.ToString();
        yield return modifiers?.SkillDamagePct.ToString();
        yield return modifiers?.SpellDamagePct.ToString();
        yield return modifiers?.MaximumHp.ToString();
        yield return modifiers?.MaximumMp.ToString();
        yield return modifiers?.Str.ToString();
        yield return modifiers?.Int.ToString();
        yield return modifiers?.Wis.ToString();
        yield return modifiers?.Con.ToString();
        yield return modifiers?.Dex.ToString();
        yield return schema.CooldownMs?.ToString();
        yield return schema.EquipmentType?.ToString();
        yield return schema.Gender?.ToString();
        yield return schema.IsDyeable.ToString();
        yield return schema.IsModifiable.ToString();
        yield return schema.Category;
        yield return schema.Description;
    }
}