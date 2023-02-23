using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public interface IMapScript : IScript, IDeltaUpdatable
{
    void OnEntered(Creature creature);
    void OnExited(Creature creature);
}