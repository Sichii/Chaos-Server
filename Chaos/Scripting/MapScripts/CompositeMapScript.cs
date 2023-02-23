using System.Runtime.InteropServices;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeMapScript : CompositeScriptBase<IMapScript>, IMapScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnEntered(Creature creature)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnEntered(creature);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnExited(Creature creature)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnExited(creature);
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}