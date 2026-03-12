#region
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Fluent test harness for testing spell scripts
/// </summary>
public sealed class SpellScriptHarness<TScript> : ScriptHarnessBase<SpellScriptHarness<TScript>> where TScript: class, ISpellScript
{
    /// <summary>
    ///     The target creature (if set)
    /// </summary>
    public Creature? Target { get; private set; }

    /// <summary>
    ///     The script under test
    /// </summary>
    public TScript Script { get; }

    /// <summary>
    ///     The spell subject the script is attached to
    /// </summary>
    public Spell Subject { get; }

    /// <summary>
    ///     The mock client for the target (if target is an Aisling)
    /// </summary>
    public Mock<IChaosWorldClient>? TargetClient => Target is Aisling a ? Mock.Get(a.Client) : null;

    public SpellScriptHarness(
        Func<Spell, TScript>? scriptFactory = null,
        Action<Spell>? spellSetup = null,
        IServiceProvider? serviceProvider = null)
        : base(serviceProvider)
    {
        Subject = MockSpell.Create(setup: spellSetup);

        Script = scriptFactory != null ? scriptFactory(Subject) : CreateScript<TScript>(Subject);
    }

    /// <summary>
    ///     Calls CanUse on the script with the current source and target
    /// </summary>
    public bool CanUse(string? promptResponse = null) => Script.CanUse(CreateContext(promptResponse));

    /// <summary>
    ///     Creates a SpellContext for this harness's source and target
    /// </summary>
    public SpellContext CreateContext(string? promptResponse = null)
    {
        var target = Target ?? Source;

        return new SpellContext(Source, target, promptResponse);
    }

    /// <summary>
    ///     Calls OnUse on the script with the current source and target
    /// </summary>
    public void Use(string? promptResponse = null) => Script.OnUse(CreateContext(promptResponse));

    /// <summary>
    ///     Set the target to a specific creature
    /// </summary>
    public SpellScriptHarness<TScript> WithTarget(Creature target)
    {
        Target = target;

        return this;
    }

    /// <summary>
    ///     Create a target aisling on the shared map
    /// </summary>
    public SpellScriptHarness<TScript> WithTargetAisling(Action<Aisling>? setup = null)
    {
        Target = MockAisling.Create(Map, setup: setup);

        return this;
    }

    /// <summary>
    ///     Create a target monster on the shared map
    /// </summary>
    public SpellScriptHarness<TScript> WithTargetMonster(Action<Monster>? setup = null)
    {
        Target = MockMonster.Create(Map, setup: setup);

        return this;
    }
}