using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.NaturalRegeneration;

// ReSharper disable once ClassCanBeSealed.Global
public class DefaultNaturalRegenerationScript : ScriptBase, INaturalRegenerationScript
{
    public IRegenFormula RegenFormula { get; set; } = RegenFormulae.Default;

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultNaturalRegenerationScript));

    /// <inheritdoc />
    public static INaturalRegenerationScript Create() => FunctionalScriptRegistry.Instance.Get<INaturalRegenerationScript>(Key);

    /// <inheritdoc />
    public void Regenerate(Creature creature)
    {
        if (!creature.IsAlive)
            return;

        var hpToRegen = RegenFormula.CalculateHealthRegen(creature);
        var mpToRegen = RegenFormula.CalculateManaRegen(creature);

        if (hpToRegen != 0)
            creature.StatSheet.AddHp(hpToRegen);

        if (mpToRegen != 0)
            creature.StatSheet.AddMp(mpToRegen);

        if (creature is Aisling aisling && ((hpToRegen != 0) || (mpToRegen != 0)))
            aisling.Client.SendAttributes(StatUpdateType.Vitality);
    }
}