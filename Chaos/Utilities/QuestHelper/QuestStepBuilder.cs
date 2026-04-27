#region
using System.Runtime.CompilerServices;
using Chaos.Collections;
using Chaos.Common.CustomTypes;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.AbilityDistribution;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
#endregion

namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Fluent builder for an operation chain. Recorded operations are functions over
/// QuestContext returning bool — true = continue the chain, false = halt (guard failed).
/// </summary>
/// <remarks>
/// <para>
/// Most guard methods (<c>When</c>, <c>WhenNeverStarted</c>, <c>RequireFlag</c>,
/// <c>RequireItem</c>, etc.) take an optional <c>failureReply</c> parameter. When provided,
/// a failed guard sends a Dialog Reply with the message and halts; when omitted, the guard
/// halts silently.
/// </para>
/// </remarks>
public sealed class QuestStepBuilder<TStage> where TStage : struct, Enum
{
    private readonly List<Func<QuestContext, bool>> Operations = [];

    /// <summary>
    /// Returns the live mutable backing list (typed as IReadOnlyList for the consumer).
    /// This aliasing is intentional: QuestBuilder calls Build() while the chain is still
    /// being constructed, so DialogQuestHandler must observe operations appended after
    /// registration. Do not change this to ToImmutableList() / ToArray() / similar —
    /// every operation appended via Append/AppendGuard would silently disappear from the
    /// handler.
    /// </summary>
    internal IReadOnlyList<Func<QuestContext, bool>> Build() => Operations;

    /// <summary>
    /// Append an operation that always continues. Used by action-only methods (Advance, GiveItem, etc.).
    /// </summary>
    internal QuestStepBuilder<TStage> Append(Action<QuestContext<TStage>> action)
    {
        Operations.Add(ctx =>
        {
            action((QuestContext<TStage>)ctx);
            return true;
        });

        return this;
    }

    /// <summary>
    /// Append a guard. The chain halts if the predicate returns false. When
    /// <paramref name="failureReply" /> is non-null, the failure path also sends a Dialog
    /// Reply with the provided message before halting.
    /// </summary>
    internal QuestStepBuilder<TStage> AppendGuard(
        Func<QuestContext<TStage>, bool> predicate,
        QuestArg<string>? failureReply = null)
    {
        if (failureReply is null)
        {
            Operations.Add(ctx => predicate((QuestContext<TStage>)ctx));

            return this;
        }

        var arg = failureReply.Value;

        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (predicate(typed))
                return true;

            typed.Subject?.Reply(typed.Source, arg.Resolve(typed));

            return false;
        });

        return this;
    }

    // ===== Stage operations =====

    /// <summary>
    /// Halt the chain unless Source is currently at the given stage. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenAt(TStage stage, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.WhenAt(stage), failureReply);

    /// <summary>Halt the chain unless the source's tracked stage is any of <paramref name="stages" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> WhenAtAny(params IReadOnlyCollection<TStage> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages));

    /// <summary>Per-execution overload — <paramref name="stages" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> WhenAtAny(QuestArg<IReadOnlyCollection<TStage>> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages.Resolve(ctx)));

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source's tracked stage is any of <paramref name="stages" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> WhenAtAny(QuestArg<string> failureReply, params IReadOnlyCollection<TStage> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages), failureReply);

    /// <summary>Per-execution overload — <paramref name="stages" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> WhenAtAny(QuestArg<string> failureReply, QuestArg<IReadOnlyCollection<TStage>> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages.Resolve(ctx)), failureReply);

    /// <summary>Set the stage. Updates ctx.CurrentStage to match.</summary>
    public QuestStepBuilder<TStage> Advance(TStage stage)
        => Append(ctx =>
        {
            ctx.Source.Trackers.Enums.Set(stage);
            ctx.CurrentStage = stage;
        });

    /// <summary>Remove the stage entirely.</summary>
    public QuestStepBuilder<TStage> ClearStage()
        => Append(ctx =>
        {
            ctx.Source.Trackers.Enums.Remove<TStage>();
            ctx.CurrentStage = default;
        });

    /// <summary>
    /// Halt the chain unless Source's <c>Trackers.Enums</c> has no value stored for
    /// <typeparamref name="TStage" /> — i.e., the player has never advanced into any stage of
    /// this quest. When <paramref name="failureReply" /> is provided, the failure path also
    /// sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenNeverStarted(QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.NeverStarted, failureReply);

    /// <summary>
    /// Companion to a chain of conditional sub-builder calls. Runs the sub-chain only if no
    /// preceding <see cref="Branch" /> matched.
    /// </summary>
    public QuestStepBuilder<TStage> Otherwise(Action<QuestStepBuilder<TStage>> configure)
    {
        var sub = new QuestStepBuilder<TStage>();
        configure(sub);
        var subOps = sub.Build();

        Operations.Add(ctx =>
        {
            if (ctx.OtherwiseTaken)
                return true;

            foreach (var op in subOps)
                if (!op(ctx))
                    return true;

            return true;
        });

        return this;
    }

    /// <summary>
    /// Maps the current stage to a dialog key and Skips to it (no-op if stage not in dict).
    /// </summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RouteByStage(IReadOnlyDictionary<TStage, string> routes)
    {
        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (!routes.TryGetValue(typed.CurrentStage, out var key))
                return true;

            typed.Subject?.Reply(typed.Source, "Skip", key);
            return true;
        });

        return this;
    }

    /// <summary>Per-execution overload — <paramref name="routes" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RouteByStage(QuestArg<IReadOnlyDictionary<TStage, string>> routes)
    {
        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (!routes.Resolve(typed).TryGetValue(typed.CurrentStage, out var key))
                return true;

            typed.Subject?.Reply(typed.Source, "Skip", key);
            return true;
        });

        return this;
    }

    // ===== Sub-stage operations =====
    //
    // Sub-stages are additional typed enum trackers that live alongside the primary TStage
    // in Source.Trackers.Enums. A quest can layer any number of sub-stages, each keyed by a
    // distinct enum type. The framework does not enforce a relationship between primary and
    // sub-stages — quest authors compose them as needed.

    /// <summary>
    /// Halt the chain unless Source's sub-stage of type <typeparamref name="TSub" /> equals
    /// <paramref name="value" />. When <paramref name="failureReply" /> is provided, the failure
    /// path also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenAtSub<TSub>(TSub value, QuestArg<string>? failureReply = null) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtSub(value), failureReply);

    /// <summary>Halt the chain unless the source's tracked sub-stage is any of <paramref name="values" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(params IReadOnlyCollection<TSub> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values));

    /// <summary>Per-execution overload — <paramref name="values" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(QuestArg<IReadOnlyCollection<TSub>> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values.Resolve(ctx)));

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source's tracked sub-stage is any of <paramref name="values" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(QuestArg<string> failureReply, params IReadOnlyCollection<TSub> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values), failureReply);

    /// <summary>Per-execution overload — <paramref name="values" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(QuestArg<string> failureReply, QuestArg<IReadOnlyCollection<TSub>> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values.Resolve(ctx)), failureReply);

    /// <summary>
    /// Halt the chain unless Source has no value stored for sub-stage type
    /// <typeparamref name="TSub" /> (the sub-track has never been advanced). When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenSubNeverStarted<TSub>(QuestArg<string>? failureReply = null) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.HasNoSub<TSub>(), failureReply);

    /// <summary>Set the sub-stage of type <typeparamref name="TSub" /> on Source's Trackers.Enums.</summary>
    public QuestStepBuilder<TStage> AdvanceSub<TSub>(TSub value) where TSub : struct, Enum
        => Append(ctx => ctx.Source.Trackers.Enums.Set(value));

    /// <summary>Remove any stored value of type <typeparamref name="TSub" /> from Source's Trackers.Enums.</summary>
    public QuestStepBuilder<TStage> ClearSub<TSub>() where TSub : struct, Enum
        => Append(ctx => ctx.Source.Trackers.Enums.Remove<TSub>());

    /// <summary>
    /// Maps the current sub-stage of type <typeparamref name="TSub" /> to a dialog key and Skips
    /// to it. No-op if the sub-track has never been advanced or the current value is not in
    /// <paramref name="routes" />.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RouteBySub<TSub>(IReadOnlyDictionary<TSub, string> routes) where TSub : struct, Enum
    {
        Operations.Add(ctx =>
        {
            if (!ctx.TryGetSub<TSub>(out var current))
                return true;

            if (!routes.TryGetValue(current, out var key))
                return true;

            ctx.Subject?.Reply(ctx.Source, "Skip", key);

            return true;
        });

        return this;
    }

    /// <summary>Per-execution overload — <paramref name="routes" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RouteBySub<TSub>(QuestArg<IReadOnlyDictionary<TSub, string>> routes) where TSub : struct, Enum
    {
        Operations.Add(ctx =>
        {
            if (!ctx.TryGetSub<TSub>(out var current))
                return true;

            if (!routes.Resolve(ctx).TryGetValue(current, out var key))
                return true;

            ctx.Subject?.Reply(ctx.Source, "Skip", key);

            return true;
        });

        return this;
    }

    // ===== Flag operations (auto-dispatch Flag vs BigFlag by argument type) =====

    /// <summary>Set the given enum flag on the source's Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> SetFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
        => Append(ctx => FlagDispatch.Set(ctx.Source, flag));

    /// <summary>Clear the given enum flag from the source's Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> ClearFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
        => Append(ctx => FlagDispatch.Clear(ctx.Source, flag));

    /// <summary>Halt the chain unless the source's Trackers.Flags contains the given enum flag.</summary>
    public QuestStepBuilder<TStage> RequireFlag<TFlag>(TFlag flag, QuestArg<string>? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasFlag(flag), failureReply);

    /// <summary>Halt the chain unless every bit in <paramref name="combined" /> is set in Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> RequireAllFlags<TFlag>(TFlag combined, QuestArg<string>? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasAllFlags(combined), failureReply);

    /// <summary>Halt the chain unless at least one bit in <paramref name="combined" /> is set in Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> RequireAnyFlag<TFlag>(TFlag combined, QuestArg<string>? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasAnyFlag(combined), failureReply);

    /// <summary>Set the given big flag on the source's Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> SetFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class
        => Append(ctx => FlagDispatch.Set(ctx.Source, flag));

    /// <summary>Clear the given big flag from the source's Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> ClearFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class
        => Append(ctx => FlagDispatch.Clear(ctx.Source, flag));

    /// <summary>Halt the chain unless the source's Trackers.BigFlags contains the given big flag.</summary>
    public QuestStepBuilder<TStage> RequireFlag<TMarker>(BigFlagsValue<TMarker> flag, QuestArg<string>? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasFlag(flag), failureReply);

    /// <summary>Halt the chain unless every bit in <paramref name="combined" /> is set in Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> RequireAllFlags<TMarker>(BigFlagsValue<TMarker> combined, QuestArg<string>? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasAllFlags(combined), failureReply);

    /// <summary>Halt the chain unless at least one bit in <paramref name="combined" /> is set in Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> RequireAnyFlag<TMarker>(BigFlagsValue<TMarker> combined, QuestArg<string>? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasAnyFlag(combined), failureReply);

    // ===== Counter operations =====

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> is at or above <paramref name="value" />.</summary>
    public QuestStepBuilder<TStage> RequireCountGreaterThanOrEqualTo(string key, QuestArg<int> value, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.CounterGreaterThanOrEqualTo(key, value.Resolve(ctx)), failureReply);

    /// <summary>Increment the source's counter for <paramref name="key" /> by <paramref name="by" />.</summary>
    public QuestStepBuilder<TStage> IncrementCounter(string key, int by = 1)
        => Append(ctx => ctx.Source.Trackers.Counters.AddOrIncrement(key, by));

    /// <summary>Increment the source's counter for <paramref name="key" /> by a per-execution resolved <paramref name="by" /> value.</summary>
    public QuestStepBuilder<TStage> IncrementCounter(string key, QuestArg<int> by)
        => Append(ctx => ctx.Source.Trackers.Counters.AddOrIncrement(key, by.Resolve(ctx)));

    /// <summary>Remove the counter for <paramref name="key" /> from the source's Trackers.Counters.</summary>
    public QuestStepBuilder<TStage> ClearCounter(string key)
        => Append(ctx => ctx.Source.Trackers.Counters.Remove(key, out _));

    /// <summary>
    /// Set the source's counter for <paramref name="key" /> to <paramref name="value" />,
    /// replacing any existing value. <paramref name="value" /> accepts either a literal
    /// (lifted via <see cref="QuestArg{T}" />'s implicit conversion) or a per-execution
    /// resolver via <see cref="QuestArg.From{T}" />.
    /// </summary>
    public QuestStepBuilder<TStage> SetCounter(string key, QuestArg<int> value)
        => Append(ctx => ctx.Source.Trackers.Counters.Set(key, value.Resolve(ctx)));

    /// <summary>
    ///     Decrement the source's counter for <paramref name="key" /> by <paramref name="by" />. No-op if the counter does not exist
    ///     or would underflow below zero.
    /// </summary>
    public QuestStepBuilder<TStage> DecrementCounter(string key, int by = 1)
        => Append(ctx => ctx.Source.Trackers.Counters.TryDecrement(key, by, out _));

    /// <summary>Decrement the source's counter for <paramref name="key" /> by a per-execution resolved <paramref name="by" /> value.</summary>
    public QuestStepBuilder<TStage> DecrementCounter(string key, QuestArg<int> by)
        => Append(ctx => ctx.Source.Trackers.Counters.TryDecrement(key, by.Resolve(ctx), out _));

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> is at or below <paramref name="value" />. A missing counter is treated as 0.</summary>
    public QuestStepBuilder<TStage> RequireCountLessThanOrEqualTo(string key, QuestArg<int> value, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.CounterLessThanOrEqualTo(key, value.Resolve(ctx)), failureReply);

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> equals <paramref name="value" />. A missing counter is treated as 0.</summary>
    public QuestStepBuilder<TStage> RequireCountEqualTo(string key, QuestArg<int> value, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.CounterEqualTo(key, value.Resolve(ctx)), failureReply);

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> is strictly less than <paramref name="value" />. A missing counter is treated as 0.</summary>
    public QuestStepBuilder<TStage> RequireCountLessThan(string key, QuestArg<int> value, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.CounterLessThan(key, value.Resolve(ctx)), failureReply);

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> is strictly greater than <paramref name="value" />. A missing counter is treated as 0.</summary>
    public QuestStepBuilder<TStage> RequireCountGreaterThan(string key, QuestArg<int> value, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.CounterGreaterThan(key, value.Resolve(ctx)), failureReply);

    // ===== Cooldown operations =====

    /// <summary>Start an auto-consuming cooldown event keyed by <paramref name="key" /> on the source's Trackers.TimedEvents.</summary>
    public QuestStepBuilder<TStage> StartCooldown(string key, QuestArg<TimeSpan> duration)
        => Append(ctx => ctx.Source.Trackers.TimedEvents.AddEvent(key, duration.Resolve(ctx), true));

    /// <summary>Halt the chain if the source's Trackers.TimedEvents has an active event for <paramref name="key" />.</summary>
    public QuestStepBuilder<TStage> RequireCooldownExpired(string key)
        => AppendGuard(ctx => !ctx.HasActiveCooldown(key, out _));

    /// <summary>
    /// Halt the chain if the source's Trackers.TimedEvents has an active event for <paramref name="key" />,
    /// and emit a templated dialog reply. Use <c>{remaining}</c> in <paramref name="activeMessageTemplate" />;
    /// it is replaced with the readable remaining time.
    /// </summary>
    public QuestStepBuilder<TStage> RequireCooldownExpired(string key, string activeMessageTemplate)
    {
        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (!typed.HasActiveCooldown(key, out var remaining))
                return true;

            var msg = activeMessageTemplate.Replace("{remaining}", remaining.ToReadableString());
            typed.Subject?.Reply(typed.Source, msg);

            return false;
        });

        return this;
    }

    // ===== Item operations =====

    /// <summary>Create an item by template key and grant it to the source (overflow goes to bank).</summary>
    public QuestStepBuilder<TStage> GiveItem(string templateKey, int count = 1)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<IItemFactory>();
            var item = factory.Create(templateKey);

            if (count > 1)
                item.Count = count;

            ctx.Source.GiveItemOrSendToBank(item);
        });

    /// <summary>Create an item by template key and grant it to the source with a per-execution resolved <paramref name="count" />.</summary>
    public QuestStepBuilder<TStage> GiveItem(string templateKey, QuestArg<int> count)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<IItemFactory>();
            var item = factory.Create(templateKey);
            var resolved = count.Resolve(ctx);

            if (resolved > 1)
                item.Count = resolved;

            ctx.Source.GiveItemOrSendToBank(item);
        });

    /// <summary>Grant a batch of items by template key. Each is created and granted independently.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> GiveItems(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<IItemFactory>();

            foreach ((var key, var count) in items)
            {
                var item = factory.Create(key);

                if (count > 1)
                    item.Count = count;

                ctx.Source.GiveItemOrSendToBank(item);
            }
        });

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> GiveItems(QuestArg<IReadOnlyCollection<(string TemplateKey, int Count)>> items)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<IItemFactory>();

            foreach ((var key, var count) in items.Resolve(ctx))
            {
                var item = factory.Create(key);

                if (count > 1)
                    item.Count = count;

                ctx.Source.GiveItemOrSendToBank(item);
            }
        });

    /// <summary>Halt the chain unless the source has at least <paramref name="count" /> of <paramref name="templateKey" />.</summary>
    public QuestStepBuilder<TStage> RequireItemByTemplateKey(string templateKey, QuestArg<int> count, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemByTemplateKey(templateKey, count.Resolve(ctx)), failureReply);

    /// <summary>Halt the chain unless the source has at least <paramref name="count" /> of items whose display name equals <paramref name="name" /> (case-insensitive). Use <see cref="RequireItemByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> RequireItem(string name, QuestArg<int> count, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasItem(name, count.Resolve(ctx)), failureReply);

    /// <summary>
    /// Halt the chain unless the source has at least <paramref name="count" /> of
    /// <paramref name="templateKey" /> in inventory, or — when <paramref name="count" /> is 1 —
    /// the item is currently equipped.
    /// </summary>
    public QuestStepBuilder<TStage> RequireItemOrEquippedByTemplateKey(string templateKey, QuestArg<int> count, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemOrEquippedByTemplateKey(templateKey, count.Resolve(ctx)), failureReply);

    /// <summary>
    /// Halt the chain unless the source has at least <paramref name="count" /> of items named
    /// <paramref name="name" /> in inventory, or — when <paramref name="count" /> is 1 —
    /// an item with that name is currently equipped. Use <see cref="RequireItemOrEquippedByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public QuestStepBuilder<TStage> RequireItemOrEquipped(string name, QuestArg<int> count, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemOrEquipped(name, count.Resolve(ctx)), failureReply);

    /// <summary>
    /// Halt the chain unless an item identified by <paramref name="templateKey" /> is currently equipped.
    /// Equipment slots are non-stacking, so this is a binary "is it worn?" check.
    /// </summary>
    public QuestStepBuilder<TStage> RequireEquippedByTemplateKey(string templateKey, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasEquippedByTemplateKey(templateKey), failureReply);

    /// <summary>
    /// Halt the chain unless an item whose display name equals <paramref name="name" /> (case-insensitive)
    /// is currently equipped. Use <see cref="RequireEquippedByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public QuestStepBuilder<TStage> RequireEquipped(string name, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasEquipped(name), failureReply);

    /// <summary>Halt the chain unless the source has every (templateKey, count) pair in <paramref name="items" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var key, var count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        });

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(QuestArg<IReadOnlyCollection<(string TemplateKey, int Count)>> items)
        => AppendGuard(ctx =>
        {
            foreach ((var key, var count) in items.Resolve(ctx))
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        });

    /// <summary>Halt the chain unless the source has every (templateKey, count) pair in <paramref name="items" />. On failure, sends <paramref name="failureReply" /> as a Reply.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(QuestArg<string> failureReply, params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var key, var count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(QuestArg<string> failureReply, QuestArg<IReadOnlyCollection<(string TemplateKey, int Count)>> items)
        => AppendGuard(ctx =>
        {
            foreach ((var key, var count) in items.Resolve(ctx))
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Halt the chain unless the source has every (name, count) pair in <paramref name="items" />. Use <see cref="RequireItemsByTemplateKey(IReadOnlyCollection{ValueTuple{string, int}})" /> when templateKeys are the stable identifiers.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RequireItems(params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var name, var count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        });

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RequireItems(QuestArg<IReadOnlyCollection<(string Name, int Count)>> items)
        => AppendGuard(ctx =>
        {
            foreach ((var name, var count) in items.Resolve(ctx))
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        });

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source has every (name, count) pair in <paramref name="items" />.</summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> RequireItems(QuestArg<string> failureReply, params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var name, var count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> RequireItems(QuestArg<string> failureReply, QuestArg<IReadOnlyCollection<(string Name, int Count)>> items)
        => AppendGuard(ctx =>
        {
            foreach ((var name, var count) in items.Resolve(ctx))
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of <paramref name="templateKey" />; otherwise consume.</summary>
    public QuestStepBuilder<TStage> ConsumeItemByTemplateKey(string templateKey, QuestArg<int> count)
        => AppendGuard(ctx =>
        {
            var resolved = count.Resolve(ctx);

            if (!ctx.HasItemByTemplateKey(templateKey, resolved))
                return false;

            return ctx.Source.Inventory.RemoveQuantityByTemplateKey(templateKey, resolved);
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of items whose display name equals <paramref name="name" /> (case-insensitive); otherwise consume. Use <see cref="ConsumeItemByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> ConsumeItem(string name, QuestArg<int> count)
        => AppendGuard(ctx =>
        {
            var resolved = count.Resolve(ctx);

            if (!ctx.HasItem(name, resolved))
                return false;

            return ctx.Source.Inventory.RemoveQuantity(name, resolved);
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of <paramref name="templateKey" /> from inventory or (count==1) equipment; otherwise consume.</summary>
    public QuestStepBuilder<TStage> ConsumeItemOrEquippedByTemplateKey(string templateKey, QuestArg<int> count)
        => AppendGuard(ctx =>
        {
            var resolved = count.Resolve(ctx);

            if (ctx.HasItemByTemplateKey(templateKey, resolved))
                return ctx.Source.Inventory.RemoveQuantityByTemplateKey(templateKey, resolved);

            if ((resolved == 1) && ctx.Source.Equipment.TryGetRemoveByTemplateKey(templateKey, out _))
                return true;

            return false;
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of items named <paramref name="name" /> from inventory or (count==1) equipment; otherwise consume. Use <see cref="ConsumeItemOrEquippedByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> ConsumeItemOrEquipped(string name, QuestArg<int> count)
        => AppendGuard(ctx =>
        {
            var resolved = count.Resolve(ctx);

            if (ctx.HasItem(name, resolved))
                return ctx.Source.Inventory.RemoveQuantity(name, resolved);

            if ((resolved == 1) && ctx.Source.Equipment.TryGetRemove(name, out _))
                return true;

            return false;
        });

    /// <summary>Halt the chain unless an item identified by <paramref name="templateKey" /> is currently equipped; otherwise unequip and destroy it.</summary>
    public QuestStepBuilder<TStage> ConsumeEquippedByTemplateKey(string templateKey)
        => AppendGuard(ctx => ctx.Source.Equipment.TryGetRemoveByTemplateKey(templateKey, out _));

    /// <summary>Halt the chain unless an item named <paramref name="name" /> (case-insensitive) is currently equipped; otherwise unequip and destroy it. Use <see cref="ConsumeEquippedByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> ConsumeEquipped(string name)
        => AppendGuard(ctx => ctx.Source.Equipment.TryGetRemove(name, out _));

    /// <summary>
    /// Pre-check every (templateKey, count) pair; halt if any insufficient. Otherwise consume all atomically.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> ConsumeItemsByTemplateKey(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var key, var count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            foreach ((var key, var count) in items)
                ctx.Source.Inventory.RemoveQuantityByTemplateKey(key, count);

            return true;
        });

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> ConsumeItemsByTemplateKey(QuestArg<IReadOnlyCollection<(string TemplateKey, int Count)>> items)
        => AppendGuard(ctx =>
        {
            var resolved = items.Resolve(ctx);

            foreach ((var key, var count) in resolved)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            foreach ((var key, var count) in resolved)
                ctx.Source.Inventory.RemoveQuantityByTemplateKey(key, count);

            return true;
        });

    /// <summary>
    /// Pre-check every (name, count) pair; halt if any insufficient. Otherwise consume all atomically.
    /// Use <see cref="ConsumeItemsByTemplateKey(IReadOnlyCollection{ValueTuple{string, int}})" /> when templateKeys are the stable identifiers.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public QuestStepBuilder<TStage> ConsumeItems(params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach ((var name, var count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            foreach ((var name, var count) in items)
                ctx.Source.Inventory.RemoveQuantity(name, count);

            return true;
        });

    /// <summary>Per-execution overload — <paramref name="items" /> resolves at chain run time.</summary>
    public QuestStepBuilder<TStage> ConsumeItems(QuestArg<IReadOnlyCollection<(string Name, int Count)>> items)
        => AppendGuard(ctx =>
        {
            var resolved = items.Resolve(ctx);

            foreach ((var name, var count) in resolved)
                if (!ctx.HasItem(name, count))
                    return false;

            foreach ((var name, var count) in resolved)
                ctx.Source.Inventory.RemoveQuantity(name, count);

            return true;
        });

    // ===== Reward operations =====

    /// <summary>Award the source <paramref name="amount" /> experience via the active IExperienceDistributionScript.</summary>
    public QuestStepBuilder<TStage> GiveExperience(QuestArg<long> amount)
        => Append(ctx => DefaultExperienceDistributionScript.Create().GiveExp(ctx.Source, amount.Resolve(ctx)));

    /// <summary>Award the source <paramref name="amount" /> ability experience via the active IAbilityDistributionScript.</summary>
    public QuestStepBuilder<TStage> GiveAbility(QuestArg<long> amount)
        => Append(ctx => DefaultAbilityDistributionScript.Create().GiveAbility(ctx.Source, amount.Resolve(ctx)));

    /// <summary>Add <paramref name="amount" /> gold to the source. Chain continues even if TryGiveGold returns false.</summary>
    public QuestStepBuilder<TStage> GiveGold(QuestArg<int> amount) => Append(ctx => ctx.Source.TryGiveGold(amount.Resolve(ctx)));

    /// <summary>
    /// Halt the chain unless Source has at least <paramref name="amount" /> gold. Pure check —
    /// does not consume. When <paramref name="failureReply" /> is provided, the failure path
    /// also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> RequireGold(QuestArg<int> amount, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.HasGold(amount.Resolve(ctx)), failureReply);

    /// <summary>
    /// Halt the chain unless Source can give up <paramref name="amount" /> gold; otherwise consume.
    /// Note: on failure the underlying <c>TryTakeGold</c> sends a system "not enough gold"
    /// orange-bar message. To present a quest-specific reply instead, gate with
    /// <see cref="RequireGold" /> first.
    /// </summary>
    public QuestStepBuilder<TStage> ConsumeGold(QuestArg<int> amount)
        => AppendGuard(ctx => ctx.Source.TryTakeGold(amount.Resolve(ctx)));

    /// <summary>Add <paramref name="amount" /> game points to the source. Chain continues unconditionally.</summary>
    public QuestStepBuilder<TStage> GiveGamePoints(QuestArg<int> amount) => Append(ctx => ctx.Source.GamePoints += amount.Resolve(ctx));

    /// <summary>Add (or accumulate) a LegendMark on the source's Legend. Time stamp is GameTime.Now.</summary>
    public QuestStepBuilder<TStage> GiveOrAccumulateLegendMark(string text, string key, MarkIcon icon, MarkColor color)
        => Append(ctx => ctx.Source.Legend.AddOrAccumulate(new LegendMark(text, key, icon, color, 1, GameTime.Now)));

    /// <summary>Add a unique LegendMark on the source's Legend. Replaces an existing mark with the same key; does NOT increment count on repeat.</summary>
    public QuestStepBuilder<TStage> GiveUniqueLegendMark(string text, string key, MarkIcon icon, MarkColor color)
        => Append(ctx => ctx.Source.Legend.AddUnique(new LegendMark(text, key, icon, color, 1, GameTime.Now)));

    /// <summary>Create and grant a skill via ISkillFactory + ComplexActionHelper.LearnSkill.</summary>
    public QuestStepBuilder<TStage> GiveSkill(string templateKey)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<ISkillFactory>();
            var skill = factory.Create(templateKey);
            ComplexActionHelper.LearnSkill(ctx.Source, skill);
        });

    /// <summary>Create and grant a spell via ISpellFactory + ComplexActionHelper.LearnSpell.</summary>
    public QuestStepBuilder<TStage> GiveSpell(string templateKey, byte? page = null)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<ISpellFactory>();
            var spell = factory.Create(templateKey);
            ComplexActionHelper.LearnSpell(ctx.Source, spell);

            if (page.HasValue && ctx.Source.SpellBook.TryGetObjectByTemplateKey(templateKey, out var learned))
                ctx.Source.SpellBook.TrySwap(learned.Slot, page.Value);
        });

    /// <summary>Remove the skill matching <paramref name="templateKey" /> from the source's SkillBook.</summary>
    public QuestStepBuilder<TStage> RemoveSkill(string templateKey)
        => Append(ctx => ctx.Source.SkillBook.RemoveByTemplateKey(templateKey));

    /// <summary>Remove the spell matching <paramref name="templateKey" /> from the source's SpellBook.</summary>
    public QuestStepBuilder<TStage> RemoveSpell(string templateKey)
        => Append(ctx => ctx.Source.SpellBook.RemoveByTemplateKey(templateKey));

    // ===== Class/level/gender guards =====

    /// <summary>Halt the chain unless Source's UserStatSheet.Level is within <c>[min, max]</c> (inclusive).</summary>
    public QuestStepBuilder<TStage> RequireLevel(int min, int max = int.MaxValue, QuestArg<string>? failureReply = null)
        => AppendGuard(
            ctx =>
            {
                var level = ctx.Source.UserStatSheet.Level;

                return (level >= min) && (level <= max);
            },
            failureReply);

    /// <summary>Per-execution overload of <see cref="RequireLevel(int, int, QuestArg{string}?)" />. Both bounds resolve at chain execution time.</summary>
    public QuestStepBuilder<TStage> RequireLevel(QuestArg<int> min, QuestArg<int> max, QuestArg<string>? failureReply = null)
        => AppendGuard(
            ctx =>
            {
                var level = ctx.Source.UserStatSheet.Level;

                return (level >= min.Resolve(ctx)) && (level <= max.Resolve(ctx));
            },
            failureReply);

    /// <summary>Halt the chain unless Source has mastered (UserStatSheet.Master is true).</summary>
    public QuestStepBuilder<TStage> RequireMaster(QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.Source.UserStatSheet.Master, failureReply);

    /// <summary>
    /// Halt the chain unless Source is a full grandmaster — both mastered and at the configured
    /// max level (<see cref="WorldOptions" />.MaxLevel).
    /// </summary>
    public QuestStepBuilder<TStage> RequireFullGrandmaster(QuestArg<string>? failureReply = null)
        => AppendGuard(
            ctx => ctx.Source.UserStatSheet.Master && (ctx.Source.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel),
            failureReply);

    /// <summary>Halt the chain unless Source is mastered AND BaseClass equals <paramref name="baseClass" />.</summary>
    public QuestStepBuilder<TStage> RequirePureMaster(BaseClass baseClass, QuestArg<string>? failureReply = null)
        => AppendGuard(
            ctx => ctx.Source.UserStatSheet.Master && (ctx.Source.UserStatSheet.BaseClass == baseClass),
            failureReply);

    /// <summary>
    /// Halt the chain unless Source's BaseClass matches <paramref name="baseClass" />. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply. For
    /// class-gated branching (no halt on mismatch), compose with <see cref="Branch" /> instead
    /// (e.g. <c>Branch(c =&gt; c.Source.UserStatSheet.BaseClass == X, body)</c>).
    /// </summary>
    public QuestStepBuilder<TStage> RequireClass(BaseClass baseClass, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.Source.UserStatSheet.BaseClass == baseClass, failureReply);

    /// <summary>
    /// Halt the chain unless Source's Gender matches <paramref name="gender" />. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply. For
    /// gender-gated branching (no halt on mismatch), compose with <see cref="Branch" /> instead
    /// (e.g. <c>Branch(c =&gt; c.Source.Gender == X, body)</c>).
    /// </summary>
    public QuestStepBuilder<TStage> RequireGender(Gender gender, QuestArg<string>? failureReply = null)
        => AppendGuard(ctx => ctx.Source.Gender == gender, failureReply);

    // ===== Branching =====

    /// <summary>
    /// Conditional sub-chain. Runs <paramref name="configure" />'s operations if
    /// <paramref name="predicate" /> returns true; otherwise the outer chain continues unchanged.
    /// On a successful match, <see cref="QuestContext.OtherwiseTaken" /> is set so a later
    /// <see cref="Otherwise(System.Action{QuestStepBuilder{TStage}})" /> knows a branch already fired.
    /// </summary>
    /// <remarks>
    /// The sub-builder is captured by reference via <see cref="Build" />'s live-aliasing
    /// convention — operations appended after this call are observed when the chain runs.
    /// </remarks>
    public QuestStepBuilder<TStage> Branch(
        Func<QuestContext<TStage>, bool> predicate,
        Action<QuestStepBuilder<TStage>> configure)
    {
        var sub = new QuestStepBuilder<TStage>();
        configure(sub);
        var subOps = sub.Build();

        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (!predicate(typed))
                return true;

            typed.OtherwiseTaken = true;

            foreach (var op in subOps)
                if (!op(typed))
                    return true; // body halted; outer continues

            return true;
        });

        return this;
    }

    // ===== Communication =====

    /// <summary>
    /// Send a dialog reply to the source. Halts the chain — replies signal the end of the
    /// current step's dialog flow. No-ops (but still halts) if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> Reply(QuestArg<string> text)
    {
        Operations.Add(ctx =>
        {
            ctx.Subject?.Reply(ctx.Source, text.Resolve(ctx));

            return false;
        });

        return this;
    }

    /// <summary>
    /// Skip to another dialog by key (Reply with text "Skip"). Halts the chain. No-ops (but still
    /// halts) if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> Skip(string dialogKey)
    {
        Operations.Add(ctx =>
        {
            ctx.Subject?.Reply(ctx.Source, "Skip", dialogKey);

            return false;
        });

        return this;
    }

    /// <summary>
    /// Add a dialog option pointing to <paramref name="dialogKey" /> if not already present. Chain
    /// continues. No-ops if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> ShowOption(QuestArg<string> text, string dialogKey)
        => Append(ctx =>
        {
            if (ctx.Subject is null)
                return;

            var resolvedText = text.Resolve(ctx);

            if (!ctx.Subject.HasOption(resolvedText))
                ctx.Subject.AddOption(resolvedText, dialogKey);
        });

    /// <summary>
    /// Insert a dialog option pointing to <paramref name="dialogKey" /> at <paramref name="index" />
    /// if not already present (default index 0 places the option at the top of the menu — useful
    /// when injecting a quest hook into an NPC's pre-existing main menu). Chain continues. No-ops
    /// if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> InsertOption(QuestArg<string> text, string dialogKey, QuestArg<int> index = default)
        => Append(ctx =>
        {
            if (ctx.Subject is null)
                return;

            var resolvedText = text.Resolve(ctx);

            if (!ctx.Subject.HasOption(resolvedText))
                ctx.Subject.InsertOption(index.Resolve(ctx), resolvedText, dialogKey);
        });

    /// <summary>
    /// Inject text parameters into ctx.Subject's dialog text (forwards to
    /// <see cref="Models.Menu.Dialog.InjectTextParameters" />). Intended for the
    /// <c>OnDisplaying</c> phase, where it runs before the dialog is sent. Chain continues; no-op
    /// if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> InjectTextParameters(params object[] parameters)
        => Append(ctx => ctx.Subject?.InjectTextParameters(parameters));

    /// <summary>
    /// Context-aware overload of <see cref="InjectTextParameters(object[])" />.
    /// <paramref name="selector" /> is invoked at chain run time so dynamic values from the
    /// player or quest context (gold, item count, kill counter, etc.) can be templated into the
    /// dialog text. Chain continues; no-op if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> InjectTextParameters(Func<QuestContext<TStage>, object[]> selector)
        => Append(ctx => ctx.Subject?.InjectTextParameters(selector(ctx)));

    /// <summary>
    /// Send an active (orange-bar) message to the source. Chain continues.
    /// </summary>
    public QuestStepBuilder<TStage> SendOrangeBar(QuestArg<string> text) => Append(ctx => ctx.Source.SendActiveMessage(text.Resolve(ctx)));

    /// <summary>
    /// Escape hatch — run an arbitrary action against the source and context, then continue the chain.
    /// </summary>
    public QuestStepBuilder<TStage> Run(Action<Aisling, QuestContext<TStage>> action)
        => Append(ctx => action(ctx.Source, ctx));

    // ===== Teleports =====

    /// <summary>
    /// Teleport Source to <c>(x, y)</c> on the map identified by <paramref name="mapKey" />.
    /// Resolves the destination map via <see cref="ISimpleCache{MapInstance}" />.
    /// </summary>
    public QuestStepBuilder<TStage> Teleport(string mapKey, QuestArg<int> x, QuestArg<int> y)
        => Append(ctx => TeleportCore(ctx, mapKey, new Point(x.Resolve(ctx), y.Resolve(ctx))));

    /// <summary>
    /// Teleport Source to a randomly-chosen walkable point inside <paramref name="rect" /> on the
    /// map identified by <paramref name="mapKey" />. Throws if no walkable point can be found in
    /// 100 attempts.
    /// </summary>
    public QuestStepBuilder<TStage> Teleport(string mapKey, Rectangle rect)
        => Append(ctx => TeleportCore(ctx, mapKey, PickWalkable(ctx, mapKey, rect)));

    /// <summary>
    /// Teleport Source's group (or Source alone if ungrouped) to <c>(x, y)</c> on the resolved map.
    /// </summary>
    public QuestStepBuilder<TStage> GroupTeleport(string mapKey, QuestArg<int> x, QuestArg<int> y)
        => Append(ctx => GroupTeleportCore(ctx, mapKey, new Point(x.Resolve(ctx), y.Resolve(ctx))));

    /// <summary>
    /// Teleport Source's group (or Source alone if ungrouped) to a single shared walkable point
    /// inside <paramref name="rect" /> on the resolved map. Per-member random points are
    /// deferred to v2 — the v1 contract is a single point for the whole group.
    /// </summary>
    public QuestStepBuilder<TStage> GroupTeleport(string mapKey, Rectangle rect)
        => Append(ctx => GroupTeleportCore(ctx, mapKey, PickWalkable(ctx, mapKey, rect)));

    private static void TeleportCore(QuestContext ctx, string mapKey, IPoint point)
    {
        var cache = ctx.Services.GetRequiredService<ISimpleCache<MapInstance>>();
        var map = cache.Get(mapKey);
        ctx.Source.TraverseMap(map, point);
    }

    private static void GroupTeleportCore(QuestContext ctx, string mapKey, IPoint point)
    {
        var cache = ctx.Services.GetRequiredService<ISimpleCache<MapInstance>>();
        var map = cache.Get(mapKey);

        if (ctx.Source.Group is null)
        {
            ctx.Source.TraverseMap(map, point);
            return;
        }

        foreach (var member in ctx.Source.Group)
            member.TraverseMap(map, point);
    }

    /// <summary>
    /// Pick a random walkable point inside <paramref name="rect" /> on the map identified by
    /// <paramref name="mapKey" />. Throws after 100 unsuccessful attempts so quest authors can
    /// detect mis-configured rectangles instead of silently swallowing the failure.
    /// </summary>
    private static IPoint PickWalkable(QuestContext ctx, string mapKey, Rectangle rect)
    {
        var cache = ctx.Services.GetRequiredService<ISimpleCache<MapInstance>>();
        var map = cache.Get(mapKey);

        for (var attempts = 0; attempts < 100; attempts++)
        {
            var p = rect.GetRandomPoint();

            if (map.IsWalkable(p, ctx.Source))
                return p;
        }

        throw new InvalidOperationException($"Could not find walkable point in {rect} on {mapKey}");
    }
}