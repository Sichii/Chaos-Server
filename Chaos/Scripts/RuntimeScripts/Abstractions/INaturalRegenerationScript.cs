using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.Abstractions;

public interface INaturalRegenerationScript : IScript
{
    void Regenerate(Creature creature);
}