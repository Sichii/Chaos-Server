# SequenceScripter

The `SequenceScripter` is a fluent builder framework for composing entity behavior over time without hand-rolling tick
logic. It is intended for scripts that need to perform actions at intervals, react to entity state, or progress through
multi-step sequences — most commonly monster AI, but it works for any entity type. The framework can be found under
`Chaos.Utilities.SequenceScripter`.

There are 2 builders to choose from. The
[ScriptBuilder<T>](<xref:Chaos.Utilities.SequenceScripter.Builder.ScriptBuilder`1>) works against any entity type. The
[CreatureScriptBuilder<T>](<xref:Chaos.Utilities.SequenceScripter.Builder.CreatureScriptBuilder`1>) extends that builder
with health-percent threshold actions and is constrained to `Creature`-derived entities.

> [!NOTE]
> Both builders produce a [ScriptedSequence<T>](<xref:Chaos.Utilities.SequenceScripter.ScriptedSequence`1>) which
> implements `IDeltaUpdatable`. Drive it from your script's `Update(TimeSpan)` to keep its actions advancing.

### Action Categories

`ScriptBuilder<T>` exposes 2 families of actions, each with a "do once" form (the action is removed after firing) and a
"repeat" form (the action stays registered and fires whenever its trigger is met):

- **Conditional** — driven by a `Func<T, bool>` predicate. Use `WhenThenDoActionOnce` / `WhileThenRepeatAction` for a
  single action, and `WhenThenDoSequenceOnce` / `WhileThenRepeatSequence` for a multi-step sequence.
- **Timed** — driven by an interval. Use `AfterTimeDoActionOnce` / `AfterTimeRepeatAction` for a single action, and
  `AfterTimeDoSequenceOnce` / `AfterTimeRepeatSequence` for a multi-step sequence.

`CreatureScriptBuilder<T>` adds 2 more families that watch `Creature.StatSheet.HealthPercent`:

- **Threshold** — fires when health drops through a percentage. Use `AtThresholdDoActionOnce` /
  `AtThresholdRepeatAction` for a single action, and `AtThresholdDoSequenceOnce` / `AtThresholdRepeatSequence` for a
  sequence.
- **Threshold + Delay** — fires after the health threshold is crossed *and* a delay elapses, useful for telegraphed
  phase-change abilities. Use `AtThresholdThenAfterTimeDoActionOnce` / `AtThresholdThenAfterTimeRepeatAction` and the
  matching `*Sequence*` variants.

> [!TIP]
> Every builder method returns the builder itself, so calls can be chained. The naming convention is intentionally
> verbose so a chained script reads close to English: `WhenThen…`, `WhileThen…`, `AfterTime…`, `AtThreshold…`.

### Sequences

Any of the `*Sequence*` overloads take a
[TimedActionSequenceBuilder<T>](<xref:Chaos.Utilities.SequenceScripter.Builder.TimedActionSequenceBuilder`1>). It chains
actions back-to-back with intervals between them via `AfterTime` and `ThenAfter` (the two methods are aliases —
`AfterTime` reads naturally for the first step, `ThenAfter` for subsequent ones).

```csharp
var sequence = new TimedActionSequenceBuilder<Monster>()
    .AfterTime(TimeSpan.FromSeconds(1), m => m.Say("Beware!"))
    .ThenAfter(TimeSpan.FromSeconds(2), m => m.AnimateBody(BodyAnimation.Assail))
    .ThenAfter(TimeSpan.FromSeconds(1), m => m.UseSkill(...));
```

> [!NOTE]
> Each step's interval is the time *since the previous step fired*, not the absolute time since the sequence started. A
> sequence completes when its last step fires; a "repeat" sequence then resets and runs again from the top.

### Building a Script

Below is an example monster behavior assembled from all four action families.

```csharp
var sequence = new CreatureScriptBuilder<Monster>(scriptUpdateInterval: TimeSpan.FromMilliseconds(100))
    // Wander while not engaged
    .WhileThenRepeatAction(
        condition: m => !m.Engaged,
        action:    m => m.Wander())

    // Cast a recurring buff every 30 seconds
    .AfterTimeRepeatAction(
        time:   TimeSpan.FromSeconds(30),
        action: m => m.TryUseSpell(buffSpell))

    // Enrage the moment it drops to 25% health (one-shot)
    .AtThresholdDoActionOnce(
        threshold: 25,
        action:    m => m.Enrage())

    // Telegraph a desperation cast 1 second after dropping below 10%
    .AtThresholdThenAfterTimeDoActionOnce(
        time:      TimeSpan.FromSeconds(1),
        threshold: 10,
        action:    m => m.TryUseSpell(desperationSpell))

    .Build(monster);
```

The resulting [ScriptedSequence<T>](<xref:Chaos.Utilities.SequenceScripter.ScriptedSequence`1>) is an `IDeltaUpdatable`,
so a script that owns it just forwards its update tick:

```csharp
public override void Update(TimeSpan delta) => sequence.Update(delta);
```

### Update Cadence

The builder constructor takes a `scriptUpdateInterval`. Internally the sequence keeps an `IIntervalTimer` and only
evaluates registered actions on update ticks at that cadence — *not* every game-loop frame. Smaller intervals give finer
reaction time at the cost of more frequent predicate evaluations.

> [!TIP]
> One-shot timed actions accept a `startAsElapsed` flag. The default is `false`, meaning the action waits the full
> interval before firing. Pass `true` to make it fire on the first update tick instead of after a delay.

### Threshold Semantics

Threshold actions arm themselves the first time `HealthPercent` crosses *down* through the configured threshold
(previous > threshold ≥ current). Once armed they do not disarm — healing back above the threshold will not re-arm them
on the next drop, and a "repeat" threshold action will continue firing every tick once armed. This matches the typical
"phase change" use case where the threshold marks a permanent transition rather than a recurring trigger.

Threshold-with-delay actions additionally start an internal timer when armed and only fire after the configured delay
elapses, allowing a wind-up between the trigger condition and the visible behavior change.
