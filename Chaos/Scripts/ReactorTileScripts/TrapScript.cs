using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts;

public class TrapScript : ConfigurableReactorTileScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private readonly Creature Owner;
    private readonly IIntervalTimer Timer;
    private int TriggerCount;
    protected Animation? Animation { get; init; }
    protected string? AppliedEffectKey { get; init; }
    protected int? Damage { get; init; }
    protected int DurationSecs { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected int MaxTriggers { get; init; }
    protected int Range { get; init; }
    protected AoeShape Shape { get; init; }
    protected byte? Sound { get; init; }

    /// <inheritdoc />
    public TrapScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""
{nameof(TrapScript)} script initialized fo {Subject} that has no owner. 
If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". 
If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{
    nameof(IReactorTileFactory.Create)}() call.
""");

        Owner = subject.Owner!;

        EffectFactory = effectFactory;
        TriggerCount = 0;
        Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs), false);
    }

    protected virtual void ApplyDamage(IEnumerable<Creature> targetEntities)
    {
        if (!Damage.HasValue)
            return;

        foreach (var target in targetEntities)
            ApplyDamageScripts.Default.ApplyDamage(
                Owner,
                target,
                this,
                Damage.Value);
    }

    protected virtual void ApplyEffect(IEnumerable<Creature> targetEntities)
    {
        if (string.IsNullOrEmpty(AppliedEffectKey))
            return;

        foreach (var target in targetEntities)
        {
            var effect = EffectFactory.Create(AppliedEffectKey);
            target.Effects.Apply(Owner, effect);
        }
    }

    protected virtual IEnumerable<T> GetAffectedEntities<T>(IEnumerable<IPoint> affectedPoints) where T: MapEntity
    {
        var entities = Map.GetEntitiesAtPoints<T>(affectedPoints);

        if (!Filter.HasValue)
            return entities;

        return entities.Where(entity => entity is not Creature creature || Filter.Value.IsValidTarget(Owner, creature));
    }

    protected virtual IEnumerable<Point> GetAffectedPoints() => Shape.ResolvePoints(Point, Range, includeSource: true);

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        //if the person who stepped on it isnt a valid target, do nothing
        if (Filter.HasValue && !Filter.Value.IsValidTarget(Owner, source))
            return;

        var affectedPoints = GetAffectedPoints().Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(affectedPoints).ToList();

        ShowAnimation(affectedPoints);
        PlaySound(affectedPoints);
        ApplyEffect(affectedEntities);
        ApplyDamage(affectedEntities);

        TriggerCount++;

        if (TriggerCount >= MaxTriggers)
            Map.RemoveObject(Subject);
    }

    protected virtual void PlaySound(ICollection<IPoint> affectedPoints)
    {
        if (!Sound.HasValue)
            return;

        Map.PlaySound(Sound.Value, affectedPoints);
    }

    protected virtual void ShowAnimation(IEnumerable<IPoint> affectedPoints)
    {
        if (Animation == null)
            return;

        foreach (var point in affectedPoints)
            Map.ShowAnimation(Animation.GetPointAnimation(point));
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            Map.RemoveObject(Subject);
    }
}