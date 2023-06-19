using Chaos.Models.Panel;
using Chaos.Scripting.CreatureScripts.Abstractions;

namespace Chaos.Scripting.AislingScripts.Abstractions;

public interface IAislingScript : ICreatureScript
{
    bool CanUseItem(Item item);
}