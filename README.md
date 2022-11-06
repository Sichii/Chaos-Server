# Chaos

A configurable Dark Ages server emulator

# Configuration (appsettings.json)

You can configure the Login and Lobby servers via appsettings.json file  
Here are a few quick tips, but there are more options available than are listed

- It's recommended to keep the [staging data](#folder-structure) out of the repo, this base staging directory can be changed at Options:
  ChaosOptions:StagingDirectory
- Username/Password rules can be changed via Options:ActiveDirectoryCreentialManagerOptions
- If you want to spin up multiple worlds, or offer redirects to other people's worlds, you can add additional servers via Options:
  LobbyOptions:Servers
- If you want to accept redirects from other people, you need to communicate a reserved redirect id, and configure it via Options:
  LoginOptions:ReservedRedirects
- Edit your login notice message via Options:LoginOptions:NoticeMessage
- Edit your new character initial spawn point via Options:LoginOptions:StartingMapInstanceId and StartingPointStr

General world options are also changed via Options:WorldOptions

### World Options

| Name                    |     Type/Values     | Description                                                                                                                   |
|:------------------------|:-------------------:|:------------------------------------------------------------------------------------------------------------------------------|
| AislingAssailIntervalMs |       number        | The base assail interval for aislings<br/>This value is modified by the AtkSpeedPct attribute                                 |
| DropRange               |  number<br/>0-255   | The tile range around an aisling that they can drop items                                                                     |
| MaxActionsPerSecond     |       number        | The maximum number of assails/skills/spells combined that an aisling can use per second                                       |
| MaxGoldHeld             |       number        | The maximum amount of gold an aisling can hold                                                                                |
| MaximumAislingAc        | number<br/>-255-255 | The highest AC an aisling can have<br/>Higher AC = Takes more damage                                                          |
| MaximumMonsterAc        | number<br/>-255-255 | The highest AC a monster can have<br/>Higher AC = Takes more damage                                                           |
| MaxLevel                |  number<br/>0-255   | The aisling level cap                                                                                                         |
| MinimumAislingAc        | number<br/>-255-255 | The lowest AC an aisling can have<br/>Lower AC = Takes less damage                                                            |
| MinimumMonsterAc        | number<br/>-255-255 | The lowest AC a monster can have<br/>Lower AC = Takes less damage                                                             |
| PickupRange             |  number<br/>0-255   | The tile range around an aisling that they can pick up items                                                                  |
| RefreshIntervalMs       |       number        | The minimum number of milliseconds allowed between each refresh request                                                       |
| SaveIntervalMins        |       number        | The number of minutes between aisling saves                                                                                   |
| TradeRange              |  number<br/>0-255   | The tiles range around an aisling that they can engage a trade with another aisling                                           |
| UpdatesPerSecond        |       number        | The number of server updates executed per second<br/>The server uses a time delta, so this number doesnt need to be very high |

# Folder Structure

📂Data ┣📂[LootTables](#loottables-folder)  ┃ ┗📜testAreaRats.json ┣📂[MapData](#mapdata-folder)  ┃ ┣📜lod3043.map ┃ ┣📜lod3044.map ┃
┗📜lod5219.map ┣📂[MapInstances](#mapinstances-folder)  ┃ ┣📂testTown ┃ ┃ ┣📜instance.json ┃ ┃ ┗📜spawns.json ┃ ┣📂testRoom ┃ ┃
┣📜instance.json ┃ ┃ ┗📜spawns.json ┃ ┗📂testArea ┃ ┣📜instance.json ┃ ┗📜spawns.json ┣📂[WorldMaps](#worldMaps-folder)  ┃
┣📂[Nodes](#nodes-folder)  ┃ ┃ ┣📜testTown.json ┃ ┃ ┗📜testArea.json ┃ ┗📜field001.json ┣📂Metafiles (TODO)  ┣📂Saved ┃ ┗📂bonk ┃
┣📜aisling.json ┃ ┣📜bank.json ┃ ┣📜equipment.json ┃ ┣📜inventory.json ┃ ┣📜legend.json ┃ ┣📜password.txt (hashed)  ┃ ┣📜skills.json ┃
┗📜spells.json ┗📂[Templates](#templates-folder)  ┣📂[Items](#items-folder)  ┃ ┗📜stick.json ┣📂[Maps](#maps-folder)  ┃ ┣📜3043.json ┃
┣📜3044.json ┃ ┗📜5219.json ┣📂[Monsters](#monsters-folder)  ┃ ┗📜common_rat.json ┣📂[Skills](#skills-folder)  ┃ ┗📜assail.json  
┗📂[Spells](#spells-folder)  ┗📜fire_breath.json

# LootTables Folder

Contains .json files such as "lootTableKey.json" that are used to determine loot drops for monsters

### LootTable Properties

| Name      |               Type/Values               | Description                                                                              |
|:----------|:---------------------------------------:|:-----------------------------------------------------------------------------------------|
| Key       |                 string                  | A unique id specific to this loot table. Best practice is to match the file name         |
| LootDrops | array{[lootDrop](#lootdrop-properties)} | A collection of lootDrops. Every item in the list is calculated, allowing multiple drops |

### LootDrop Properties

| Name            |     Type/Values     | Description                                                        |
|:----------------|:-------------------:|:-------------------------------------------------------------------|
| DropChance      | number<br/>(0-100)  | The chance of the item to drop                                     |
| ItemTemplateKey |       string        | A unique id specific to the template of the item that should drop  |

### Example file "testAreaRats.json"

A loot table that gives a creature a 25% chance to drop a stick

```json
{
  "Key": "testAreaRats",
  "LootDrops": [
    {
      "ItemTemplateKey": "stick",
      "DropChance": 25
    }
  ]
}


```

# MapData Folder

Contains .map files containing tile data for maps

### Example

lod0.map  
lod1.map  
lod2.map

# MapInstances Folder

Contains subfolders, one for each map instance  
Multiple map instances can have the same numeric map id, but must have unique instance ids  
Best practice is for the folder name to match the map instance id

## MapInstance Sub-Folder "TestTown"

Contains two .json files, "instance.json" and "spawns.json"  
instance.json contains basic information about the map instance  
spawns.json contains a collection of spawn objects

### MapInstance Properties

| Name          |                                 Type/Values                                 | Description                                                                                                                                                   |
|:--------------|:---------------------------------------------------------------------------:|:--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Flags         | string<br/>None<br/>Snow<br/>Rain<br/>Darkness<br/>NoTabMap<br/>SnowTileset | A flag, or combination of flags that should affect the map<br/>You can combine multiple flags by separating them with commas<br/>Ex. "Snow, NoTabMap"         |
| InstanceId    |                                   string                                    | A unique id specific to this map instance<br/>Best practice is to match the folder name                                                                       |
| MinimumLevel  |                              number(optional)                               | Default null<br/>If specified, sets the minimum level needed to enter this map via warp tile                                                                  |
| MaximumLevel  |                              number(optional)                               | Default null<br/>If specified, sets the maximum level allowed to enter this map via warp tile                                                                 |
| Music         |                             number<br/>(0-255)                              | The byte values of the music track to play when entering the map<br/>These values aren't explored yet, so you'll have to figure out what's available yourself |
| Name          |                                   string                                    | The name of the map that will display in-game                                                                                                                 |
| ScriptKeys    |                                array{string}                                | A collection of script keys to load for this map (TODO: scripts section)                                                                                      |
| TemplateKey   |                            string<br/>(0-32767)                             | A string representation of the map id. Ex. 500 for mileth                                                                                                     |
| Warps         |                       array{[warp](#warp-properties)}                       | A collection of warps                                                                                                                                         |
| WorldMapWarps |               array{[worldMapWarp](#warpMapWarp-properties)}                | A collection fo world map warps                                                                                                                               |

### Warp Properties

| Name         |            Type/Values            | Description                                                                                                                                     |
|:-------------|:---------------------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------|
| Destination  | string<br/>"MapInstanceId:(X, Y)" | A string representation of a location<br/>The map instance id and coordinates the warp sends you to when stepped on<br/> Ex. "mileth1:(10, 10)" |
| Source       |        string<br/>"(X, Y)"        | A string representation of a point<br/>The tile coordinates the warp is on<br/>Ex. "(50, 15)"                                                   |

### WorldMapWarp Properties

| Name        | Type/Values         | Description                                                                                        |
|-------------|---------------------|----------------------------------------------------------------------------------------------------|
| WorldMapKey | string              | The unique id of the world map this tile will show the player                                      |
| Source      | string<br/>"(X, Y)" | A string representation of a point<br/>The tile coordinates the world map is on<br/>Ex. "(50, 15)" |

### Spawn Properties

| Name                |                  Type/Values                  | Description                                                                                                                                                                               |
|:--------------------|:---------------------------------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AggroRange          |               number(optional)                | Defaults to 0<br/>If specified, monsters created by this spawn will be aggressive and attack enemies if they come within the specified distance                                           |
| ExpReward           |                    number                     | The amount of exp monsters created by this spawn will reward when killed                                                                                                                  |
| ExtraScriptKeys     |                 array{string}                 | A collection of extra monster script keys to add to the monsters created by this spawn                                                                                                    |
| IntervalSecs        |                    number                     | A number of seconds between each trigger of this spawn                                                                                                                                    |
| IntervalVariancePct |               number(optional)                | Defaults to 0<br/>If specified, will randomize the interval by the percentage specified<br/>Ex. With an interval of 60, and a Variance of 50, the spawn interval would var from 45-75secs |
| LootTableKey        |                string(options)                | Default is to not have a loot table. If specified, the unique id for the loot table used to determine monster drops from this spawn                                                       |
| MaxAmount           |                    number                     | The maximum number of monsters that can be on the map from this spawn                                                                                                                     |
| MaxGoldDrop         |                    number                     | Maximum amount of gold for monsters created by this spawn to drop                                                                                                                         |
| MaxPerSpawn         |                    number                     | The maximum number of monsters to create per interval of this spawn                                                                                                                       |
| MinGoldDrop         |                    number                     | Minimum amount of gold for monsters created by this spawn to drop                                                                                                                         |
| MonsterTemplateKey  |                    string                     | The unique id for the template of the monster to spawn                                                                                                                                    |
| SpawnArea           | [rectangle](#rectangle-properties)(optional)  | Defaults to spawn on entire map<br/>If specified, monsters will only spawn within the specified bounds                                                                                    |

### Rectangle Properties

| Name   |    Type/Values     | Description                              |
|:-------|:------------------:|:-----------------------------------------|
| Top    | number<br/>(0-255) | The lowest Y coordinate of the rectangle |
| Left   | number<br/>(0-255) | The lowest X coordinate of the rectangle |
| Width  | number<br/>(0-255) | The width of the rectangle               |
| Height | number<br/>(0-255) | The height of the rectangle              |

### Example file "instance.json"

This is a test town that has a warp to testRoom, and a world map tile on it

```json
{
  "flags": "None",
  "instanceId": "testTown",
  "music": 1,
  "name": "Test Town",
  "templateKey": "3043",
  "warps": [
    {
      "destination": "testRoom:(4, 12)",
      "source": "(18, 11)"
    }
  ],
  "worldMapWarps": [
    {
      "worldMapKey": "field001",
      "source": "(0, 13)"
    }
  ]
}
```

### Example file "spawns.json"

This will spawn 10 rats per 22.5 - 37.5 secs, upt to am aximum of 20  
Those rats will aggro within 4 spaces

```json
[
  {
    "LootTableKey": "testAreaRats",
    "IntervalSecs": 30,
    "IntervalVariancePct": 50,
    "MaxPerSpawn": 10,
    "MaxAmount": 20,
    "AggroRange": 4,
    "MinGoldDrop": 10,
    "MaxGoldDrop": 30,
    "ExpReward": 50,
    "MonsterTemplateKey": "Common Rat"
  }
]
```

# WorldMap Folder

Contains a "Nodes" subfolder that contains all possible world map nodes

## Nodes Folder

Contains all possible world map nodes

### WorldMapNode Properties

| Name           | Type/Values                       | Description                                                                                       |
|----------------|-----------------------------------|---------------------------------------------------------------------------------------------------|
| NodeKey        | string                            | A unique id specific to this world map node                                                       |
| Destination    | string<br/>"MapInstanceId:(X, Y)" | A string representation of the map id and coordinates this node will take the player when clicked |
| Text           | string                            | The text display on the world map for this node                                                   |
| ScreenPosition | string<br/>"(X, Y)"               | A string representation of the screen coordinates this node will show in the world map            |

### Example file "testTown.json"

```json
{
    "nodeKey": "testTown",
    "destination": "testTown:(1, 13)",
    "text": "Test Town",
    "screenPosition": "(300, 150)"
}

```

### WorldMap Properties

| Name        | Type/Values                                    | Description                                                                    |
|-------------|------------------------------------------------|--------------------------------------------------------------------------------|
| WorldMapKey | string                                         | A unique key specific to this world map                                        |
| FieldIndex  | number(1-3)                                    | The image index the world map uses<br/>Temuair = 1<br/>Medenia = 2<br/>??? = 3 |
| NodeKeys    | array{[worldMapNode](#worldMapNode-properties) | A collection of keys to world map nodes to display on this world map           |

### Example file "field001.json"

```json
{
    "worldMapKey": "field001",
    "fieldIndex": 1,
    "nodeKeys": [
        "testTown",
        "testArea"
    ]
}
```

# Metafiles Folder

Not implemented yet

# Templates Folder

Contains subfolders for each type of template

## Items Folder

Contains .json files to be used as blueprints for items

### ItemTemplate Properties

| Name          |                                   Type/Values                                    | Description                                                                                                                                                                                  |
|:--------------|:--------------------------------------------------------------------------------:|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AccountBound  |                               bool<br/>true/false                                | If the item is account bound, it cannot be traded or dropped                                                                                                                                 |
| Color         |                    string<br/>[Color Options](#color-options)                    | Defaults to None(lavender)<br/>If the item is dyeable, this is the dye color                                                                                                                 |
| DisplaySprite |                                 number(optional)                                 | Defaults to null<br/>If specified, this is the sprite used to display the item on character when it is equipped                                                                              |
| MaxDurability |                                 number(optional)                                 | Defaults to null<br/>If specified, the base max durability of the item                                                                                                                       |
| MaxStacks     |                                      number                                      | The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable                                                                                          |
| Modifiers     |                  [attributes](#attributes-properties)(optional)                  | Defaults to null<br/>If specified, these are the stats this item grants when equipped                                                                                                        |
| PantsColor    | string<br/>any color between Black and White<br/>[Color Options](#color-options) | Default null<br />If specified, this armor will have pants, and they will be this color                                                                                                      |
| Value         |                                      number                                      | Not fully implemented                                                                                                                                                                        |
| Weight        |                                number<br/>(0-255)                                | The weight of the item in the inventory, or equipped                                                                                                                                         |
| CooldownMs    |                                 number(optional)                                 | Defaults to null<br/>If specified, any on-use effect of this object will use this cooldown                                                                                                   |
| Name          |                                      string                                      | The base name of the item                                                                                                                                                                    |
| PanelSprite   |                                number<br/>(1-500)                                | The sprite id used to display the item in the inventory, minus the offset                                                                                                                    |
| ScriptKeys    |                                  array{string}                                   | A collection of names of item scripts to attach to this item by default                                                                                                                      |
| ScriptVars    |                   dictionary{string, dictionary{string, any}}                    | A collection of key-value pairs of key-value pairs<br/>Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |
| TemplateKey   |                                      string                                      | A unique id specific to this item template. Best practice is to match the file name                                                                                                          |

### Attributes Properties

| Name            |      Type/Values      | Description |
|:----------------|:---------------------:|:------------|
| Ac              | number<br/>(-127-127) ||
| AtkSpeedPct     | number<br/>(-200-200) ||
| Con             |  number<br/>(0-255)   ||
| Dam             |  number<br/>(0-255)   ||
| Dex             |  number<br/>(0-255)   ||
| Hit             |  number<br/>(0-255)   ||
| Int             |  number<br/>(0-255)   ||
| MagicResistance |        number         ||
| MaximumHp       |        number         ||
| MaximumMp       |        number         ||
| Str             |  number<br/>(0-255)   ||
| Wis             |  number<br/>(0-255)   ||

#### Color Options

|           |           |           |            |          |            |
|-----------|-----------|-----------|------------|----------|------------|
| None      | Black     | Red       | Orange     | Blonde   | Cyan       |
| Blue      | Mulberry  | Olive     | Green      | Fire     | Brown      |
| Grey      | Navy      | Tan       | White      | Pink     | Chartreuse |
| Golden    | Lemon     | Royal     | Platinum   | Lilac    | Fuchsia    |
| Magenta   | Peacock   | NeonPink  | Arctic     | Mauve    | NeonOrange |
| Sky       | NeonGreen | Pistachio | Corn       | Cerulean | Chocolate  |
| Ruby      | Hunter    | Crimson   | Ocean      | Ginger   | Mustard    |
| Apple     | Leaf      | Cobalt    | Strawberry | Unusual  | Sea        |
| Harlequin | Amethyst  | NeonRed   | NeonYellow | Rose     | Salmon     |
| Scarlet   | Honey     |||||

### Example file "stick.json"

A basic stick item that gives 1 str and 100% atk speed

```json
{
  "templateKey": "stick",
  "accountBound": false,
  "maxDurability": 1000,
  "maxStacks": 1,
  "value": 1000,
  "weight": 10,
  "name": "Stick",
  "panelSprite": 86,
  "displaySprite": 1,
  "modifiers": {
    "str": 1,
    "atkSpeedPct": 100
  },
  "pantsColor": "black",
  "scriptKeys": ["Equipment"],
  "scriptVars": {
    "equipment": {
      "equipmentType": "weapon"
    }
  }
}
```

## Maps Folder

Contains .json files to be used as blueprints for maps  
Each template should match up to the numeric id of a mapdata file

### MapTemplate Properties

| Name        |    Type/Values     | Description                                                                                                                                       |
|:------------|:------------------:|:--------------------------------------------------------------------------------------------------------------------------------------------------|
| Height      | number<br/>(1-255) | The height of the map                                                                                                                             |
| ScriptKeys  |   array{string}    | A collection of names of map scripts to attach to this map by default                                                                             |
| TemplateKey |       string       | A unique id specific to this map template<br/>Best practice is to match the name of the file, and use the numeric id the map this template is for |
| WarpPoints  |   array{string}    | The coordinates of each warp tile on the map                                                                                                      |
| Width       | number<br/>(1-255) | The width of the map                                                                                                                              |

### Example file "3044.json"

```json
{
  "height": 14,
  "templateKey": "3044",
  "warpPoints": [],
  "width": 14
}
```

## Monsters Folder

Contains .json files to be used as blueprints for monsters

### MonsterTemplate Properties

| Name              |                    Type/Values                    | Description                                                                                                          |
|:------------------|:-------------------------------------------------:|:---------------------------------------------------------------------------------------------------------------------|
| AttackIntervalMs  |                      number                       | The number of milliseconds between usages of assails                                                                 |
| Direction         |     string<br/>Up<br/>Down<br/>Left<br/>Right     | The initial direction of the monster when spawned                                                                    |
| MoveIntervalMs    |                      number                       | The number of milliseconds between movements while this monster is targeting an enemy                                |
| Name              |                      string                       | The name of the monster when double clicked                                                                          |
| ScriptKeys        |                   array{string}                   | A collection of names of monsters scripts to attach to this monster<br/>TODO: scripts section                        |
| SkillIntervalMs   |                      number                       | The number of milliseconds between usages of non-assail skills                                                       |
| SkillTemplateKeys |                   array{string}                   | A collection of template keys of skills this monster will use                                                        |
| SpellIntervalMs   |                      number                       | The number of milliseconds between usages of spells                                                                  |
| SpellTemplateKeys |                   array{string}                   | A collection of template keys of spells this monster will cast                                                       |
| Sprite            |                number<br/>(1-1500)                | The sprite id of the monster minus the offset                                                                        |
| StatSheet         |        [statsheet](#statsheet-properties)         | The base stats of this monster                                                                                       |
| TemplateKey       |                      string                       | A unique id specific to this monster template<br/>Best practice is to match the name of the file                     |
| Type              | string<br/>Normal<br/>WalkThrough<br/>WhiteSquare | The monster's type<br/>WhiteSquare has no additional functionality, it just appears as a white square on the tab map |
| WanderIntervalMs  |                      number                       | The number of milliseconds between movements while this monster is wandering when it has no target                   |

### StatSheet Properties

| Name            |      Type/Values      | Description                       |
|:----------------|:---------------------:|:----------------------------------|
| Ability         |        number         | The ability level of this monster |
| Level           |        number         | The level of this monster         |
| Ac              | number<br/>(-127-127) ||
| Str             |  number<br/>(0-255)   ||
| Int             |  number<br/>(0-255)   ||
| Wis             |  number<br/>(0-255)   ||
| Con             |  number<br/>(0-255)   ||
| Dex             |  number<br/>(0-255)   ||
| Hit             |  number<br/>(0-255)   ||
| Dam             |  number<br/>(0-255)   ||
| MagicResistance |        number         ||
| MaximumHp       |        number         ||
| MaximumMp       |        number         ||

### Example file "common_rat.json"

```json
{
  "TemplateKey": "Common Rat",
  "StatSheet": {
    "Ability": 0,
    "Level": 20,
    "MaximumHp": 100,
    "MaximumMp": 100,
    "Ac": 50,
    "Str": 1,
    "Int": 1,
    "Wis": 1,
    "Con": 2,
    "Dex": 1,
    "MagicResistance": 0
  },
  "Type": "Normal",
  "Direction": "Down",
  "Name": "Common Rat",
  "Sprite": 7,
  "WanderIntervalMs": 2000,
  "MoveIntervalMs": 1500,
  "SkillIntervalMs": 1500,
  "SpellIntervalMs": 10000,
  "AssailIntervalMs": 1500,
  "SpellTemplateKeys": [],
  "SkillTemplateKeys": ["Assail"],
  "ScriptKeys": ["CommonMonster"]
}
```

## Skills Folder

Contains .json files to be used as blueprints for skills

### SkillTemplate Properties

| Name                  |                           Type/Values                           | Description                                                                                                                                                                                  |
|:----------------------|:---------------------------------------------------------------:|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| IsAssail              |                       bool<br/>true/false                       | Whether or not the skill is an assail and should be used when spacebar is pressed<br/>Assail cooldowns are handled by AssailIntervalMs and AtkSpeedPct                                       |
| CooldownMs            |                        number(optional)                         | Defaults to null. If specified, any on-use effect of this skill will use this cooldown                                                                                                       |
| Name                  |                             string                              | The base name of the skill                                                                                                                                                                   |
| PanelSprite           |                       number<br/>(1-500)                        | The sprite id used to display the skill in the skill pane                                                                                                                                    |
| ScriptKeys            |                          array{string}                          | A collection of names of skill scripts to attach to this skill by default                                                                                                                    |
| ScriptVars            |           dictionary{string, dictionary{string, any}}           | A collection of key-value pairs of key-value pairs<br/>Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |
| TemplateKey           |                             string                              | A unique id specific to this skill template. Best practice is to match the file name                                                                                                         |

### Example file "assail.json"

```json
{
  "templateKey": "assail",
  "name": "Assail",
  "panelSprite": 1,
  "isAssail": true,
  "scriptKeys": ["statBasedDamage"],
  "scriptVars": {
    "statBasedDamage": {
      "damage": 6,
      "bodyAnimation": "assail",
      "stat": "str",
      "statCoefficient": 0.333
    }
  }
}
```

## Spells Folder

Contains .json files to be used as blueprints for spells

### SpellTemplate Properties

| Name                  |                                                     Type/Values                                                      | Description                                                                                                                                                                                  |
|:----------------------|:--------------------------------------------------------------------------------------------------------------------:|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| CastLines             |                                                   number<br/>(0-9)                                                   | The number of chant lines this spell requires by default                                                                                                                                     |
| Prompt                |                                                   string(optional)                                                   | Defaults to null<br/>Should be specified with a spell type of "Prompt", this is the prompt the spell will offer when cast                                                                    |
| SpellType             | string<br/>None<br/>Prompt<br/>Targeted<br/>Prompt4Nums<br/>Prompt3Nums<br/>NoTarget<br/>Prompt2Nums<br/>Prompt1Num  | The way the spell is cast by the player                                                                                                                                                      |
| CooldownMs            |                                                   number(optional)                                                   | Defaults to null. If specified, any on-use effect of this spell will use this cooldown                                                                                                       |
| Name                  |                                                        string                                                        | The base name of the spell                                                                                                                                                                   |
| PanelSprite           |                                                  number<br/>(1-500)                                                  | The sprite id used to display the spell in the skill pane                                                                                                                                    |
| ScriptKeys            |                                                    array{string}                                                     | A collection of names of spell scripts to attach to this spell by default                                                                                                                    |
| ScriptVars            |                                     dictionary{string, dictionary{string, any}}                                      | A collection of key-value pairs of key-value pairs<br/>Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |
| TemplateKey           |                                                        string                                                        | A unique id specific to this spell template. Best practice is to match the file name                                                                                                         |

### Example file "fireBreath.json"

```json
{
  "templateKey": "fireBreath",
  "name": "Fire Breath",
  "panelSprite": 39,
  "scriptKeys": ["cascade"],
  "spellType": "notarget",
  "castLines": 0,
  "scriptVars": {
    "cascade": {
      "damage": 100,
      "sound": 140,
      "bodyAnimation": "wizardCast",
      "minSoundDelayMs": 2000,
      "propagationDelayMs": 300,
      "range": 15,
      "shape": "cone",
      "stopAtWalls": true,
      "targetAnimation": 138
    }
  }
}
```