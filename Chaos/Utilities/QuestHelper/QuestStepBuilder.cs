#region
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
        string? failureReply = null)
    {
        if (failureReply is null)
        {
            Operations.Add(ctx => predicate((QuestContext<TStage>)ctx));

            return this;
        }

        Operations.Add(ctx =>
        {
            var typed = (QuestContext<TStage>)ctx;

            if (predicate(typed))
                return true;

            typed.Subject?.Reply(typed.Source, failureReply);

            return false;
        });

        return this;
    }

    // ===== Stage operations =====

    /// <summary>
    /// Halt the chain unless Source is currently at the given stage. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenAt(TStage stage, string? failureReply = null)
        => AppendGuard(ctx => ctx.WhenAt(stage), failureReply);

    /// <summary>Halt the chain unless the source's tracked stage is any of <paramref name="stages" />.</summary>
    public QuestStepBuilder<TStage> WhenAtAny(params IReadOnlyCollection<TStage> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages));

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source's tracked stage is any of <paramref name="stages" />.</summary>
    public QuestStepBuilder<TStage> WhenAtAny(string failureReply, params IReadOnlyCollection<TStage> stages)
        => AppendGuard(ctx => ctx.WhenAtAny(stages), failureReply);

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
    public QuestStepBuilder<TStage> WhenNeverStarted(string? failureReply = null)
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
    public QuestStepBuilder<TStage> WhenAtSub<TSub>(TSub value, string? failureReply = null) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtSub(value), failureReply);

    /// <summary>Halt the chain unless the source's tracked sub-stage is any of <paramref name="values" />.</summary>
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(params IReadOnlyCollection<TSub> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values));

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source's tracked sub-stage is any of <paramref name="values" />.</summary>
    public QuestStepBuilder<TStage> WhenAtAnySub<TSub>(string failureReply, params IReadOnlyCollection<TSub> values) where TSub : struct, Enum
        => AppendGuard(ctx => ctx.WhenAtAnySub(values), failureReply);

    /// <summary>
    /// Halt the chain unless Source has no value stored for sub-stage type
    /// <typeparamref name="TSub" /> (the sub-track has never been advanced). When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> WhenSubNeverStarted<TSub>(string? failureReply = null) where TSub : struct, Enum
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

    // ===== Flag operations (auto-dispatch Flag vs BigFlag by argument type) =====

    /// <summary>Set the given enum flag on the source's Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> SetFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
        => Append(ctx => FlagDispatch.Set(ctx.Source, flag));

    /// <summary>Clear the given enum flag from the source's Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> ClearFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
        => Append(ctx => FlagDispatch.Clear(ctx.Source, flag));

    /// <summary>Halt the chain unless the source's Trackers.Flags contains the given enum flag.</summary>
    public QuestStepBuilder<TStage> RequireFlag<TFlag>(TFlag flag, string? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasFlag(flag), failureReply);

    /// <summary>Halt the chain unless every bit in <paramref name="combined" /> is set in Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> RequireAllFlags<TFlag>(TFlag combined, string? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasAllFlags(combined), failureReply);

    /// <summary>Halt the chain unless at least one bit in <paramref name="combined" /> is set in Trackers.Flags.</summary>
    public QuestStepBuilder<TStage> RequireAnyFlag<TFlag>(TFlag combined, string? failureReply = null) where TFlag : struct, Enum
        => AppendGuard(ctx => ctx.HasAnyFlag(combined), failureReply);

    /// <summary>Set the given big flag on the source's Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> SetFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class
        => Append(ctx => FlagDispatch.Set(ctx.Source, flag));

    /// <summary>Clear the given big flag from the source's Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> ClearFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class
        => Append(ctx => FlagDispatch.Clear(ctx.Source, flag));

    /// <summary>Halt the chain unless the source's Trackers.BigFlags contains the given big flag.</summary>
    public QuestStepBuilder<TStage> RequireFlag<TMarker>(BigFlagsValue<TMarker> flag, string? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasFlag(flag), failureReply);

    /// <summary>Halt the chain unless every bit in <paramref name="combined" /> is set in Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> RequireAllFlags<TMarker>(BigFlagsValue<TMarker> combined, string? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasAllFlags(combined), failureReply);

    /// <summary>Halt the chain unless at least one bit in <paramref name="combined" /> is set in Trackers.BigFlags.</summary>
    public QuestStepBuilder<TStage> RequireAnyFlag<TMarker>(BigFlagsValue<TMarker> combined, string? failureReply = null) where TMarker : class
        => AppendGuard(ctx => ctx.HasAnyFlag(combined), failureReply);

    // ===== Counter operations =====

    /// <summary>Halt the chain unless the source's kill counter for <paramref name="monsterTemplateKey" /> is at or above <paramref name="count" />.</summary>
    public QuestStepBuilder<TStage> RequireKills(string monsterTemplateKey, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.CounterHasValue(monsterTemplateKey, count), failureReply);

    /// <summary>Remove the kill counter for <paramref name="monsterTemplateKey" /> from the source's Trackers.Counters.</summary>
    public QuestStepBuilder<TStage> ClearKills(string monsterTemplateKey)
        => Append(ctx => ctx.Source.Trackers.Counters.Remove(monsterTemplateKey, out _));

    /// <summary>Halt the chain unless the source's counter for <paramref name="key" /> is at or above <paramref name="count" />.</summary>
    public QuestStepBuilder<TStage> RequireCount(string key, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.CounterHasValue(key, count), failureReply);

    /// <summary>Increment the source's counter for <paramref name="key" /> by <paramref name="by" />.</summary>
    public QuestStepBuilder<TStage> IncrementCounter(string key, int by = 1)
        => Append(ctx => ctx.Source.Trackers.Counters.AddOrIncrement(key, by));

    /// <summary>Remove the counter for <paramref name="key" /> from the source's Trackers.Counters.</summary>
    public QuestStepBuilder<TStage> ClearCounter(string key)
        => Append(ctx => ctx.Source.Trackers.Counters.Remove(key, out _));

    // ===== Cooldown operations =====

    /// <summary>Start an auto-consuming cooldown event keyed by <paramref name="key" /> on the source's Trackers.TimedEvents.</summary>
    public QuestStepBuilder<TStage> StartCooldown(string key, TimeSpan duration)
        => Append(ctx => ctx.Source.Trackers.TimedEvents.AddEvent(key, duration, true));

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

    /// <summary>Grant a batch of items by template key. Each is created and granted independently.</summary>
    public QuestStepBuilder<TStage> GiveItems(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => Append(ctx =>
        {
            var factory = ctx.Services.GetRequiredService<IItemFactory>();

            foreach (var (key, count) in items)
            {
                var item = factory.Create(key);

                if (count > 1)
                    item.Count = count;

                ctx.Source.GiveItemOrSendToBank(item);
            }
        });

    /// <summary>Halt the chain unless the source has at least <paramref name="count" /> of <paramref name="templateKey" />.</summary>
    public QuestStepBuilder<TStage> RequireItemByTemplateKey(string templateKey, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemByTemplateKey(templateKey, count), failureReply);

    /// <summary>Halt the chain unless the source has at least <paramref name="count" /> of items whose display name equals <paramref name="name" /> (case-insensitive). Use <see cref="RequireItemByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> RequireItem(string name, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasItem(name, count), failureReply);

    /// <summary>
    /// Halt the chain unless the source has at least <paramref name="count" /> of
    /// <paramref name="templateKey" /> in inventory, or — when <paramref name="count" /> is 1 —
    /// the item is currently equipped.
    /// </summary>
    public QuestStepBuilder<TStage> RequireItemOrEquippedByTemplateKey(string templateKey, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemOrEquippedByTemplateKey(templateKey, count), failureReply);

    /// <summary>
    /// Halt the chain unless the source has at least <paramref name="count" /> of items named
    /// <paramref name="name" /> in inventory, or — when <paramref name="count" /> is 1 —
    /// an item with that name is currently equipped. Use <see cref="RequireItemOrEquippedByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public QuestStepBuilder<TStage> RequireItemOrEquipped(string name, int count, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasItemOrEquipped(name, count), failureReply);

    /// <summary>
    /// Halt the chain unless an item identified by <paramref name="templateKey" /> is currently equipped.
    /// Equipment slots are non-stacking, so this is a binary "is it worn?" check.
    /// </summary>
    public QuestStepBuilder<TStage> RequireEquippedByTemplateKey(string templateKey, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasEquippedByTemplateKey(templateKey), failureReply);

    /// <summary>
    /// Halt the chain unless an item whose display name equals <paramref name="name" /> (case-insensitive)
    /// is currently equipped. Use <see cref="RequireEquippedByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public QuestStepBuilder<TStage> RequireEquipped(string name, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasEquipped(name), failureReply);

    /// <summary>Halt the chain unless the source has every (templateKey, count) pair in <paramref name="items" />.</summary>
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (key, count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        });

    /// <summary>Halt the chain unless the source has every (templateKey, count) pair in <paramref name="items" />. On failure, sends <paramref name="failureReply" /> as a Reply.</summary>
    public QuestStepBuilder<TStage> RequireItemsByTemplateKey(string failureReply, params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (key, count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Halt the chain unless the source has every (name, count) pair in <paramref name="items" />. Use <see cref="RequireItemsByTemplateKey(IReadOnlyCollection{ValueTuple{string, int}})" /> when templateKeys are the stable identifiers.</summary>
    public QuestStepBuilder<TStage> RequireItems(params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (name, count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        });

    /// <summary>Halt the chain (with <paramref name="failureReply" />) unless the source has every (name, count) pair in <paramref name="items" />.</summary>
    public QuestStepBuilder<TStage> RequireItems(string failureReply, params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (name, count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            return true;
        }, failureReply);

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of <paramref name="templateKey" />; otherwise consume.</summary>
    public QuestStepBuilder<TStage> ConsumeItemByTemplateKey(string templateKey, int count)
        => AppendGuard(ctx =>
        {
            if (!ctx.HasItemByTemplateKey(templateKey, count))
                return false;

            return ctx.Source.Inventory.RemoveQuantityByTemplateKey(templateKey, count);
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of items whose display name equals <paramref name="name" /> (case-insensitive); otherwise consume. Use <see cref="ConsumeItemByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> ConsumeItem(string name, int count)
        => AppendGuard(ctx =>
        {
            if (!ctx.HasItem(name, count))
                return false;

            return ctx.Source.Inventory.RemoveQuantity(name, count);
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of <paramref name="templateKey" /> from inventory or (count==1) equipment; otherwise consume.</summary>
    public QuestStepBuilder<TStage> ConsumeItemOrEquippedByTemplateKey(string templateKey, int count)
        => AppendGuard(ctx =>
        {
            if (ctx.HasItemByTemplateKey(templateKey, count))
                return ctx.Source.Inventory.RemoveQuantityByTemplateKey(templateKey, count);

            if ((count == 1) && ctx.Source.Equipment.TryGetRemoveByTemplateKey(templateKey, out _))
                return true;

            return false;
        });

    /// <summary>Halt the chain unless the source can give up <paramref name="count" /> of items named <paramref name="name" /> from inventory or (count==1) equipment; otherwise consume. Use <see cref="ConsumeItemOrEquippedByTemplateKey" /> when the templateKey is the stable identifier.</summary>
    public QuestStepBuilder<TStage> ConsumeItemOrEquipped(string name, int count)
        => AppendGuard(ctx =>
        {
            if (ctx.HasItem(name, count))
                return ctx.Source.Inventory.RemoveQuantity(name, count);

            if ((count == 1) && ctx.Source.Equipment.TryGetRemove(name, out _))
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
    public QuestStepBuilder<TStage> ConsumeItemsByTemplateKey(params IReadOnlyCollection<(string TemplateKey, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (key, count) in items)
                if (!ctx.HasItemByTemplateKey(key, count))
                    return false;

            foreach (var (key, count) in items)
                ctx.Source.Inventory.RemoveQuantityByTemplateKey(key, count);

            return true;
        });

    /// <summary>
    /// Pre-check every (name, count) pair; halt if any insufficient. Otherwise consume all atomically.
    /// Use <see cref="ConsumeItemsByTemplateKey" /> when templateKeys are the stable identifiers.
    /// </summary>
    public QuestStepBuilder<TStage> ConsumeItems(params IReadOnlyCollection<(string Name, int Count)> items)
        => AppendGuard(ctx =>
        {
            foreach (var (name, count) in items)
                if (!ctx.HasItem(name, count))
                    return false;

            foreach (var (name, count) in items)
                ctx.Source.Inventory.RemoveQuantity(name, count);

            return true;
        });

    // ===== Reward operations =====

    /// <summary>Award the source <paramref name="amount" /> experience via the active IExperienceDistributionScript.</summary>
    public QuestStepBuilder<TStage> GiveExperience(long amount)
        => Append(ctx => DefaultExperienceDistributionScript.Create().GiveExp(ctx.Source, amount));

    /// <summary>Award the source <paramref name="amount" /> ability experience via the active IAbilityDistributionScript.</summary>
    public QuestStepBuilder<TStage> GiveAbility(long amount)
        => Append(ctx => DefaultAbilityDistributionScript.Create().GiveAbility(ctx.Source, amount));

    /// <summary>Add <paramref name="amount" /> gold to the source. Chain continues even if TryGiveGold returns false.</summary>
    public QuestStepBuilder<TStage> GiveGold(int amount) => Append(ctx => ctx.Source.TryGiveGold(amount));

    /// <summary>
    /// Halt the chain unless Source has at least <paramref name="amount" /> gold. Pure check —
    /// does not consume. When <paramref name="failureReply" /> is provided, the failure path
    /// also sends a Reply.
    /// </summary>
    public QuestStepBuilder<TStage> RequireGold(int amount, string? failureReply = null)
        => AppendGuard(ctx => ctx.HasGold(amount), failureReply);

    /// <summary>
    /// Halt the chain unless Source can give up <paramref name="amount" /> gold; otherwise consume.
    /// Note: on failure the underlying <c>TryTakeGold</c> sends a system "not enough gold"
    /// orange-bar message. To present a quest-specific reply instead, gate with
    /// <see cref="RequireGold" /> first.
    /// </summary>
    public QuestStepBuilder<TStage> ConsumeGold(int amount)
        => AppendGuard(ctx => ctx.Source.TryTakeGold(amount));

    /// <summary>Add <paramref name="amount" /> game points to the source. Chain continues unconditionally.</summary>
    public QuestStepBuilder<TStage> GiveGamePoints(int amount) => Append(ctx => ctx.Source.GamePoints += amount);

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
    public QuestStepBuilder<TStage> RequireLevel(int min, int max = int.MaxValue, string? failureReply = null)
        => AppendGuard(
            ctx =>
            {
                var level = ctx.Source.UserStatSheet.Level;

                return (level >= min) && (level <= max);
            },
            failureReply);

    /// <summary>Halt the chain unless Source has mastered (UserStatSheet.Master is true).</summary>
    public QuestStepBuilder<TStage> RequireMaster(string? failureReply = null)
        => AppendGuard(ctx => ctx.Source.UserStatSheet.Master, failureReply);

    /// <summary>
    /// Halt the chain unless Source is a full grandmaster — both mastered and at the configured
    /// max level (<see cref="WorldOptions" />.MaxLevel).
    /// </summary>
    public QuestStepBuilder<TStage> RequireFullGrandmaster(string? failureReply = null)
        => AppendGuard(
            ctx => ctx.Source.UserStatSheet.Master && (ctx.Source.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel),
            failureReply);

    /// <summary>Halt the chain unless Source is mastered AND BaseClass equals <paramref name="baseClass" />.</summary>
    public QuestStepBuilder<TStage> RequirePureMaster(BaseClass baseClass, string? failureReply = null)
        => AppendGuard(
            ctx => ctx.Source.UserStatSheet.Master && (ctx.Source.UserStatSheet.BaseClass == baseClass),
            failureReply);

    /// <summary>
    /// Halt the chain unless Source's BaseClass matches <paramref name="baseClass" />. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply. For
    /// class-gated branching (no halt on mismatch), compose with <see cref="Branch" /> instead
    /// (e.g. <c>Branch(c =&gt; c.Source.UserStatSheet.BaseClass == X, body)</c>).
    /// </summary>
    public QuestStepBuilder<TStage> RequireClass(BaseClass baseClass, string? failureReply = null)
        => AppendGuard(ctx => ctx.Source.UserStatSheet.BaseClass == baseClass, failureReply);

    /// <summary>
    /// Halt the chain unless Source's Gender matches <paramref name="gender" />. When
    /// <paramref name="failureReply" /> is provided, the failure path also sends a Reply. For
    /// gender-gated branching (no halt on mismatch), compose with <see cref="Branch" /> instead
    /// (e.g. <c>Branch(c =&gt; c.Source.Gender == X, body)</c>).
    /// </summary>
    public QuestStepBuilder<TStage> RequireGender(Gender gender, string? failureReply = null)
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
    public QuestStepBuilder<TStage> Reply(string text)
    {
        Operations.Add(ctx =>
        {
            ctx.Subject?.Reply(ctx.Source, text);

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
    public QuestStepBuilder<TStage> ShowOption(string text, string dialogKey)
        => Append(ctx =>
        {
            if (ctx.Subject is null)
                return;

            if (!ctx.Subject.HasOption(text))
                ctx.Subject.AddOption(text, dialogKey);
        });

    /// <summary>
    /// Insert a dialog option pointing to <paramref name="dialogKey" /> at <paramref name="index" />
    /// if not already present (default index 0 places the option at the top of the menu — useful
    /// when injecting a quest hook into an NPC's pre-existing main menu). Chain continues. No-ops
    /// if ctx.Subject is null.
    /// </summary>
    public QuestStepBuilder<TStage> InsertOption(string text, string dialogKey, int index = 0)
        => Append(ctx =>
        {
            if (ctx.Subject is null)
                return;

            if (!ctx.Subject.HasOption(text))
                ctx.Subject.InsertOption(index, text, dialogKey);
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
    public QuestStepBuilder<TStage> SendOrangeBar(string text) => Append(ctx => ctx.Source.SendActiveMessage(text));

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
    public QuestStepBuilder<TStage> Teleport(string mapKey, int x, int y)
        => Append(ctx => TeleportCore(ctx, mapKey, new Point(x, y)));

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
    public QuestStepBuilder<TStage> GroupTeleport(string mapKey, int x, int y)
        => Append(ctx => GroupTeleportCore(ctx, mapKey, new Point(x, y)));

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
