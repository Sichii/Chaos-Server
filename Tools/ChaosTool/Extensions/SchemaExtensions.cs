using Chaos.Schemas.Templates;

namespace ChaosTool.Extensions;

internal static class SchemaExtensions
{
    internal static IEnumerable<string?> EnumerateProperties(this DialogTemplateSchema schema)
    {
        yield return schema.TemplateKey;
        yield return schema.Type.ToString();
        yield return schema.Text;
        yield return schema.NextDialogKey;
        yield return schema.PrevDialogKey;
        yield return schema.Contextual.ToString();
        yield return schema.TextBoxLength?.ToString();
        yield return string.Join(", ", schema.Options.Select(opt => $"{opt.OptionText}/{opt.DialogKey}"));
        yield return string.Join(", ", schema.ScriptKeys);
    }

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
        yield return schema.ScriptKeys.ToLinePerString();
    }

    internal static IEnumerable<string?> EnumerateProperties(this SpellTemplateSchema schema)
    {
        var reqs = schema.LearningRequirements;
        var stats = reqs?.RequiredStats;

        yield return schema.TemplateKey;
        yield return schema.Name;
        yield return schema.SpellType.ToString();
        yield return schema.Prompt;
        yield return schema.CastLines.ToString();
        yield return schema.PanelSprite.ToString();
        yield return schema.Level.ToString();
        yield return schema.Class.ToString();
        yield return schema.AdvClass.ToString();
        yield return schema.RequiresMaster.ToString();
        yield return reqs?.RequiredGold.ToString();
        yield return stats?.Str.ToString();
        yield return stats?.Int.ToString();
        yield return stats?.Wis.ToString();
        yield return stats?.Con.ToString();
        yield return stats?.Dex.ToString();
        yield return schema.CooldownMs.ToString();
        yield return schema.Description;

        yield return reqs?.ItemRequirements.To2LinesPerItem(
            itemReqs => itemReqs.ItemTemplateKey,
            itemReqs => itemReqs.AmountRequired.ToString());

        yield return reqs?.PrerequisiteSkillTemplateKeys.ToLinePerString();
        yield return reqs?.PrerequisiteSpellTemplateKeys.ToLinePerString();
        yield return schema.ScriptKeys.ToLinePerString();
    }

    internal static IEnumerable<string?> EnumerateProperties(this SkillTemplateSchema schema)
    {
        var reqs = schema.LearningRequirements;
        var stats = reqs?.RequiredStats;

        yield return schema.TemplateKey;
        yield return schema.Name;
        yield return schema.IsAssail.ToString();
        yield return schema.PanelSprite.ToString();
        yield return schema.Level.ToString();
        yield return schema.Class.ToString();
        yield return schema.AdvClass.ToString();
        yield return schema.RequiresMaster.ToString();
        yield return reqs?.RequiredGold.ToString();
        yield return stats?.Str.ToString();
        yield return stats?.Int.ToString();
        yield return stats?.Wis.ToString();
        yield return stats?.Con.ToString();
        yield return stats?.Dex.ToString();
        yield return schema.CooldownMs.ToString();
        yield return schema.Description;

        yield return reqs?.ItemRequirements.To2LinesPerItem(
            itemReqs => itemReqs.ItemTemplateKey,
            itemReqs => itemReqs.AmountRequired.ToString());

        yield return reqs?.PrerequisiteSkillTemplateKeys.ToLinePerString();
        yield return reqs?.PrerequisiteSpellTemplateKeys.ToLinePerString();
        yield return schema.ScriptKeys.ToLinePerString();
    }

    internal static string To2LinesPerItem<T>(this IEnumerable<T> items, Func<T, string> selector1, Func<T, string> selector2) =>
        string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            items.Select(item => $"{selector1(item)}{Environment.NewLine}{selector2(item)}"));

    internal static string ToLinePerString(this IEnumerable<string> scriptKeys) => string.Join(Environment.NewLine, scriptKeys);
}