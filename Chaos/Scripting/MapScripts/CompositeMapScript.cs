using System.Runtime.InteropServices;
using Chaos.Models.World.Abstractions;
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
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnEntered(creature);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnExited(Creature creature)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnExited(creature);
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}