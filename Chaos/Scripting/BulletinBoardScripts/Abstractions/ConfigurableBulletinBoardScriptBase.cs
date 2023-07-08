using Chaos.Collections;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts.Abstractions;

public class ConfigurableBulletinBoardScriptBase : ConfigurableScriptBase<BulletinBoard>, IBulletinBoardScript
{
    /// <inheritdoc />
    public ConfigurableBulletinBoardScriptBase(BulletinBoard subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool AllowedToDelete(Aisling aisling, Post post) => true;

    /// <inheritdoc />
    public virtual bool AllowedToHighlight(Aisling aisling) => true;

    /// <inheritdoc />
    public virtual bool AllowedToPost(Aisling aisling) => true;

    /// <inheritdoc />
    public virtual bool AllowedToView(Aisling aisling) => true;

    /// <inheritdoc />
    public virtual bool ShouldRejectPost(Aisling aisling, Post post, [MaybeNullWhen(false)] out string reason)
    {
        reason = null;

        return false;
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}