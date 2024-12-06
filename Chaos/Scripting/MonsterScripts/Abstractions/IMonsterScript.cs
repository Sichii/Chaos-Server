#region
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.CreatureScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
#endregion

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MonsterScripts.Abstractions;

public interface IMonsterScript : ICreatureScript
{
    /// <summary>
    ///     Triggers when the monster is attacked by a creature. Monsters can have "aggro", so an aggro override is provided
    /// </summary>
    /// <param name="source">
    ///     The source of the attack
    /// </param>
    /// <param name="damage">
    ///     The final damage dealt
    /// </param>
    /// <param name="aggroOverride">
    ///     An amount of aggro produced, regardless of final damage
    /// </param>
    void OnAttacked(Creature source, int damage, int? aggroOverride);

    /// <summary>
    ///     Triggers when the monster first spawns (the monster is on the map)
    /// </summary>
    /// <remarks>
    ///     Comes after <see cref="IMapScript.OnEntered" />
    ///     <br />
    ///     Comes after <see cref="ICreatureScript.OnApproached" />
    /// </remarks>
    void OnSpawn();
}