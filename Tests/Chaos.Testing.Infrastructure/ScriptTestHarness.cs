#region
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Testing.Infrastructure.Harnesses;
#endregion

namespace Chaos.Testing.Infrastructure;

/// <summary>
///     Entry point for creating script test harnesses. Provides fluent factory methods for each script type.
/// </summary>
public static class ScriptTestHarness
{
    /// <summary>
    ///     Creates a harness for testing an aisling script.
    /// </summary>
    public static AislingScriptHarness<TScript> ForAislingScript<TScript>(
        Func<Aisling, TScript>? scriptFactory = null,
        IServiceProvider? serviceProvider = null) where TScript: class, IAislingScript
        => new(scriptFactory, serviceProvider);

    /// <summary>
    ///     Creates a harness for testing an item script.
    /// </summary>
    public static ItemScriptHarness<TScript> ForItemScript<TScript>(
        Func<Item, TScript>? scriptFactory = null,
        Action<Item>? itemSetup = null,
        IServiceProvider? serviceProvider = null) where TScript: class, IItemScript
        => new(scriptFactory, itemSetup, serviceProvider);

    /// <summary>
    ///     Creates a harness for testing a monster script.
    /// </summary>
    public static MonsterScriptHarness<TScript> ForMonsterScript<TScript>(
        Func<Monster, TScript>? scriptFactory = null,
        Action<Monster>? monsterSetup = null,
        IServiceProvider? serviceProvider = null) where TScript: class, IMonsterScript
        => new(scriptFactory, monsterSetup, serviceProvider);

    /// <summary>
    ///     Creates a harness for testing a skill script. Use the factory overload for scripts with complex DI dependencies.
    /// </summary>
    /// <param name="scriptFactory">
    ///     Optional factory to create the script. If null, uses ActivatorUtilities.CreateInstance.
    /// </param>
    /// <param name="skillSetup">
    ///     Optional action to configure the skill subject
    /// </param>
    /// <param name="serviceProvider">
    ///     Optional service provider for DI. Only used if scriptFactory is null.
    /// </param>
    public static SkillScriptHarness<TScript> ForSkillScript<TScript>(
        Func<Skill, TScript>? scriptFactory = null,
        Action<Skill>? skillSetup = null,
        IServiceProvider? serviceProvider = null) where TScript: class, ISkillScript
        => new(scriptFactory, skillSetup, serviceProvider);

    /// <summary>
    ///     Creates a harness for testing a spell script.
    /// </summary>
    public static SpellScriptHarness<TScript> ForSpellScript<TScript>(
        Func<Spell, TScript>? scriptFactory = null,
        Action<Spell>? spellSetup = null,
        IServiceProvider? serviceProvider = null) where TScript: class, ISpellScript
        => new(scriptFactory, spellSetup, serviceProvider);
}