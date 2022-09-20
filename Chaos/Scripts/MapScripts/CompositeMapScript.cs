using Chaos.Containers;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.MapScripts.Abstractions;

namespace Chaos.Scripts.MapScripts;

public class CompositeMapScript : CompositeScriptBase<IMapScript>, IMapScript
{
    protected MapInstance Subject { get; }

    public CompositeMapScript(MapInstance subject) => Subject = subject;

    /// <inheritdoc />
    public void OnEntered(Creature creature)
    {
        foreach (var component in Components)
            component.OnEntered(creature);
    }

    /// <inheritdoc />
    public void OnExiting(Creature creature)
    {
        foreach (var component in Components)
            component.OnExiting(creature);
    }
}