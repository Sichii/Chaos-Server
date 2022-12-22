using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.RuntimeScripts.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.NaturalRegeneration;

// ReSharper disable once ClassCanBeSealed.Global
public class DefaultNaturalRegenerationScript : ScriptBase, INaturalRegenerationScript
{
    private readonly IRegenFormula RegenFormula;

    public DefaultNaturalRegenerationScript(IRegenFormula regenFormula) => RegenFormula = regenFormula;

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