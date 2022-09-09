# Chaos
A configurable Dark Ages server emulator 

## Folder Structure
- Data
  - LootTables
  - MapData
  - MapInstances
  - Metafiles
  - Saved
  - Templates
    - Items
    - Maps
    - Monsters
    - Skills
    - Spells
    
    
## LootTables Folder
Contains .json files such as "lootTableKey.json"  

### LootTable Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Key|string|A unique id specific to this loot table. Best practice is to match the file name|
|LootDrops|array<lootDrop>|A collection of lootDrops|

### LootDrop Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|ItemTemplateKey|string|A unique id specific to the template of the item that should drop|
|DropChance|int<br>(0-100)|The chance of the item to drop. Every item in the list is calculated, allowing multiple drops|

### Example file
A loot table that gives a creature a 10% chance to drop a stick and a 30% chance to drop an apple  
```json
{
  "key": "rat1Sticks",
  "lootDrops": [
    {
      "itemTemplateKey": "stick",
      "dropChance": 10
    },
    {
      "itemTemplateKey": "rotten apple",
      "dropChance": 30
    }
  ]
}
```

## MapData Folder
Contains .map files containing tile data for maps  

### Example
lod0.map  
lod1.map  
lod2.map  

## MapInstances Folder
Contains subfolders, one for each map instance  
Multiple map instances can have the same numeric map id, but must have unique instance ids  
Best practice is for the folder name to match the map instance id  

## MapInstance Sub-Folder "Mileth"
Contains two .json files, "instance.json" and "spawns.json"  
instance.json contains basic information about the map instance  
spawns.json contains a collection of spawn objects  

### MapInstance Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string<br>(0-32767)|A string representation of the map id. Ex. 500 for mileth|
|Name|string|The name of the map that will display in-game|
|InstanceId|string|A unique name gives specifically to this map instance. Should match the folder name|
|Music|byte<br>(0-255)|The byte values of the music track to play when entering the map. These values arent explored yet, so you'll have to figure out what's available yourself|
|Flags|string<br>None<br>Snow<br>Rain<br>Darkness<br>NoTabMap<br>SnowTileset|A flag, or combination of flags that should affect the map. You can combine multiple flags by separating them with commas<br>Ex. "Snow, NoTabMap"|
|ScriptKeys|array<string>|A collection of script keys to load for this map (TODO: scripts section)|
|Warps|array<warp>|A collection of warps|

### Warp Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Source|point/string<br>"(X, Y)"|A string representation of a point.<br>The tile coordinates the warp is on.<br>Ex. "(50, 15)"|
|Destination|location/string<br>"MapInstanceId:(X, Y)"|A string representation of a location.<br>The map and coordinates of the map the warp sends you to when stepped on.<br> Ex. "500:(10, 10)"|

### Spawn Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|MonsterTemplateKey|string|A unique id for the template of the monster to spawn|
|LootTableKey|string|A unique id for the loot table used to determine monster drops from this spawn|
|IntervalSecs|int|A number of seconds between each trigger of this spawn|
|IntervalVariancePct|int(optional)|If specified, will randomize the interval by the percentage specified.<br>Ex. With an interval of 60, and a Variance of 50, the spawn interval would var from 30-90secs|
|MaxAmount|int|The maximum number of monsters that can be on the map from this spawn|
|MaxPerSpawn|int|The maximum number of monsters to create per interval of this spawn|
|AggroRange|int(optional)|If specified, monsters created by this spawn will be aggressive, and attack enemies if they come within the specified distance|
|MinGoldDrop|int|Minimum amount of gold for monsters created by this spawn to drop|
|MaxGoldDrop|int|Maximum amount of gold for monsters created by this spawn to drop|
|ExpReward|int|The amount of exp monsters created by this spawn will reward when killed|
|ExtraScriptKeys|array<string>|A collection of extra monster script keys to add to the monsters created by this spawn|
|SpawnArea|rectangle(optional)|If specified, monsters will only spawn within the specified bounds. If not specified, monsters will spawn anywhere on the map|

### Rectangle Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Top|int|The lowest Y coordinate of the rectangle|
|Left|int|The lowest X coordinate of the rectangle|
|Width|int|The width of the rectangle|
|Height|int|The height of the rectangle|

### Example instance.json
This is the mileth village map with the added flags of falling snow and usage of the snow tileset  
This map will have 2 warps to mileth village way  
The map has a quest script on it  

```json
{
  "templateKey": 500,
  "name": "Mileth",
  "instanceId": "Mileth1",
  "music": 1,
  "flags": "snow, snowtileset",
  "scriptKeys": [
    "SomeQuestScriptKey"
  ],
  "warps": [
    {
      "source": "(99, 30)",
      "destination": "3006:(0, 15)"
    },
    {
      "source": "(99, 31)",
      "destination": "3006:(0, 16)"
    }
  ]
}
```

### Example spawns.json
This will spawn 25 rats that drop sticks/apples/20-30 gold/12 exp every 126-236 seconds up to a maximum of 50 rats  
The rats will spawn at the bottom quadrant of the map  
They will aggressively target anyone who comes within 6 spaces of them  

```json
[
  {
    "monsterTemplateKey": "rat1",
    "lootTableKey": "rat1Sticks",
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
