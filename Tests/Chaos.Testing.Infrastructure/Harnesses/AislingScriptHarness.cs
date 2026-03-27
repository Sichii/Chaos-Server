#region
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Fluent test harness for testing aisling scripts
/// </summary>
public sealed class AislingScriptHarness<TScript> : ScriptHarnessBase<AislingScriptHarness<TScript>> where TScript: class, IAislingScript
{
    /// <summary>
    ///     The script under test. The subject is the Source aisling.
    /// </summary>
    public TScript Script { get; }

    public AislingScriptHarness(Func<Aisling, TScript>? scriptFactory = null, IServiceProvider? serviceProvider = null)
        : base(serviceProvider)
        => Script = scriptFactory != null ? scriptFactory(Source) : CreateScript<TScript>(Source);

    /// <summary>
    ///     Calls OnAttacked on the script
    /// </summary>
    public void Attacked(Creature source, int damage) => Script.OnAttacked(source, damage);

    /// <summary>
    ///     Calls OnClicked on the script
    /// </summary>
    public void Click(Aisling clicker) => Script.OnClicked(clicker);

    /// <summary>
    ///     Calls OnDeath on the script
    /// </summary>
    public void Death() => Script.OnDeath();

    /// <summary>
    ///     Calls Update on the script
    /// </summary>
    public void Update(TimeSpan delta) => Script.Update(delta);
}