using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public sealed class BulletinBoardScript : ConfigurableReactorTileScriptBase
{
    private readonly IStore<BulletinBoard> BoardStore;
    public string BoardKey { get; init; } = null!;

    /// <inheritdoc />
    public BulletinBoardScript(ReactorTile subject, IStore<BulletinBoard> boardStore)
        : base(subject) => BoardStore = boardStore;

    /// <inheritdoc />
    public override void OnClicked(Aisling source) => BoardStore.Load(BoardKey).Show(source);
}