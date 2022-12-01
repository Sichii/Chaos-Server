using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateIntervalSecs(Aisling aisling) => 6;

    /// <inheritdoc />
    public void Regenerate(Aisling aisling)
    {
        if (!aisling.IsAlive)
            return;

        aisling.StatSheet.AddHealthPct(10);
        aisling.StatSheet.AddManaPct(5);
        aisling.Client.SendAttributes(StatUpdateType.Vitality);
    }
}