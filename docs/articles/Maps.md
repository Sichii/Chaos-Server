# Maps

Maps in Chaos are both templated and scripted objects. There are multiple pieces of information to define to create a
map, the map templates, and the map instances

Chaos uses a map instancing system. What map an object is on is determined by that map's MapInstanceId, instead of it's
numerical map file number. This allow multiple maps with the same MapId/Name to coexist at the same time. Chaos also has
an automatic map sharding system that will be explained later.

## Map Templates

A map template contains a bare minimum of information about a map. Contained within this project are a large amount of
auto-generated map templates that should match up to the maps in original Dark Ages.

## Object Mapping and TypeMapper Profiles

Chaos uses a two-tier object system for data management:

1. **Schema Objects** - Simple data transfer objects (DTOs) used for JSON serialization/deserialization
2. **Domain Objects** - Rich objects with behavior and business logic used at runtime

**TypeMapper profiles** are responsible for converting between these object types. They implement the
`IMapperProfile<T1, T2>` interface and handle the conversion logic between schema and domain objects.

### MapInstanceMapperProfile Responsibilities

The [MapInstanceMapperProfile](<xref:Chaos.Services.MapperProfiles.MapInstanceMapperProfile>) handles three key
conversions:

- **MapTemplateSchema ↔ MapTemplate** - Converts map template data from JSON to domain objects
- **MapInstanceSchema ↔ MapInstance** - Converts map instance data from JSON to domain objects
- **MapInstance → MapInfoArgs** - Converts domain objects to network protocol arguments

This mapping process allows the system to:

- Keep JSON schemas simple and focused on data structure
- Build rich domain objects with complex behavior and validation
- Maintain separation between data storage and business logic

### Mapping Lifecycle

The object mapping lifecycle follows this pattern:

1. **File Loading** - JSON configuration files are read from disk
2. **Schema Deserialization** - JSON is deserialized into schema objects (e.g., `MapTemplateSchema`)
3. **Domain Conversion** - TypeMapper profiles convert schemas to domain objects (e.g., `MapTemplate`)
4. **Cache Storage** - Domain objects are stored in memory caches for performance
5. **Runtime Usage** - Domain objects are used throughout the application with full behavior
6. **Network Serialization** - When needed, domain objects are converted to network protocol objects for client
   communication

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

Map instances are loaded from multiple files, so each map instance should be in its own directory. Here is the
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

- `instance.json` will be deserialized as [MapInstanceSchema](<xref:Chaos.Schemas.Content.MapInstanceSchema>)
- `merchants.json` will be a JSON array of [MerchantSpawnSchema](<xref:Chaos.Schemas.Content.MerchantSpawnSchema>)
- `monsters.json` will be a JSON array of [MonsterSpawnSchema](<xref:Chaos.Schemas.Content.MonsterSpawnSchema>)
- `reactors.json` will be a JSON array of [ReactorTileSchema](<xref:Chaos.Schemas.Content.ReactorTileSchema>)

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

## Morphing

MapInstances can be morphed via [MapInstance.Morph](<xref:Chaos.Collections.MapInstance.Morph>). This will change the
underlying map template to the one specified. All aisling on the map will be automatically refreshed.

[OnMorphing](#scripting) should be used to handle any special cases that need to be addressed when a map is morphed. For
example, if you morph into a smaller dimension map, aislings will need to be moved so that they are not outside of map
bounds.

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
| OnMorphing | Called before a map is morphed                                                                                                                                                           |
| OnMorphed  | Called after a map has been morphed                                                                                                                                                      |
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