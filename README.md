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

📂Data  
 ┣📂[LootTables](#loottables-folder)  
 ┃ ┗📜rat__stick_apple.json  
 ┣📂[MapData](#mapdata-folder)  
 ┃ ┣📜lod500.map  
 ┃ ┗📜lod3006.map  
 ┣📂[MapInstances](#mapinstances-folder)  
 ┃ ┣📂mileth1  
 ┃ ┃ ┣📜instance.json  
 ┃ ┃ ┗📜spawns.json  
 ┃ ┗📂milethVillageWay1  
 ┃   ┣📜instance.json  
 ┃   ┗📜spawns.json  
 ┣📂Metafiles (TODO)  
 ┣📂Saved  
 ┃ ┗📂bonk  
 ┃   ┣📜aisling.json  
 ┃   ┣📜bank.json  
 ┃   ┣📜equipment.json  
 ┃   ┣📜inventory.json  
 ┃   ┣📜legend.json  
 ┃   ┣📜password.txt (hashed)  
 ┃   ┣📜skills.json  
 ┃   ┗📜spells.json  
 ┗📂[Templates](#templates-folder)  
   ┣📂[Items](#items-folder)  
   ┃ ┗📜stick.json  
   ┣📂[Maps](#maps-folder)  
   ┃ ┗📜500.json  
   ┣📂[Monsters](#monsters-folder)  
   ┃ ┗📜common_rat.json  
   ┣📂[Skills](#skills-folder)  
   ┃ ┗📜assail.json  
   ┗📂[Spells](#spells-folder)  
     ┗📜fire_breath.json

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

### Example file "rat1Sticks.json"

A loot table that gives a creature a 10% chance to drop a stick and a 30% chance to drop an apple

```json
{
  "key": "rat__stick_apple",
  "lootDrops": [
    {
      "itemTemplateKey": "stick",
      "dropChance": 10
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

## MapInstance Sub-Folder "Mileth"

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

### Warp Properties

| Name         |            Type/Values            | Description                                                                                                                                     |
|:-------------|:---------------------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------|
| Destination  | string<br/>"MapInstanceId:(X, Y)" | A string representation of a location<br/>The map instance id and coordinates the warp sends you to when stepped on<br/> Ex. "mileth1:(10, 10)" |
| Source       |        string<br/>"(X, Y)"        | A string representation of a point.<br/>The tile coordinates the warp is on<br/>Ex. "(50, 15)"                                                  |

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

This is the mileth village map with the added flags of falling snow and usage of the snow tileset  
This map will have 2 warps to mileth village way  
The map has a quest script on it

```json
{
  "templateKey": "500",
  "name": "Mileth",
  "instanceId": "mileth",
  "music": 1,
  "flags": "snow, snowtileset",
  "scriptKeys": [
    "SomeQuestScriptKey"
  ],
  "warps": [
    {
      "source": "(99, 30)",
      "destination": "milethVillageWay:(0, 15)"
    },
    {
      "source": "(99, 31)",
      "destination": "milethVillageWay:(0, 16)"
    }
  ]
}
```

### Example file "spawns.json"

This will spawn 25 rats that drop sticks/apples/20-30 gold/12 exp every 126-236 seconds up to a maximum of 50 rats  
The rats will spawn at the bottom quadrant of the map  
They will aggressively target anyone who comes within 6 spaces of them

```json
[
  {
    "monsterTemplateKey": "common_rat",
    "lootTableKey": "rat__stick_apple",
    "intervalSecs": 180,
    "intervalVariancePct": 30,
    "maxAmount": 50,
    "maxPerSpawn": 25,
    "aggroRange": 6,
    "minGoldDrop": 20,
    "maxGoldDrop": 30,
    "expReward": 12,
    "extraScriptKeys": [],
    "spawnArea": {
      "top": 75,
      "left": 75,
      "width": 25,
      "height": 25
    }
  }
]
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

A basic stick item that gives 1 str

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

### Example file "500.json"

```json
{
  "height": 100,
  "width": 100,
  "templateKey": "500",
  "warpPoints": [
    "(99, 30)",
    "(99, 31)"
  ],
  "scriptKeys": []
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
  "templateKey": "common_rat",
  "statSheet": {
    "ability": 0,
    "level": 5,
    "maximumHp": 100,
    "maximumMp": 100,
    "ac": 50,
    "str": 1,
    "int": 1,
    "wis": 1,
    "con": 2,
    "dex": 1,
    "magicResistance": 0
  },
  "type": "normal",
  "direction": "down",
  "name": "Common Rat",
  "sprite": 7,
  "wanderIntervalMs": 2000,
  "moveIntervalMs": 1500,
  "skillIntervalMs": 1500,
  "spellIntervalMs": 10000,
  "assailIntervalMs": 1500,
  "spellTemplateKeys": [],
  "skillTemplateKeys": ["assail"],
  "scriptKeys": ["commonMonster"]
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
  "templateKey": "fire_Breath",
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