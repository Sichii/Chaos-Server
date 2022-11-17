using System.Runtime.InteropServices;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MapScripts.Abstractions;

namespace Chaos.Scripts.MapScripts;

public class CompositeMapScript : CompositeScriptBase<IMapScript>, IMapScript
{
    /// <inheritdoc />
    public virtual void OnEntered(Creature creature)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnEntered(creature);
    }

    /// <inheritdoc />
    public virtual void OnExiting(Creature creature)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnExiting(creature);
    }
}