using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.Abstractions;

public interface ILevelUpScript : IScript
{
    void LevelUp(Aisling source);
}