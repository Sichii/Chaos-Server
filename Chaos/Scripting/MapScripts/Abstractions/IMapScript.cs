using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MapScripts.Abstractions;

public interface IMapScript : IScript, IDeltaUpdatable
{
    void OnEntered(Creature creature);
    void OnExited(Creature creature);
}