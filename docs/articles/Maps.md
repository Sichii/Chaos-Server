# Maps

Maps in Chaos are both templated and scripted objects. There are multiple pieces of information to define to create a
map, the map templates, and the map instances

Chaos uses a map instancing system. What map an object is on is determined by that map's MapInstanceId, instead of it's
numerical map file number. This allow multiple maps with the same MapId/Name to coexist at the same time. Chaos also has
an automatic map sharding system that will be explained later.

## Map Templates

A map template contains a bare minimum of information about a map. Contained within this project are a large amount of
auto-generated map templates that should match up to the maps in original Dark Ages.

## How do I create them?

There are 2 things you will need to create.

1. A map template, which will define details about the map data to use
2. A map instance, which will be an instance of a map using the data defined in the map template

### Creating a MapTemplate

By default, map templates should be created at `Data\Configuration\Templates\Maps`. Configuration of how map templates
are loaded can be found in `appsettings.json` at `Options:MapTemplateCacheOptions`.

Map templates are initially serialized into [MapTemplateSchema](<xref:Chaos.Schemas.Templates.MapTemplateSchema>) before
being mapped to a [MapTemplate](<xref:Chaos.Models.Templates.MapTemplate>). The schema object is mapped via the
the [MapInstanceMapperProfile](<xref:Chaos.Services.MapperProfiles.MapInstanceMapperProfile>).

See [MapTemplateSchema](<xref:Chaos.Schemas.Templates.MapTemplateSchema>) for a list of all configurable properties with
descriptions.

### Creating a MapInstance

By default, map instances should be created at `Data\Configuration\MapInstances`. Configuration of how map instances are
loaded can be found in `appsettings.json` at `Options:MapInstanceCacheOptions`.

Map instances are initially serialized into [MapInstanceSchema](<xref:Chaos.Schemas.Content.MapInstanceSchema>) before
being mapped to a [MapInstance](<xref:Chaos.Collections.MapInstance>). The schema object is mapped via the
the [MapInstanceMapperProfile](<xref:Chaos.Services.MapperProfiles.MapInstanceMapperProfile>).

Map instances are serialized from multiple files, so each map instance should be in it's own directory. Here is the
folder structure for a map instance:

<pre>
📂Configuration
 ┗📂MapInstances
   ┗📂mileth
     ┣📄instance.json
     ┣📄merchants.json
     ┣📄monsters.json
     ┗📄reactors.json
</pre>

For a list of all configurable properties with descriptions, see:

- `instance.json` will be serialized from [MapInstanceSchema](<xref:Chaos.Schemas.Content.MapInstanceSchema>)
- `merchants.json` will be a jArray of [MerchantSpawnSchema](<xref:Chaos.Schemas.Content.MerchantSpawnSchema>)
- `monsters.json` will be a jArray of [MonsterSpawnSchema](<xref:Chaos.Schemas.Content.MonsterSpawnSchema>)
- `reactors.json` will be a jArray of [ReactorTileSchema](<xref:Chaos.Schemas.Content.ReactorTileSchema>)

## How do I use them?

Map instances are loaded via the [ExpiringMapInstanceCache](<xref:Chaos.Services.Storage.ExpiringMapInstanceCache>),
which is an implementation of [ISimpleCache\<T\>](<xref:Chaos.Storage.Abstractions.ISimpleCache`1>).

> [!NOTE]
> Map instances are loaded on-demand, and can expire if not accessed for a period of time

You can fetch the map instance directly like this:

```cs
private readonly ISimpleCache SimpleCache;

public Foo(ISimpleCache simpleCache) => SimpleCache = simpleCache;

public void Bar()
{
    var mapInstance = SimpleCache.Get<MapInstance>("mileth");
}
```

You can also fetch the cache itself like this:

```cs
private readonly ISimpleCacheProvider SimpleCacheProvider;

public Foo(ISimpleCacheProvider simpleCacheProvider) => SimpleCacheProvider = simpleCacheProvider;

public void Bar()
{
    //fetch the ISimpleCache<T> implementation
    var mapInstanceCache = SimpleCacheProvider.GetCache<MapInstance>();
    
    //fetch map instance from that cache
    var mapInstance = mapInstanceCache.Get("mileth");
}
```

## Map Sharding

When creating your map instance, you might notice that there are sharding options. Under the conditions that you set in
those options, a new map instance will be spun up with a new map instance id. The map instance id will be the same as
the instance id of the map that it was sharded from, but with a guid attached. You can view all the shards of a map
instance from any of the shards, or the base map instance by accessing the `MapInstance.Shards` property.

It's important to note here that these shards are not the same type of "instances" that you might find in AAA MMO's.
These
instances are not owned or associated to any aisling, and there are no persistent zone timers or anything.

### AbsolutePlayerLimit

- Once the map instance has reached `ShardingOptions.Limit` number of players, any new players that get added will
  instead be added to any existing shards that arent yet full
- If no shards exist, or all shards are full, a new shard will be created
- If the limit is 1, no checks will be made on existing shards, and a new map instance will always be created (shards
  will not be reused)
- If a player logs out of one of these instances, if the instance still exists, they will be added back to it, otherwise
  they will be added moved to `ShardingOptions.ExitLocation`
- If a shard is over-filled due to a player logging into an already-full shard, that player will be moved to
  `ShardingOptions.ExitLocation` after a short delay

### PlayerLimit

The same as AbsolutePlayerLimit except for the following

- When joining a shard, an attempt will be made to place group members together, even if the shard is full
- Players will not be removed from a shard if it is over-filled

### AbsoluteGroupLimit

- Once the map instance has reached `ShardingOptions.Limit` number of groups, any aisling not in an existing group that
  get added will instead be added to any existing shards that arent yet full
- If no shards exist, or all shards are full, a new shard will be created
- If the limit is 1, if no group member is in an existing shard, a new shard will always be created. (shards will not be
  reused)
- If a player logs out of one of these instances, if the instance still exists, they will be added back to it, otherwise
  they will be added moved to `ShardingOptions.ExitLocation`
- If a shard is over-filled due to a player logging into an already-full shard, that player will be moved to
  `ShardingOptions.ExitLocation` after a short delay. If the player is grouped by someone, they will not be moved.

### AbsoluteGuildLimit

- Once the map instance has reached `ShardingOptions.Limit` number of guilds, any aisling not in an existing guild that
  get added will instead be added to any existing shards that arent yet full
- If no shards exist, or all shards are full, a new shard will be created
- If the limit is 1, if no group member is in an existing shard, a new shard will always be created. (shards will not be
  reused)
- If a player logs out of one of these instances, if the instance still exists, they will be added back to it, otherwise
  they will be added moved to `ShardingOptions.ExitLocation`
- If a shard is over-filled due to a player logging into an already-full shard, that player will be moved to
  `ShardingOptions.ExitLocation` after a short delay. If the player is in one of the guilds that are already in the
  shard, they will not be moved.

## Scripting

Maps are scripted via [IMapScript](<xref:Chaos.Scripting.MapScripts.Abstractions.IMapScript>).

- Inherit from [MapScriptBase](<xref:Chaos.Scripting.MapScripts.Abstractions.MapScriptBase>) for a basic script that
  requires no external configuration
- I could not think of a reason to have configurable map scripts, but you are free to create the base for yourself

Specify any number of script keys in the `MapInstance.ScriptKeys` property, and those scripts will automatically be
attached to the `MapInstance` when it is created.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in map scripts:

| Event Name | Description                                                                                                                                                                              |
|------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OnEntered  | Called after a creature has entered the map for any reason (including spawns). This includes monsters, merchants, and aislings<br/>This is called after OnSpawn, but before OnApproached |
| OnExited   | Called after a creature has exited the map for any reason (including death). This includes monsters, merchants, and aislings<br/>This is called after OnDeath, but before OnDeparture    |
| Update     | Called every time the map updates. Ever map has it's own update loop. Time between updates is configurable via [WorldOptions](WorldOptions.md#updatespersecond)                          |

## Example

Here is an example of a map template json for the map used for TestTown. This map uses `lod3043.map`, has a number of
test merchants on it, no monsters, 2 boards, a world map, and a warp reactor.

### MapTemplate `3043.json`

[!code-json[](../../Data/Configuration/Templates/Maps/3043.json)]

### MapInstance `instance.json`

[!code-json[](../../Data/Configuration/MapInstances/testTown/instance.json)]

### MapInstance `merchants.json`

[!code-json[](../../Data/Configuration/MapInstances/testTown/merchants.json)]

### MapInstance `monsters.json`

[!code-json[](../../Data/Configuration/MapInstances/testTown/monsters.json)]

### MapInstance `reactors.json`

[!code-json[](../../Data/Configuration/MapInstances/testTown/reactors.json)]