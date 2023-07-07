using Chaos.Collections;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.BoardScripts;

public class BulletinBoardScriptBase : SubjectiveScriptBase<BulletinBoard>, IBulletinBoardScript
{
    /// <inheritdoc />
    public BulletinBoardScriptBase(BulletinBoard subject)
        : base(subject) { }
}