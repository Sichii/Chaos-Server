using Chaos.Containers;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MapScripts.Abstractions;

public abstract class MapScriptBase : ScriptBase, IMapScript
{
    protected MapInstance Subject { get; }

    protected MapScriptBase(MapInstance subject) => Subject = subject;

    /// <inheritdoc />
    public virtual void OnEntered(Creature creature) { }

    /// <inheritdoc />
    public virtual void OnExiting(Creature creature) { }
}