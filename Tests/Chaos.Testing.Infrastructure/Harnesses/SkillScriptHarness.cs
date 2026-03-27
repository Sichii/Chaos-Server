#region
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Fluent test harness for testing skill scripts
/// </summary>
public sealed class SkillScriptHarness<TScript> : ScriptHarnessBase<SkillScriptHarness<TScript>> where TScript: class, ISkillScript
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
    ///     The skill subject the script is attached to
    /// </summary>
    public Skill Subject { get; }

    /// <summary>
    ///     The mock client for the target (if target is an Aisling)
    /// </summary>
    public Mock<IChaosWorldClient>? TargetClient => Target is Aisling a ? Mock.Get(a.Client) : null;

    public SkillScriptHarness(
        Func<Skill, TScript>? scriptFactory = null,
        Action<Skill>? skillSetup = null,
        IServiceProvider? serviceProvider = null)
        : base(serviceProvider)
    {
        Subject = MockSkill.Create(setup: skillSetup);

        Script = scriptFactory != null ? scriptFactory(Subject) : CreateScript<TScript>(Subject);
    }

    /// <summary>
    ///     Calls CanUse on the script with the current source and target
    /// </summary>
    public bool CanUse() => Script.CanUse(CreateContext());

    /// <summary>
    ///     Creates an ActivationContext for this harness's source and target
    /// </summary>
    public ActivationContext CreateContext()
    {
        if (Target is MapEntity mapEntity)
            return new ActivationContext(Source, mapEntity);

        return new ActivationContext(Source, Source, Map);
    }

    /// <summary>
    ///     Calls OnUse on the script with the current source and target
    /// </summary>
    public void Use() => Script.OnUse(CreateContext());

    /// <summary>
    ///     Set the target to a specific creature
    /// </summary>
    public SkillScriptHarness<TScript> WithTarget(Creature target)
    {
        Target = target;

        return this;
    }

    /// <summary>
    ///     Create a target aisling on the shared map
    /// </summary>
    public SkillScriptHarness<TScript> WithTargetAisling(Action<Aisling>? setup = null)
    {
        Target = MockAisling.Create(Map, setup: setup);

        return this;
    }

    /// <summary>
    ///     Create a target monster on the shared map
    /// </summary>
    public SkillScriptHarness<TScript> WithTargetMonster(Action<Monster>? setup = null)
    {
        Target = MockMonster.Create(Map, setup: setup);

        return this;
    }
}