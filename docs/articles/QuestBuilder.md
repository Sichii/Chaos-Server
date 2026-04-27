# QuestBuilder

The `QuestBuilder` is a fluent framework for declaring quests as a tree of dialog reactions and operation chains.
It replaces hand-rolled `IDialogScript` implementations for quest content with a declarative configuration that runs
inside the dialog lifecycle. The framework can be found under `Chaos.Utilities.QuestHelper`.

A quest is a subclass of [Quest<TStage>](<xref:Chaos.Utilities.QuestHelper.Quest`1>), where `TStage` is a fork-defined
enum describing the named stages the player can be in. Stage values are persisted on the player's
`Trackers.Enums`, so progression survives across sessions automatically.

> [!NOTE]
> Quest subclasses are auto-discovered by
> [QuestRegistry](<xref:Chaos.Services.Quests.QuestRegistry>) at startup — there is no manual
> registration step. Any non-abstract subclass of `Quest` in any loaded assembly is instantiated, configured, and
> indexed by its dialog handlers.

### Defining a Quest

Subclass `Quest<TStage>`, expose a `Key`, and override `Configure` to register dialog handlers via the supplied
[QuestBuilder<TStage>](<xref:Chaos.Utilities.QuestHelper.QuestBuilder`1>):

```csharp
public enum LostRingStage { Start, Accepted, Returned, Complete }

public sealed class LostRingQuest : Quest<LostRingStage>
{
    public override string Key => "lost_ring";

    protected override void Configure(QuestBuilder<LostRingStage> q)
    {
        q.OnNext("rosa_initial")
            .WhenNeverStarted("Hello, traveler.")
            .Advance(LostRingStage.Accepted)
            .Reply("Please find my ring near the river.");

        q.OnNext("rosa_initial")
            .WhenAt(LostRingStage.Accepted, "Have you found my ring yet?")
            .RequireItemByTemplateKey("ring", count: 1, "I don't see it on you.")
            .ConsumeItemByTemplateKey("ring", count: 1)
            .Advance(LostRingStage.Returned)
            .GiveExperience(5_000)
            .GiveGold(500)
            .Reply("Thank you so much!");
    }
}
```

> [!TIP]
> `Configure` runs once per quest at registry startup, building an immutable handler table. Per-player evaluation
> happens at dialog time on a fresh [QuestContext](<xref:Chaos.Utilities.QuestHelper.QuestContext`1>) constructed by
> the framework — your handler chains hold no per-player state.

### Dialog Phases

`QuestBuilder<TStage>` exposes one method per dialog lifecycle phase. The phase determines *when* during the dialog
flow your chain runs:

- **`OnDisplaying(templateKey)`** — runs as the dialog is about to be sent. Use this for option injection
  (`ShowOption`) and text templating. State mutations placed here fire *before* the player sees the dialog and are
  almost always wrong.
- **`OnDisplayed(templateKey)`** — runs after the dialog has been sent. Rarely the right phase; kept for parity with
  [IDialogScript.OnDisplayed](<xref:Chaos.Scripting.DialogScripts.Abstractions.IDialogScript>).
- **`OnNext(templateKey)`** — runs when the player advances the dialog (Next button, option click, menu selection).
  This is the commit phase — `Advance`, `GiveItem`, `ConsumeItemByTemplateKey`, `GiveExperience`, and other state mutations belong
  here.
- **`OnPrevious(templateKey)`** — runs when the player backs up via the Previous button on a Normal dialog.

Multiple quests can register handlers against the same `(templateKey, phase)` pair; all are evaluated in registration
order.

### Step Operations

Each phase method returns a [QuestStepBuilder<TStage>](<xref:Chaos.Utilities.QuestHelper.QuestStepBuilder`1>) that
records an *operation chain*. Operations are evaluated in order; each returns `true` to continue the chain or `false`
to halt (e.g., a guard failed). Operations fall into the categories below.

- **Stage** — `WhenAt(stage)`, `WhenAtAny(stage1, stage2, ...)`, `WhenNeverStarted()`, `Advance(stage)`, `ClearStage()`, `RouteByStage(map)`.
- **Sub-stage** — orthogonal `TSub`-typed enum tracked alongside the primary stage. `WhenAtSub<TSub>(value)`,
  `WhenAtAnySub<TSub>(value1, value2, ...)`, `WhenSubNeverStarted<TSub>()`, `AdvanceSub<TSub>(value)`,
  `ClearSub<TSub>()`, `RouteBySub<TSub>(map)`. Useful for nested phases inside a stage (e.g. crafting
  steps, riddle attempts, multi-part escort objectives) without proliferating top-level `TStage` values.
- **Flags** — `SetFlag`, `ClearFlag`, `RequireFlag`, `RequireAllFlags`, `RequireAnyFlag`. Each method has overloads
  for both regular `[Flags]` enums (stored in `Trackers.Flags`) and `BigFlagsValue<TMarker>` values (stored in
  `Trackers.BigFlags`); the compiler picks the right overload by argument type.
- **Counters** — `IncrementCounter`, `DecrementCounter`, `SetCounter`, `ClearCounter`, plus `RequireCountGreaterThanOrEqualTo` / `RequireCountLessThanOrEqualTo` / `RequireCountEqualTo` / `RequireCountLessThan` / `RequireCountGreaterThan` guards. Counters are arbitrary integer trackers keyed by string — kill counts, items collected, repeat-completion tallies, etc.
- **Cooldowns** — `StartCooldown(key, duration)`, `RequireCooldownExpired(key)`. The string-template overload of the
  latter accepts `"{remaining}"` and substitutes a readable remaining-time string into the failure reply.
- **Items** — `GiveItem`, `GiveItems`, `RequireItemByTemplateKey`, `RequireItem`, `RequireItemsByTemplateKey`,
  `RequireItems`, `ConsumeItemByTemplateKey`, `ConsumeItem`, `ConsumeItemsByTemplateKey`, `ConsumeItems`,
  the `*OrEquipped*` variants that fall back to the equipment panel, and the equipment-only
  `RequireEquipped` / `RequireEquippedByTemplateKey` / `ConsumeEquipped` / `ConsumeEquippedByTemplateKey`
  for outfit, insignia, or "must currently be wearing" checks. The batch `ConsumeItemsByTemplateKey` /
  `ConsumeItems` operations are atomic: every required pair is verified before any are removed.
- **Rewards** — `GiveExperience`, `GiveAbility`, `GiveGold`, `GiveGamePoints`, `GiveOrAccumulateLegendMark`, `GiveUniqueLegendMark`, `GiveSkill`, `GiveSpell`, `RemoveSkill`, `RemoveSpell`.
- **Costs/checks** — `RequireGold`, `ConsumeGold`.
- **Class / Level / Gender guards** — `RequireLevel(min, max)`, `RequireMaster`, `RequireFullGrandmaster`,
  `RequirePureMaster(baseClass)`, `RequireClass(baseClass)`, `RequireGender(gender)`.
- **Communication** — `Reply(text)` and `Skip(dialogKey)` halt the chain after sending; `ShowOption(text, dialogKey)`
  appends an option without halting and `InsertOption(text, dialogKey, index)` inserts at a specific position;
  `InjectTextParameters(...)` (variadic or `Func<QuestContext<TStage>, object[]>` selector form) seeds
  parameters used by the dialog template's text formatting; `SendOrangeBar(text)` sends an active message.
- **Teleports** — `Teleport(mapKey, x, y)`, `Teleport(mapKey, rect)` (random walkable point), and `GroupTeleport`
  variants that move the player's full party.
- **Escape hatch** — `Run(action)` invokes an arbitrary `Action<Aisling, QuestContext<TStage>>` and continues the
  chain.

### Value-or-resolver parameters

Most builder methods that take a numeric, text, or duration *value* accept a `QuestArg<T>`. Pass a
literal (lifted via implicit conversion) for the common case, or `QuestArg.From(ctx => ...)` to
defer evaluation until the chain runs:

```csharp
q.OnDisplaying("morris_accept")
    .WhenNeverStarted()
    .SetCounter("spotIndex", QuestArg.From(_ => Random.Shared.Next(1, 22)))
    .GiveGold(QuestArg.From(c => 50 + (c.Source.UserStatSheet.Level * 5)))
    .Reply(QuestArg.From(c => $"Take this, {c.Source.Name}."))
    .Advance(StowawayHunt.Started);
```

Resolvers receive the non-generic `QuestContext` base, which exposes `Source`, `Subject`,
`OptionIndex`, and `Services`. To read `CurrentStage` or stage-typed predicates, cast inside the
lambda: `((QuestContext<MyStage>)ctx).CurrentStage`.

Methods with non-zero default values (`IncrementCounter(by = 1)`, `DecrementCounter(by = 1)`,
`GiveItem(count = 1)`, `RequireLevel(max = int.MaxValue)`) keep their original literal-only
signature *plus* a `QuestArg<int>` overload — call with a literal to use the default-aware form,
or pass `QuestArg.From(...)` to use the resolver form.

Optional `failureReply` parameters on guards are typed `QuestArg<string>?` — pass `null` (default)
for no message, a literal for a static message, or `QuestArg.From(c => ...)` for a dynamic message.
Identifier-style strings (`key`, `templateKey`, `name`, `dialogKey`, `mapKey`) remain plain `string`;
they're lookup identifiers, not values — runtime resolution there isn't a real-world need.

Collection-shaped methods (`WhenAtAny`, `WhenAtAnySub`, `RouteByStage`, `RouteBySub`, `GiveItems`,
`RequireItems`, `RequireItemsByTemplateKey`, `ConsumeItems`, `ConsumeItemsByTemplateKey`) keep their
existing `params`/dictionary signatures *plus* a parallel `QuestArg<IReadOnlyCollection<…>>` /
`QuestArg<IReadOnlyDictionary<…>>` overload for runtime-built lists:

```csharp
.GiveItems(QuestArg.From(c =>
{
    c.Source.Trackers.Counters.TryGetValue("kills", out var kills);
    return (IReadOnlyCollection<(string, int)>)[("wolfsfur", kills), ("wolfsclaw", kills * 2)];
}))
```

The literal `params` form is preferred by overload resolution when both could match.

### Matching multiple stages

When a handler should fire on more than one stage, use `WhenAtAny` (or
`WhenAtAnySub` for sub-stages):

```csharp
q.OnNext("npc_greeting")
 .WhenAtAny(WolfStage.Hunting, WolfStage.Returning)
 .Reply("Have you found anything yet?");
```

Both methods accept variadic args or a pre-built collection. Empty input halts the chain.

A `failureReply` overload is available on each: `WhenAtAny(failureReply, stages)` /
`WhenAtAnySub<TSub>(failureReply, values)` send the supplied text via `Reply` when the
guard fails, mirroring the single-value `WhenAt(stage, failureReply)` overload.

### Name vs. template key

Item methods come in two flavors. The simple-name form (`RequireItem(name, ...)`,
`ConsumeItem(name, ...)`, etc.) matches by display name and is convenient for
deliverables whose name is stable and unique within the working content. The
`*ByTemplateKey` form matches by the static template identifier and is the right
choice when names may collide, when names may be localized in the future, or when
the quest already holds the templateKey.

### Unique vs. accumulating legend marks

`GiveOrAccumulateLegendMark(text, key, icon, color)` calls `Legend.AddOrAccumulate` —
if a mark with the same key already exists, its count is incremented.
`GiveUniqueLegendMark(text, key, icon, color)` calls `Legend.AddUnique` —
the existing mark is replaced in place, count reset to 1. Use the unique
variant for one-shot quest milestones whose count should not stack on
repeat triggers.

### Branching

`Branch(predicate, configure)` runs a sub-chain only when its predicate matches; the outer chain continues regardless.
A trailing `Otherwise(configure)` runs only if no preceding `Branch` on the same chain matched, giving an if/else-if
shape:

```csharp
q.OnDisplaying("riddle_master_initial")
    .WhenAt(RiddleStage.Asked)
    .Branch(
        predicate: c => c.Source.UserStatSheet.BaseClass == BaseClass.Wizard,
        configure: sb => sb.ShowOption("Answer with magic", "riddle_master_wizard"))
    .Branch(
        predicate: c => c.Source.UserStatSheet.BaseClass == BaseClass.Warrior,
        configure: sb => sb.ShowOption("Answer with might", "riddle_master_warrior"))
    .Otherwise(
        configure: sb => sb.ShowOption("Answer plainly", "riddle_master_default"));
```

> [!NOTE]
> A `Branch` predicate matching does *not* halt the outer chain — only a guard or `Reply`/`Skip` inside the sub-chain
> can. This makes branches composable: an `OnDisplaying` chain can inject several class-specific options without any
> single match preempting the rest of the page setup.

### Failure Replies

Every guard method (`WhenAt`, `RequireItemByTemplateKey`, `RequireFlag`, `RequireLevel`, etc.) accepts an optional `failureReply`
argument. When provided, a failed guard sends a Dialog `Reply` with that message before halting. When omitted, the
guard halts silently — useful when a different handler on the same dialog template is expected to fire instead.

```csharp
q.OnNext("guard_gate_initial")
    .RequireLevel(min: 50, max: int.MaxValue, failureReply: "You are too inexperienced.")
    .RequireItemByTemplateKey("permit", count: 1, failureReply: "Where is your permit?")
    .Skip("guard_gate_admit");
```

### Quest Context

Predicates passed to `Branch` and callbacks passed to `Run` receive a
[QuestContext<TStage>](<xref:Chaos.Utilities.QuestHelper.QuestContext`1>). The context is
**read-only** — it carries the player's current state and the predicates needed to gate behavior.
All mutations live on the fluent builder.

**Core state**
- **`Source`** — the `Aisling` interacting with the dialog.
- **`Subject`** — the active `Dialog?` (null if the chain runs outside a dialog flow).
- **`OptionIndex`** — for option-driven dialogs, the byte index of the option the player picked.
- **`CurrentStage`** — the player's current `TStage` value, kept in sync with `Trackers.Enums`.
- **`NeverStarted`** — true when no value of `TStage` has ever been written for this player.
- **`Services`** — the `IServiceProvider` used to lazy-resolve framework dependencies (`IItemFactory`,
  `ISkillFactory`, `ISpellFactory`, `ISimpleCache<MapInstance>`).

**Predicates** (mirroring the `Require*` guards, available for custom `Branch` conditions)
- **Stage** — `WhenAt(stage)`, `WhenAtAny(stages)`.
- **Sub-stage** — `WhenAtSub<TSub>(value)`, `WhenAtAnySub<TSub>(values)`, `HasNoSub<TSub>()`,
  `TryGetSub<TSub>(out value)`.
- **Flags** — `HasFlag`, `HasAllFlags`, `HasAnyFlag` (regular and `BigFlagsValue<TMarker>` overloads).
- **Counters & cooldowns** — `CounterGreaterThanOrEqualTo(key, value)`, `CounterLessThanOrEqualTo(key, value)`, `CounterEqualTo(key, value)`, `CounterLessThan(key, value)`, `CounterGreaterThan(key, value)`, `HasActiveCooldown(key, out remaining)`.
- **Items** — `HasItemByTemplateKey`, `HasItem`, `HasItemOrEquippedByTemplateKey`, `HasItemOrEquipped`,
  `HasEquippedByTemplateKey`, `HasEquipped`.
- **Gold** — `HasGold(amount)`.

> [!IMPORTANT]
> The context exposes **predicates only** — there are no mutation methods on `QuestContext`.
> All state changes live on `QuestStepBuilder<TStage>`. From a `Run` callback, either call back
> into a builder method (preferred — keeps your behavior composable across the chain) or write
> through `ctx.Source` directly (e.g. `ctx.Source.Trackers.Enums.Set(stage)`,
> `ctx.Source.Inventory.RemoveQuantity(...)`). If you find yourself reaching for `Run` repeatedly
> for the same operation, that's a signal to add a fluent builder method instead.

> [!TIP]
> For most quests, every gating concern is expressible with the named guards. Reach for `Run` and bare `Branch`
> predicates only for behavior that genuinely doesn't fit — keeping operations on the named API makes a quest's
> requirements legible to anyone reading its `Configure`.
