using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Board;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.BoardScripts;
using Microsoft.Extensions.Logging;

namespace Chaos.Collections;

public class BulletinBoard : BoardBase, IScripted<IBulletinBoardScript>
{
    private readonly ILogger<BulletinBoard> Logger;
    /// <inheritdoc />
    public IBulletinBoardScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    /// <inheritdoc />
    public BulletinBoard(
        ushort boardId,
        string name,
        string key,
        IEnumerable<Post> posts,
        IEnumerable<string> moderators,
        ILogger<BulletinBoard> logger,
        IScriptProvider scriptProvider,
        IEnumerable<string> scriptKeys
    )
        : base(
            boardId,
            name,
            key,
            posts,
            moderators)
    {
        Logger = logger;
        ScriptKeys = scriptKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        Script = scriptProvider.CreateScript<IBulletinBoardScript, BulletinBoard>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override bool Delete(IWorldClient deletedBy, short postId) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Highlight(IWorldClient highlightedBy, short postId) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Post(
        IWorldClient addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false
    ) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public override void Show(IWorldClient client, short startPostId) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void ShowPost(IWorldClient client, short postId, BoardControls control) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void UnHighlight(IWorldClient unhighlightedBy, ref Post post) => throw new NotImplementedException();
}