# Reactor Tiles

ReactorTiles in Chaos are both templated and scripted objects, however they do have 2 different use patterns. When you
define a reactor tile in a map, you are directly defining a reactor tile, but you can also define reactor tile templates
to be used by abilities. More details about these use cases below.

## ReactorTile Templates

A reactor tile template is used to create new instances of reactor tiles as required. It contains all the information
required to create an instance of a new reactor tile. Reactor tile templates are not used directly in maps, but are
instead used by abilities or other scripts that create reactor tiles on the fly, such as traps. The trap would be
defined in a template, and a fresh reactor would be created from that template every time the ability is used.

## How do I create them?

By default, reactor tile templates should be created in `Data\Configuration\Templates\ReactorTiles`. Configuration of
how reactor tiles are loaded can be found in `appsettings.json` at `Options:ReactorTileTemplateCacheOptions`.

Reactor tile templates are initially serialized
into [ReactorTileTemplateSchema](<xref:Chaos.Schemas.Templates.ReactorTileTemplateSchema>) before being mapped to
a [ReactorTileTemplate](<xref:Chaos.Models.Templates.ReactorTileTemplate>). The schema object is mapped via
the [ReactorTileMapperProfile](<xref:Chaos.Services.MapperProfiles.ReactorTileMapperProfile>).

See [ReactorTileTemplateSchema](<xref:Chaos.Schemas.Templates.ReactorTileTemplateSchema>) for a list of all configurable
properties with descriptions.

## How do I use them?

### In other scripts

Reactor tiles can be created by using the [ReactorTileFactory](<xref:Chaos.Services.Factories.ReactorTileFactory>),
which is an implementation of [IReactorTileFactory](<xref:Chaos.Services.Factories.Abstractions.IReactorTileFactory>).

```cs
private readonly IReactorTileFactory ReactorTileFactory;

public Foo(IReactorTileFactory reactorTileFactory) => ReactorTileFactory = reactorTileFactory;

public void Bar()
{
    //create a new instance of the reactor tile "myReactorTileTemplateKey"
    //mapInstance is the map instance that the reactor tile will be added to
    //point is the point on the map that the reactor tile will be added to
    //extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated reactor tile
    //owner is optional, but is required if the reactor tile deals damage
    var reactorTile = ReactorTileFactory.Create("myReactorTileTemplateKey", mapInstance, point, extraScriptKeys, creatureOwner);
    
    //the reactor tile is not automatically added to the map when created, so you must do so yourself
    mapInstance.AddObject(reactorTile, point);
}
```

### In map instances

Reactor tiles in map instances are defines in the `reactors.json` file. These reactors are not templates, and are not
references to templates. All details about each reactor defined this way is required. This file is where you will define
things like warp tiles, world map tiles, traps in a trap room, etc.

See the article on [MapInstances](Maps.md) for more details.

## Scripting

Reactor tiles are scripted
via [IReactorTileScript](<xref:Chaos.Scripting.ReactorTileScripts.Abstractions.IReactorTileScript>).

- Inherit from [ReactorTileScriptBase](<xref:Chaos.Scripting.ReactorTileScripts.Abstractions.ReactorTileScriptBase>) for
  a
  basic script that requires no external configuration
- Inherit
  from [ConfigurableReactorTileScriptBase](<xref:Chaos.Scripting.ReactorTileScripts.Abstractions.ConfigurableReactorTileScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `ReactorTileTemplate.ScriptKeys` property, and those scripts will automatically
be
attached to the `ReactorTile` when it is created.

If the script is configurable, you must also have an entry for that script in the `ReactorTileTemplate.ScriptVars`
property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in reactor tile scripts:

| Event Name         | Description                                             |
|--------------------|---------------------------------------------------------|
| OnClicked          | Called when the reactor tile is clicked                 |
| OnGoldDroppedOn    | Called after gold is dropped on the reactor tile        |
| OnGoldPickedUpFrom | Called after gold is picked up from the reactor tile    |
| OnItemDroppedOn    | Called after an item is dropped on the reactor tile     |
| OnItemPickedUpFrom | Called after an item is picked up from the reactor tile |
| OnWalkedOn         | Called when a creature walks on the reactor tile        |
| Update             | Called every time the map updates                       |

## Example

Here is an example of a needle trap reactor tile template. Most of the configuration is done through the "trap" script.
The trap will last for 300 seconds, and deal 500 damage to any hostile creature that steps on it. It will trigger up to
1 time before removing itself. The trap will also not block pathfinding, which is a property not shown here because
false is the default value of a boolean.

[!code-json[](../../Data/Configuration/Templates/ReactorTiles/needle_trap.json)]