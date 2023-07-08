# Bulletin Boards

Boards in Chaos are both templated and scripted objects. When you define a board, you are defining the read-only
configuration of that board across the game. BulletinBoards are created from that configuration, with it's posts being
separately serialized and deserialized elsewhere.

## BulletinBoard Templates

A bulletin board template is a configuration object that defines the read-only configuration of a bulletin board.

By default, bulletin board templates are stored in `Data\Configuration\Tempaltes\BulletinBoards`. Configuration of how
bulletin boards are loaded can be found in `appsettings.json` at `Options:BulletinBoardTemplateCacheOptions`.

BulletinBoard templates are initially serialized
into [BulletinBoardTemplateSchema](<xref:Chaos.Schemas.Templates.BulletinBoardTemplateSchema>) before being mapped to a
non-schema type.

### BulletinBoardTemplateSchema

| Type                                                                              | Name        | Description                                                                                                                                                                                   |
|-----------------------------------------------------------------------------------|-------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                                                            | TemplateKey | A unique id specific to this template. This must match the file name                                                                                                                          |
| string                                                                            | Name        | The name of the board, as displayed if shown in a list of boards                                                                                                                              |
| ICollection\<string\>                                                             | ScriptKeys  | A collection of names of scripts to attach to this object by default                                                                                                                          |
| IDictionary\<string, [DynamicVars](<xref:Chaos.Collections.Common.DynamicVars>)\> | ScriptVars  | A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |

### Example BulletinBoard Template Json

Here is an example of a public bulletin board that anyone can read or post to. Posts are not automatically removed from
this board, but the option exists.

[!code-json[Example BulletinBoard Template Json](../../Data/Configuration/Templates/BulletinBoards/public_test_board.json)]

## BulletinBoard

The BulletinBoard that is created from the configured template is serialized to and
from [BulletinBoardSchema](<xref:Chaos.Schemas.Boards.BulletinBoardSchema>). Configuration of how BulletinBoards are
saved and backed up can be found in `appsettings.json` at `Options:BulletinBoardStoreOptions`.

## MailBox

MailBoxes are handled slightly differently, and are serialized to and
from [MailBoxSchema](<xref:Chaos.Schemas.Boards.MailBoxSchema>). MailBoxes are not configurable or scripted, and are
created when an Aisling is created. Configuration of how MailBoxes are saved and backed up can be found
in `appsettings.json` at `Options:MailBoxStoreOptions`.

## Posts

Posts are serialized to and from [PostSchema](<xref:Chaos.Schemas.Boards.BulletinBoardSchema>)

## Board List

When logged onto a character, there's a button you can press to see a list of boards that you have access to. This list
of boards is configurable through the `DefaultAislingScript`.

## BulletinBoards in the world

If you want to display a board by clicking a tile on the map, you can use a reactor tile. Just place a reactor tile on
the point the tile is at, and add a script to that reactor tile to display the board when clicked.