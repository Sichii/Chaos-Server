using Chaos.Formulae;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.RuntimeScripts;

namespace Chaos.Time;

public sealed class RegenTimer : IntervalTimer
{
    private readonly Creature Creature;

    /// <inheritdoc />
    public RegenTimer(Creature creature)
        : base(CalculateInterval(creature), false) =>
        Creature = creature;

    private TimeSpan CalculateInterval() =>
        TimeSpan.FromSeconds(RegenFormulae.Default.CalculateIntervalSecs(Creature));

    private static TimeSpan CalculateInterval(Creature creature) =>
        TimeSpan.FromSeconds(RegenFormulae.Default.CalculateIntervalSecs(creature));

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (IntervalElapsed)
        {
            NaturalRegenerationScripts.Default.Regenerate(Creature);
            Interval = CalculateInterval();
        }
    }
}