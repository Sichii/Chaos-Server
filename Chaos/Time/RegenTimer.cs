using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Time;

public sealed class RegenTimer : IntervalTimer
{
    private readonly Aisling Aisling;
    private readonly IRegenFormula RegenFormula;

    /// <inheritdoc />
    public RegenTimer(Aisling aisling, IRegenFormula regenFormula)
        : base(TimeSpan.FromSeconds(regenFormula.CalculateIntervalSecs(aisling)), false)
    {
        Aisling = aisling;
        RegenFormula = regenFormula;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (IntervalElapsed)
        {
            RegenFormula.Regenerate(Aisling);
            Interval = TimeSpan.FromSeconds(RegenFormula.CalculateIntervalSecs(Aisling));
        }
    }
}