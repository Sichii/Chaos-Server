# Chaos
A configurable Dark Ages server emulator 

# Folder Structure
📂Data  
 ┣📂[LootTables](#loottables-folder)  
 ┃ ┗📜rat1Sticks.json  
 ┣📂[MapData](#mapdata-folder)  
 ┃ ┣📜lod500.map  
 ┃ ┗📜lod3006.map  
 ┣📂[MapInstances](#mapinstances-folder)  
 ┃ ┣📂mileth1  
 ┃ ┃ ┣📜instance.json  
 ┃ ┃ ┗📜spawns.json  
 ┃ ┗📂milethVillageWay1  
 ┃   ┣📜instance.json  
 ┃   ┗📜spawns.json  
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
   ┃ ┗📜rat1.json  
   ┣📂[Skills](#skills-folder)  
   ┃ ┗📜assail.json  
   ┗📂[Spells](#spells-folder)  
     ┗📜srad tut.json
     
# LootTables Folder
 Contains .json files such as "lootTableKey.json" that are used to determine loot drops for monsters  

### LootTable Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Key|string|A unique id specific to this loot table. Best practice is to match the file name|
|LootDrops|array{[lootDrop](#lootdrop-properties)}|A collection of lootDrops. Every item in the list is calculated, allowing multiple drops|

### LootDrop Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|ItemTemplateKey|string|A unique id specific to the template of the item that should drop|
|DropChance|number<br>(0-100)|The chance of the item to drop|

### Example file "rat1Sticks.json"
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
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string<br>(0-32767)|A string representation of the map id. Ex. 500 for mileth|
|Name|string|The name of the map that will display in-game|
|InstanceId|string|A unique id specific to this map instance<br>Best practice is to match the folder name|
|Music|number<br>(0-255)|The byte values of the music track to play when entering the map<br>These values aren't explored yet, so you'll have to figure out what's available yourself|
|Flags|string<br>None<br>Snow<br>Rain<br>Darkness<br>NoTabMap<br>SnowTileset|A flag, or combination of flags that should affect the map<br>You can combine multiple flags by separating them with commas<br>Ex. "Snow, NoTabMap"|
|ScriptKeys|array{string}|A collection of script keys to load for this map (TODO: scripts section)|
|Warps|array{[warp](#warp-properties)}|A collection of warps|

### Warp Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Source|string<br>"(X, Y)"|A string representation of a point.<br>The tile coordinates the warp is on<br>Ex. "(50, 15)"|
|Destination|string<br>"MapInstanceId:(X, Y)"|A string representation of a location<br>The map and coordinates of the map the warp sends you to when stepped on<br> Ex. "500:(10, 10)"|

### Spawn Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|MonsterTemplateKey|string|The unique id for the template of the monster to spawn|
|LootTableKey|string|The unique id for the loot table used to determine monster drops from this spawn|
|IntervalSecs|number|A number of seconds between each trigger of this spawn|
|IntervalVariancePct|number(optional)|Defaults to 0<br>If specified, will randomize the interval by the percentage specified<br>Ex. With an interval of 60, and a Variance of 50, the spawn interval would var from 45-75secs|
|MaxAmount|number|The maximum number of monsters that can be on the map from this spawn|
|MaxPerSpawn|number|The maximum number of monsters to create per interval of this spawn|
|AggroRange|number(optional)|Defaults to 0<br>If specified, monsters created by this spawn will be aggressive and attack enemies if they come within the specified distance|
|MinGoldDrop|number|Minimum amount of gold for monsters created by this spawn to drop|
|MaxGoldDrop|number|Maximum amount of gold for monsters created by this spawn to drop|
|ExpReward|number|The amount of exp monsters created by this spawn will reward when killed|
|ExtraScriptKeys|array{string}|A collection of extra monster script keys to add to the monsters created by this spawn|
|SpawnArea|[rectangle](#rectangle-properties)(optional)|Defaults to spawn on entire map<br>If specified, monsters will only spawn within the specified bounds|

### Rectangle Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Top|number<br>(0-255)|The lowest Y coordinate of the rectangle|
|Left|number<br>(0-255)|The lowest X coordinate of the rectangle|
|Width|number<br>(0-255)|The width of the rectangle|
|Height|number<br>(0-255)|The height of the rectangle|

### Example file "instance.json"
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

### Example file "spawns.json"
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

# Metafiles Folder
 Not implemented yet

# Templates Folder
 Contains subfolders for each type of template  

## Items Folder
 Contains .json files to be used as blueprints for items

### ItemTemplate Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string|A unique id specific to this item template. Best practice is to match the file name|
|Name|string|The base name of the item|
|AccountBound|bool<br>true/false|If the item is account bound, it cannot be traded or dropped|
|AdvClass|string(optional)<br>None<br>Gladiator<br>Druid<br>Archer<br>Bard<br>Summoner|Defaults to null<br>If specified, this advanced class flag is required to equip this item|
|BaseClass|string(optional)<br>Peasant<br>Warrior<br>Rogue<br>Wizard<br>Priest<br>Monk|Defaults to null<br>If specified, this base class flag is required to equip this item|
|Color|string<br>[see below](#color-options)|Defaults to None(lavender)<br>If the item is dyeable, this is the dye color|
|DisplaySprite|number(optional)|Defaults to null<br>If specified, this is the sprite used to display the item on character when it is equipped|
|EquipmentType|string(optional)<br>[see below](#equipmenttype-options)|Default to null<br>If specified, this is type of equipment the item is, determining what slot it can be equipped to|
|PanelSprite|number<br>(1-500)|The sprite id used to display the item in the inventory, minus the offset|
|Gender|string(optional)<br>Male<br>Female<br>Unisex|Defaults to null<br>If specified, player must be of this gender to equip the item|
|MaxDurability|number(optional)|Defaults to null<br>If specified, the base max durability of the item|
|MaxStacks|number|The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable|
|Value|number|Not fully implemented|
|Weight|number<br>(0-255)|The weight of the item in the inventory, or equipped|
|CooldownMs|number(optional)|Defaults to null. If specified, any on-use effect of this item will use this cooldown|
|Animation|[animation](#animation-properties)(optional)|Defaults to null. If specified, this will be used by any on-use effect|
|ScriptKeys|array{string}|A collection of names of item scripts to attach to this item by default|
|Modifiers|[attributes](#attributes-properties)(optional)|Defaults to null<br>If specified, these are the stats this item grants when equipped|

### Animation Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|AnimationSpeed|number|Defaults to 1000<br>How fast the animation plays, lower is faster|
|SourceAnimation|number|The id of the animation to play on the source of this action|
|TargetAnimation|number|The id of the animation to play on the target of this action|
 
### Attributes Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Ac|number<br>(-127-127)||
|Str|number<br>(0-255)||
|Int|number<br>(0-255)||
|Wis|number<br>(0-255)||
|Con|number<br>(0-255)||
|Dex|number<br>(0-255)||
|Hit|number<br>(0-255)||
|Dam|number<br>(0-255)||
|MagicResistance|number||
|MaximumHp|number||
|MaximumMp|number||
 
#### Color Options
|||||||
|-|-|-|-|-|-|
|None|Black|Red|Orange|Blonde|Cyan|
|Blue|Mulberry|Olive|Green|Fire|Brown|
|Grey|Navy|Tan|White|Pink|Chartreuse|
|Golden|Lemon|Royal|Platinum|Lilac|Fuchsia|
|Magenta|Peacock|NeonPink|Arctic|Mauve|NeonOrange|
|Sky|NeonGreen|Pistachio|Corn|Cerulean|Chocolate|
|Ruby|Hunter|Crimson|Ocean|Ginger|Mustard|
|Apple|Leaf|Cobalt|Strawberry|Unusual|Sea|
|Harlequin|Amethyst|NeonRed|NeonYellow|Rose|Salmon|
|Scarlet|Honey|||||
 
#### EquipmentType Options
|||||||
|-|-|-|-|-|-|
|NotEquipment|Weapon|Armor|OverArmor|Shield|Helmet|
|OverHelmet|Earrings|Necklace|Ring|Gauntlet|Belt|
|Greaves|Boots|Accessory||||
 
### Example file "stick.json"
 A basic stick item that gives 1 str
  
```json
{
  "templateKey": "stick",
  "accountBound": false,
  "baseClass": "Peasant",
  "advClass": "None",
  "displaySprite": 1,
  "equipmentType": "Weapon",
  "gender": "Unisex",
  "maxDurability": 1000,
  "maxStacks": 1,
  "modifiers": {
    "str": 1
  },
  "value": 1000,
  "weight": 1,
  "name": "Stick",
  "panelSprite": 86,
  "scriptKeys": ["Equipment"]
}
```
 
## Maps Folder
 Contains .json files to be used as blueprints for maps  
 Each template should match up to the numeric id of a mapdata file
  
### MapTemplate Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string|A unique id specific to this map template<br>Best practice is to match the name of the file, and use the numeric id the map this template is for|
|Width|number<br>(1-255)|The width of the map|
|Height|number<br>(1-255)|The height of the map|
|WarpPoints|array{string}|The coordinates of each warp tile on the map|
|ScriptKeys|array{string}|A collection of names of map scripts to attach to this map by default|
  
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
  "scriptKeys": [
    "someQuestScript"
  ]
}
```
  
## Monsters Folder
 Contains .json files to be used as blueprints for monsters  
 
### MonsterTemplate Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string|A unique id specific to this monster template<br>Best practice is to match the name of the file|
|Name|string|The name of the monster when double clicked|
|Sprite|number<br>(1-1500)|The sprite id of the monster minus the offset|
|Type|string<br>Normal<br>WalkThrough<br>WhiteSquare|The monster's type. WhiteSquare has no additional functionality, it just appears as a white square on the tab map|
|Direction|string<br>Up<br>Down<br>Left<br>Right|The initial direction of the monster when spawned|
|WanderingIntervalMs|number|The number of miliseconds between movements while this monster is wandering when it has no target|
|MoveIntervalMs|number|The number of miliseconds between movements while this monster is targeting an enemy|
|AttackIntervalMs|number|The number of miliseconds between usages of skills|
|CastIntervalMs|number|The number of miliseconds between usages of spells|
|StatSheet|[statsheet](#statsheet-properties)|The base stats of this monster|
|ScriptKeys|array{string}|A collection of names of monsters scripts to attach to this monster<br>TODO: scripts section|
|SkillTemplateKeys|array{string}|A collection of template keys of skills this monster will use|
|SpellTemplateKeys|array{string}|A collection of template keys of spells this monster will cast|

### StatSheet Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|Ability|number|The ability level of this monster|
|Level|number|The level of this monster|
|Ac|number<br>(-127-127)||
|Str|number<br>(0-255)||
|Int|number<br>(0-255)||
|Wis|number<br>(0-255)||
|Con|number<br>(0-255)||
|Dex|number<br>(0-255)||
|Hit|number<br>(0-255)||
|Dam|number<br>(0-255)||
|MagicResistance|number||
|MaximumHp|number||
|MaximumMp|number||

### Example file "rat1.json"
```json
{
  "TemplateKey": "rat1",
  "StatSheet": {
    "Ability": 0,
    "Level": 1,
    "MaximumHp": 100,
    "MaximumMp": 100,
    "Ac": 50,
    "Str": 1,
    "Int": 1,
    "Wis": 1,
    "Con": 2,
    "Dex": 1,
  },
  "Type": "Normal",
  "Direction": "Down",
  "Name": "Common Rat",
  "Sprite": 7,
  "WanderIntervalMs": 2000,
  "MoveIntervalMs": 1500,
  "AttackIntervalMs": 1500,
  "CastIntervalMs": 10000,
  "SpellTemplateKeys": [],
  "SkillTemplateKeys": [ "assail" ],
  "ScriptKeys": [
    "commonMonster"
  ]
}
```

## Skills Folder
 Contains .json files to be used as blueprints for skills
 
### SkillTemplate Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string|A unique id specific to this skill template. Best practice is to match the file name|
|Name|string|The base name of the skill|
|PanelSprite|number<br>(1-500)|The sprite id used to display the skill in the skill pane|
|CooldownMs|number(optional)|Defaults to null. If specified, any on-use effect of this skill will use this cooldown|
|Animation|[animation](#animation-properties)(optional)|Defaults to null. If specified, this will be used by any on-use effect|
|ScriptKeys|array{string}|A collection of names of skill scripts to attach to this skill by default|
|IsAssail|bool<br>true/false|Whether or not the skill is an assail and should be used when spacebar is pressed|

### Example file "assail.json"
```json
{
  "templateKey": "assail",
  "name": "Assail",
  "panelSprite": 1,
  "cooldownMs": 1500,
  "scriptKeys": [
    "damageSkill"
  ],
  "isAssail": true
}
```

## Spells Folder
 Contains .json files to be used as blueprints for spells
 
### SpellTemplate Properties
|Name|Type/Values|Description|
|:---|:----:|:---|
|TemplateKey|string|A unique id specific to this spell template. Best practice is to match the file name|
|Name|string|The base name of the spell|
|PanelSprite|number<br>(1-500)|The sprite id used to display the spell in the skill pane|
|CooldownMs|number(optional)|Defaults to null. If specified, any on-use effect of this spell will use this cooldown|
|Animation|[animation](#animation-properties)(optional)|Defaults to null. If specified, this will be used by any on-use effect|
|ScriptKeys|array{string}|A collection of names of spell scripts to attach to this spell by default|
|CastLines|number<br>(0-9)|The number of chant lines this spell requires by default|
|SpellType|string<br>None<br>Prompt<br>Targeted<br>Prompt4Nums<br>Prompt3Nums<br>NoTarget<br>Prompt2Nums<br>Prompt1Num|The way the spell is cast by the player|
|Prompt|string(optional)|Defaults to null<br>Should be specified with a spell type of "Prompt", this is the prompt the spell will offer when cast|

### Example file "srad tut.json"
```json
{
  "templateKey": "srad tut",
  "name": "Srad Tut",
  "panelSprite": 40,
  "animation": {
    "animationSpeed": 1000,
    "targetAnimation": 12
  },
  "scriptKeys": [
    "damageSpell"
  ],
  "castLines": 1,
  "spellType": "targeted"
}
```
