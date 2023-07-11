# Bulletin Boards

Boards in Chaos are both templated and scripted objects. When you define a board, you are defining the read-only
configuration of that board across the game. BulletinBoards are created from that configuration, with it's posts being
separately serialized and deserialized elsewhere.

## BulletinBoard Templates

A bulletin board template is a configuration object that defines the read-only configuration of a bulletin board.

## How do I create them?

By default, bulletin board templates should be created in `Data\Configuration\Tempaltes\BulletinBoards`. Configuration
of how bulletin boards are loaded can be found in `appsettings.json` at `Options:BulletinBoardTemplateCacheOptions`.

BulletinBoard templates are initially serialized
into [BulletinBoardTemplateSchema](<xref:Chaos.Schemas.Templates.BulletinBoardTemplateSchema>) before being mapped to
a [BulletinBoardTemplate](<xref:Chaos.Models.Templates.BulletinBoardTemplate>). The schema object is mapped via
the [BoardMapperProfile](<xref:Chaos.Services.MapperProfiles.BoardMapperProfile>).

See [BulletinBoardTemplateSchema](<xref:Chaos.Schemas.Templates.BulletinBoardTemplateSchema>) for a list of all
configurable properties with descriptions.

## How do I use them?

BulletinBoards are loaded via the [BulletinBoardStore](<xref:Chaos.Services.Storage.BulletinBoardStore>),
which is an implementation of [IStore\<T\>](<xref:Chaos.Storage.Abstractions.IStore`1>).

> [!NOTE]
> Objects stored via `IStore<T>` are loaded on-demand, and never go out of scope

```cs
private readonly IStore<BulletinBoard> BulletinBoardStore;

public Foo(IStore<BulletinBoard> bulletinBoardStore) => BulletinBoardStore = bulletinBoardStore;

public void Bar()
{
    //this will fetch the bulletin board that has the key specified
    var bulletinBoard = BulletinBoardStore.Load("myBoardKey");
}
```

## MailBoxes

MailBoxes are handled slightly differently, and are serialized to and
from [MailBoxSchema](<xref:Chaos.Schemas.Boards.MailBoxSchema>). MailBoxes are not configurable or scripted, and are
created when an Aisling is created. Configuration of how MailBoxes are saved and backed up can be found
in `appsettings.json` at `Options:MailBoxStoreOptions`.

### How do I use them?

MailBoxes are made accessibly through the [MailStore](<xref:Chaos.Services.Storage.MailStore>), which is an
implementation of [IStore\<T\>](<xref:Chaos.Storage.Abstractions.IStore`1>).

> [!NOTE]
> Objects stored via `IStore<T>` are loaded on-demand, and never go out of scope

```cs
private readonly IStore<MailBox> MailStore;

public Foo(IStore<MailBox> mailStore) => MailStore = mailStore;

public void Bar()
{
    //this will fetch the mailbox for the specified aisling name
    var mailBox = MailStore.Load("sichi");
}
```

## Board List

When logged onto a character, there's a button you can press to see a list of boards that you have access to. This list
of boards is configurable through the `DefaultAislingScript`.

You can also display a list of boards via other scripts, such as a reactor tile, by calling

```cs
private readonly IStore<MailBox> MailStore;
private readonly IStore<BulletinBoard> BulletinBoardStore;

public Foo(IStore<MailBox> mailStore, IStore<BulletinBoard> bulletinBoardStore)
{
    MailStore = mailStore;
    BulletinBoardStore = bulletinBoardStore;
}

public void Bar(IWorldClient client)
{
    //fetch mailbox
    var mailBox = MailStore.Load("sichi");
    //fetch other board
    var bulletinBoard = BulletinBoardStore.Load("myBoardKey");
    
    //display both boards to the client
    client.SendBoardList(new[] { mailBox, bulletinBoard });
}
```

## BulletinBoards in the world

If you want to display a board by clicking a tile on the map, you can use a reactor tile. Just place a reactor tile on
the point the tile is at, and add a script to that reactor tile to display the board when clicked.

## Scripting

BulletinBoards are scripted
via [IBulletinBoardScript](<xref:Chaos.Scripting.BulletinBoardScripts.Abstractions.IBulletinBoardScript>).

- Inherit
  from [BulletinBoardScriptBase](<xref:Chaos.Scripting.BulletinBoardScripts.Abstractions.BulletinBoardScriptBase>) for a
  basic script that requires no external configuration
- Inherit
  from [ConfigurableBulletinBoardScriptBase](<xref:Chaos.Scripting.BulletinBoardScripts.Abstractions.ConfigurableBulletinBoardScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `BulletinBoardTemplate.ScriptKeys` property, and those scripts will
automatically be attached to the `BulletinBoard` when it is created.

If the script is configurable, you must also have an entry for that script in the `BulletinBoardTemplate.ScriptVars`
property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in bulletinBoard scripts

| Event Name         | Context                                                                                                                                                             |
|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AllowedToDelete    | Called when an aisling is trying to delete a post                                                                                                                   |
| AllowedToHighlight | Called when an aisling is trying to highlight a post. It's also used to determine if an aisling has the ability to see the highlight button in the UI               |
| AllowedToPost      | Called when an aisling is trying to post to the board                                                                                                               |
| AllowedToView      | Called when an aisling is trying to view the board, or a post on the board                                                                                          |
| ShouldRejectPost   | Called when an aisling is trying to post to the board. This event should be used to filter out posts for whatever reason you decide                                 |
| Update             | Called every time boards update, which is about once per minute. Boards update separately from all other objects. Only boards that have been loaded will be updated |

See [Scripting](Scripting.md) for more details on scripting in general

## Example

Here is an example of a public bulletin board that is configured to use the "publicBoard" script, which retains posts
indefinitely, and is moderated by a character named "fooBar". (more on scripts later)

[!code-json[](../../Data/Configuration/Templates/BulletinBoards/public_test_board.json)]