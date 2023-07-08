using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public sealed class BulletinBoardListScript : ConfigurableReactorTileScriptBase
{
    private readonly IStore<BulletinBoard> BoardStore;
    public ICollection<string> BoardKeys { get; init; } = null!;

    /// <inheritdoc />
    public BulletinBoardListScript(ReactorTile subject, IStore<BulletinBoard> boardStore)
        : base(subject) => BoardStore = boardStore;

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        var boards = BoardKeys.Select(BoardStore.Load);

        source.Client.SendBoardList(boards);
    }
}