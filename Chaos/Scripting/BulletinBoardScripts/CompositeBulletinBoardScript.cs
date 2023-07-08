using System.Runtime.InteropServices;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeBulletinBoardScript : CompositeScriptBase<IBulletinBoardScript>, IBulletinBoardScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool AllowedToDelete(Aisling aisling, Post post)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.AllowedToDelete(aisling, post))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool AllowedToHighlight(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.AllowedToHighlight(aisling))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool AllowedToPost(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.AllowedToPost(aisling))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool AllowedToView(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.AllowedToView(aisling))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool ShouldRejectPost(Aisling aisling, Post post, [MaybeNullWhen(false)] out string reason)
    {
        reason = null;

        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (script.ShouldRejectPost(aisling, post, out reason))
                return true;

        return false;
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}