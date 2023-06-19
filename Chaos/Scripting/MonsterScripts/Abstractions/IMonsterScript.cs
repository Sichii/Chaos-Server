using Chaos.Models.World.Abstractions;
using Chaos.Scripting.CreatureScripts.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MonsterScripts.Abstractions;

public interface IMonsterScript : ICreatureScript
{
    void OnAttacked(Creature source, int damage, int? aggroOverride);
    void OnSpawn();
}