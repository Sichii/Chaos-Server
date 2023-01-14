using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.Components;

public class CascadeAbilityComponent
{
    public virtual void Activate(ActivationContext context, CascadeAbilityComponentOptions options, IEffectFactory effectFactory)
    {
        if (options.BodyAnimation.HasValue)
            context.Source.AnimateBody(options.BodyAnimation.Value);

        var allPoints = options.Shape.ResolvePoints(
                                   context.TargetPoint,
                                   options.Range,
                                   context.Target.Direction,
                                   null,
                                   options.IncludeSourcePoint)
                               .Cast<IPoint>();

        if (options.StopAtWalls)
            allPoints = allPoints.FilterByLineOfSight(context.SourcePoint, context.Map);

        var cascadingProxy = new CascadingProxy(
            context,
            options,
            allPoints.ToList(),
            effectFactory);

        var proxyEntity = new ProxyEntity(context.Map, context.TargetPoint, cascadingProxy);
        context.Map.SimpleAdd(proxyEntity);
    }

    public class CascadeProxyOptions
    {
        public bool AnimatePoints { get; init; }
        public Animation? Animation { get; init; }
        public DamageComponent.DamageComponentOptions? DamageComponentOptions { get; init; }
        public string? EffectKey { get; init; }
        public TargetFilter? Filter { get; init; }
        public required int PropagationIntervalMs { get; init; }
        public required int Range { get; init; }
        public required AoeShape Shape { get; init; }
        public byte? Sound { get; init; }
        public required int SoundIntervalMs { get; init; }
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public class CascadeAbilityComponentOptions : CascadeProxyOptions
    {
        public BodyAnimation? BodyAnimation { get; init; }
        public bool IncludeSourcePoint { get; init; }
        public bool StopAtWalls { get; init; }
    }

    public sealed class CascadingProxy : ProxyEntity.IProxy
    {
        /// <inheritdoc />
        public MapEntity Entity { get; set; } = null!;
        public int Stage { get; set; }
        public IReadOnlyCollection<IPoint> AllPoints { get; }
        public ActivationContext Context { get; }
        public DamageComponent? DamageComponent { get; }
        public Direction Direction { get; }
        public IEffectFactory EffectFactory { get; }
        public CascadeProxyOptions Options { get; }
        public IIntervalTimer PropagationTimer { get; }
        public IIntervalTimer SoundTimer { get; }

        public CascadingProxy(
            ActivationContext context,
            CascadeProxyOptions options,
            IReadOnlyCollection<IPoint> allPoints,
            IEffectFactory effectFactory
        )
        {
            Context = context;
            Direction = context.Target.Direction;
            Options = options;
            AllPoints = allPoints;
            EffectFactory = effectFactory;
            PropagationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(options.PropagationIntervalMs));
            SoundTimer = new IntervalTimer(TimeSpan.FromMilliseconds(options.SoundIntervalMs));
            Stage = 0;

            if (options.DamageComponentOptions is not null)
                DamageComponent = new DamageComponent();
        }

        /// <inheritdoc />
        public void Update(TimeSpan delta)
        {
            PropagationTimer.Update(delta);

            if (!SoundTimer.IntervalElapsed)
                SoundTimer.Update(delta);

            SoundTimer.Update(delta);

            if (PropagationTimer.IntervalElapsed)
            {
                Stage++;

                if (Stage > Options.Range)
                {
                    Entity.MapInstance.RemoveObject(Entity);

                    return;
                }

                var targetPoints = Options.Shape.ResolvePointsForRange(
                                              Entity,
                                              Direction,
                                              Stage,
                                              AllPoints)
                                          .ToList();

                var targetEntities = Context.Map.GetEntitiesAtPoints<Creature>(targetPoints)
                                            .ToList();

                if (Options.Animation != null)
                {
                    if (Options.AnimatePoints)
                        foreach (var point in targetPoints)
                            Context.Map.ShowAnimation(Options.Animation.GetPointAnimation(point, Context.Source.Id));
                    else
                        foreach (var entity in targetEntities)
                            entity.Animate(Options.Animation, Context.Source.Id);
                }

                DamageComponent?.ApplyDamage(Context, targetEntities, Options.DamageComponentOptions!);

                if (!string.IsNullOrEmpty(Options.EffectKey))
                    foreach (var entity in targetEntities)
                    {
                        var effect = EffectFactory.Create(Options.EffectKey);
                        entity.Effects.Apply(Context.Source, effect);
                    }

                if (Options.Sound.HasValue && SoundTimer.IntervalElapsed)
                    Context.Map.PlaySound(Options.Sound.Value, targetPoints);
            }
        }
    }
}