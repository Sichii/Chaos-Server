using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts.Abstractions;

public interface IBulletinBoardScript : IScript, IDeltaUpdatable
{
    bool AllowedToDelete(Aisling aisling, Post post);
    bool AllowedToHighlight(Aisling aisling);
    bool AllowedToPost(Aisling aisling);
    bool AllowedToView(Aisling aisling);
    bool ShouldRejectPost(Aisling aisling, Post post, [MaybeNullWhen(false)] out string reason);
}