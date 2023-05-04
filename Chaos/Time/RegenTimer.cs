using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Time;

public sealed class RegenTimer : IntervalTimer
{
    private readonly Creature Creature;
    private readonly INaturalRegenerationScript NaturalRegenerationScript;

    /// <inheritdoc />
    public RegenTimer(Creature creature, INaturalRegenerationScript naturalRegenerationScript)
        : base(TimeSpan.Zero, false)
    {
        Creature = creature;
        NaturalRegenerationScript = naturalRegenerationScript;
        Interval = CalculateInterval();
    }

    private TimeSpan CalculateInterval() =>
        TimeSpan.FromSeconds(NaturalRegenerationScript.RegenFormula.CalculateIntervalSecs(Creature));

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (IntervalElapsed)
        {
            NaturalRegenerationScript.Regenerate(Creature);
            Interval = CalculateInterval();
        }
    }
}