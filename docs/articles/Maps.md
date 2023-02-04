# Maps

One of the first things you're going to want to do is add maps to your server. There are a few things you should know about maps first
though. Chaos implement a map instance system. What map an object is on is determined by that maps MapInstanceId. This allow multiple maps
with the same exact map id / tile data, or even the same names to coexist at the same time. Chaos also has an automatic map sharding system
that will be explained later.

## Map Templates

A map template contains a bare minimum of information about a map. Contained within this project are a large amount of auto-generated map
templates that should match up to the maps in original Dark Ages.

By default, Map Templates are stored at `Data\Templates\Maps`. Configuration of how map templates are loaded can be found
in `appsettings.json` at `Options:MapTemplateCacheOptions`.

Map templates are initially serialized into [MapTemplateSchema](<xref:Chaos.Schemas.Templates.MapTemplateSchema>) before being mapped to a
non-schema type.

### MapTemplateSchema

| Type                  | Name        | Description                                                                                                |
|-----------------------|-------------|------------------------------------------------------------------------------------------------------------|
| string                | TemplateKey | A unique id specific to this map instance<br />This must match the name of the folder containing this file |
| ICollection\<string\> | ScriptKeys  | A collection of names of map scripts to attach to this map by default                                      |
| byte                  | Height      | The height of the map                                                                                      |
| byte                  | Width       | The width of the map                                                                                       |
| Point[]               | WarpPoints  | Nothing atm,                                                                                               |

### Example Map Template Json

Here is an example of a map template json for the map used for Mileth (MapId: 500). As with all template objects, the file name should match
the template key. So in this case, the file name is `500.json`, and it is stored in the `Data\Templates\Maps` directory.

[!code-json[](../../Data/Templates/Maps/500.json)]

## Map Instances

Map instances consist of a few different files that contain different information about a map.

- `instance.json` contains additional details about the map instance itself
- `merchants.json` contains all of the merchant spawners that are on the map
- `monsters.json` contains all of the monsters spawners that are on the map
- `reactors.json` contains all of the reactor tiles that are on the map

By default, Map Instances are stored at `Data\MapInstances`. Configuration of how map instances are loaded can be found
in `appsettings.json` at `Options:MapInstanceCacheOptions`.

### Example map instance directory

<pre>
📂Data
 ┗📂MapInstances
   ┗📂mileth
     ┣📄instance.json
     ┣📄merchants.json
     ┣📄monsters.json
     ┗📄reactors.json
</pre>

## instance.json

`instance.json` is serialized into [MapInstanceSchema](<xref:Chaos.Schemas.Content.MapInstanceSchema>) before being mapped to a
non-schema type.

### MapInstanceSchema

| Type                                                                      | Name            | Description                                                                                                                                                    |
|---------------------------------------------------------------------------|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                                                    | TemplateKey     | A string representation of the map id. Ex. 500 for mileth                                                                                                      |
| string                                                                    | Name            | The name of the map that will display in-game                                                                                                                  |
| string                                                                    | InstanceId      | A unique id specific to this map instance<br/>This must match the name of the folder containing this file                                                      |
| [MapFlags](<xref:Chaos.Common.Definitions.MapFlags>)                      | Flags           | A flag, or combination of flags that should affect the map<br />You can combine multiple flags by separating them with commas<br />Ex. "Snow, NoTabMap"        |
| int?                                                                      | MinimumLevel    | Default null<br />If specified, sets the maximum level allowed to enter this map via warp tile                                                                 |
| int?                                                                      | MaximumLevel    | Default null<br />If specified, sets the minimum level needed to enter this map via warp tile                                                                  |
| byte                                                                      | Music           | The byte values of the music track to play when entering the map<br />These values aren't explored yet, so you'll have to figure out what's available yourself |
| ICollection\<string\>                                                     | ScriptKeys      | A collection of script keys for script to load for this map                                                                                                    |
| [ShardingOptionsSchema?](<xref:Chaos.Schemas.Data.ShardingOptionsSchema>) | ShardingOptions | Default null<br/>If specified, these options will be used to determine how this instance will shard itself                                                     |

### Example instance.json

Here is an example of an `instance.json` for Mileth. Note that this map instance references a `templateKey` of `500`. This is the key of
the Map Template that this map instance is based on.

[!code-json[](../../Data/MapInstances/mileth/instance.json)]

### Instance Sharding

When sharding options are specified in the `instance.json` file, automatic sharding will be enabled for that map instance.  
See [ShardingOptionsSchema](<xref:Chaos.Schemas.Data.ShardingOptionsSchema>) for more details

### ShardingOptionsSchema

| Type                                                         | Name         | Description                                                                            |
|--------------------------------------------------------------|--------------|----------------------------------------------------------------------------------------|
| [Location](<xref:Chaos.Geometry.Location>)                   | ExitLocation | The instanceId to teleport players to if they log on and the instance no longer exists |
| int                                                          | Limit        | The number of players or groups allowed per instance (based on Shardingtype)           |
| [ShardingType](<xref:Chaos.Common.Definitions.ShardingType>) | ShardingType | The conditions that lead to new shards of this instance being created                  |

### Example instance.json with sharding

In this example, this map will shard every time a unique player tries to enter the map. If for any reason the server does not know where to
put a player that is trying to enter a shard of this map, say for example they log off for an extended period of time and the shard shuts
down, they will be placed in the map instance `testTown` at coordinates `1, 13`

[!code-json[](../../Data/MapInstances/testArea/instance.json)]

## merchants.json

`merchants.json` is serialized into [MerchantSpawnSchema](<xref:Chaos.Schemas.Content.MerchantSpawnSchema>) before being mapped to
a non-schema type.

Most importantly, there is the ability to add extra script keys to the merchant that are not normally part of it. This allows you to have
multiple instances of the same merchant that each do slightly different things.

See [the article on merchants](<Merchants.md>) for more information on how to create merchants.

### MerchantSpawnSchema

| Type                                                                  | Name                | Description                                                                             |
|-----------------------------------------------------------------------|---------------------|-----------------------------------------------------------------------------------------|
| string                                                                | MerchantTemplateKey | The unique id for the template of the merchant to spawn                                 |
| [Point](<xref:Chaos.Geometry.Point>)                                  | SpawnPoint          | The point on ths map where the merchant will spawn                                      |
| [Direction](<xref:Chaos.Geometry.Abstractions.Definitions.Direction>) | Direction           | The direction the merchant will be facing when spawned                                  |
| ICollection\<string\>                                                 | ExtraScriptKeys     | A collection of extra merchant script keys to add to the monsters created by this spawn |

### Example merchants.json

[!code-json[](../../Data/MapInstances/testTown/merchants.json)]

## monsters.json

`monsters.json` is serialized into [MonsterSpawnSchema](<xref:Chaos.Schemas.Content.MonsterSpawnSchema>) before being mapped to a non-schema
type.

See [the article on monsters](<Monsters.md>) for more information on how to create monsters.  
See [the article on loot tables](<LootTables.md>) for more information on how to create loot tables.

Similarly to `merchants.json` you can add extra script keys, and various other information to a specific spawn of a monster.

### MonsterSpawnSchema

| Type                                          | Name                | Description                                                                                                                                                                                  |
|-----------------------------------------------|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                        | MonsterTemplateKey  | The unique id for the template of the monster to spawn                                                                                                                                       |
| string?                                       | LootTableKey        | Default is to not have a loot table.<br />If specified, the unique id for the loot table used to determine monster drops from this spawn                                                     |
| int                                           | MaxAmount           | The maximum number of monsters that can be on the map from this spawn                                                                                                                        |
| int                                           | MaxPerSpawn         | The maximum number of monsters to create per interval of this spawn                                                                                                                          |
| int                                           | IntervalSecs        | The number of seconds between each trigger of this spawn                                                                                                                                     |
| int?                                          | IntervalVariancePct | Defaults to 0<br />If specified, will randomize the interval by the percentage specified<br />Ex. With an interval of 60, and a Variance of 50, the spawn interval would vary from 45-75secs |
| [Rectangle?](<xref:Chaos.Geometry.Rectangle>) | SpawnArea           | Defaults to spawn on entire map<br />If specified, monsters will only spawn within the specified bounds                                                                                      |
| int                                           | MinGoldDrop         | Minimum amount of gold for monsters created by this spawn to drop                                                                                                                            |
| int                                           | MaxGoldDrop         | Maximum amount of gold for monsters created by this spawn to drop                                                                                                                            |
| int                                           | ExpReward           | The amount of exp monsters created by this spawn will reward when killed                                                                                                                     |
| int                                           | AggroRange          | Defaults to 0<br />If specified, monsters created by this spawn will be aggressive and attack enemies if they come within the specified distance                                             |
| ICollection\<string\>                         | ExtraScriptKeys     | A collection of extra monster script keys to add to the monsters created by this spawn                                                                                                       |

### Example monsters.json

[!code-json[](../../Data/MapInstances/test1/monsters.json)]

## reactors.json

`reactors.json` is serialized into [ReactorTileSchema](<xref:Chaos.Schemas.Content.ReactorTileSchema>) before being mapped to a non-schema
type.

> [!CAUTION]
> Reactors added directly to a map are not the same as the reactors that are created on the fly by scripts.

### ReactorTileSchema

| Type                                                                              | Name                    | Description                                                                                                                                                                                   |
|-----------------------------------------------------------------------------------|-------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Point](<xref:Chaos.Geometry.Point>)                                              | Source                  | The coordinates this reactor is located at                                                                                                                                                    |
| bool                                                                              | ShouldBlockPathfinding  | Whether or not this reactor should block monster pathfinding. If this is set to false, monsters and merchants will be able to step on this reactor                                            |
| string?                                                                           | OwnerMonsterTemplateKey | If this reactor does damage, it is required to have an owner, otherwise this property can be ignored. The owning monster can be a basic monster with no stats or scripts                      |
| ICollection\<string\>                                                             | ScriptKeys              | A collection of names of scripts to attach to this object by default                                                                                                                          |
| IDictionary\<string, [DynamicVars](<xref:Chaos.Common.Collections.DynamicVars>)\> | ScriptVars              | A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |

### Example reactors.json

[!code-json[](../../Data/MapInstances/testArea/reactors.json)]