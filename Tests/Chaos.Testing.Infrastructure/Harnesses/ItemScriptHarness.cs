#region
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Fluent test harness for testing item scripts
/// </summary>
public sealed class ItemScriptHarness<TScript> : ScriptHarnessBase<ItemScriptHarness<TScript>> where TScript: class, IItemScript
{
    /// <summary>
    ///     The script under test
    /// </summary>
    public TScript Script { get; }

    /// <summary>
    ///     The item subject the script is attached to
    /// </summary>
    public Item Subject { get; }

    public ItemScriptHarness(
        Func<Item, TScript>? scriptFactory = null,
        Action<Item>? itemSetup = null,
        IServiceProvider? serviceProvider = null)
        : base(serviceProvider)
    {
        Subject = MockItem.Create(setup: itemSetup);

        Script = scriptFactory != null ? scriptFactory(Subject) : CreateScript<TScript>(Subject);
    }

    /// <summary>
    ///     Calls CanUse on the script
    /// </summary>
    public bool CanUse() => Script.CanUse(Source);

    /// <summary>
    ///     Calls OnUse on the script
    /// </summary>
    public void Use() => Script.OnUse(Source);
}