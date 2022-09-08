using Chaos.Containers;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MapScripts;

public class CompositeMapScript : CompositeScriptBase<IMapScript>, IMapScript
{
    protected MapInstance Source { get; }

    public CompositeMapScript(MapInstance source) => Source = source;

    /// <inheritdoc />
    public void OnEntered(Creature creature)
    {
        foreach(var component in Components)
            component.OnEntered(creature);
    }

    /// <inheritdoc />
    public void OnExiting(Creature creature)
    {
        foreach(var component in Components)
            component.OnExiting(creature);
    }
}