#region
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Fluent test harness for testing monster scripts
/// </summary>
public sealed class MonsterScriptHarness<TScript> : ScriptHarnessBase<MonsterScriptHarness<TScript>> where TScript: class, IMonsterScript
{
    /// <summary>
    ///     The script under test
    /// </summary>
    public TScript Script { get; }

    /// <summary>
    ///     The monster subject the script is attached to
    /// </summary>
    public Monster Subject { get; }

    public MonsterScriptHarness(
        Func<Monster, TScript>? scriptFactory = null,
        Action<Monster>? monsterSetup = null,
        IServiceProvider? serviceProvider = null)
        : base(serviceProvider)
    {
        Subject = MockMonster.Create(Map, setup: monsterSetup);

        Script = scriptFactory != null ? scriptFactory(Subject) : CreateScript<TScript>(Subject);
    }

    /// <summary>
    ///     Calls OnAttacked on the script
    /// </summary>
    public void Attack(Creature attacker, int damage, int? aggroOverride = null) => Script.OnAttacked(attacker, damage, aggroOverride);

    /// <summary>
    ///     Calls OnClicked on the script
    /// </summary>
    public void Click(Aisling clicker) => Script.OnClicked(clicker);

    /// <summary>
    ///     Calls OnSpawn on the script
    /// </summary>
    public void Spawn() => Script.OnSpawn();

    /// <summary>
    ///     Calls Update on the script
    /// </summary>
    public void Update(TimeSpan delta) => Script.Update(delta);
}