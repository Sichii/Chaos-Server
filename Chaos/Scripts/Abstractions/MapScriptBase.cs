using Chaos.Containers;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Abstractions;

public abstract class MapScriptBase : ScriptBase, IMapScript
{
    protected MapInstance Map { get; }

    protected MapScriptBase(MapInstance map) => Map = map;
    
    /// <inheritdoc />
    public virtual void OnEntered(Creature creature) { }

    /// <inheritdoc />
    public virtual void OnExiting(Creature creature) { }
}