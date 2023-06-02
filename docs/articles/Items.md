# Items

Items in Chaos are both templated and scripted objects. When you define an item, you are actually defining an item
template, and that iem template can be further changed via in-game systems. The item remains associated to the template
to avoid having to serialize data that you have already defined.

## Item Templates

An item template is the definition of an item. It contains all data that is common to all instances of that item.

By default, Item Templates are stored at `Data\Templates\Items`. Configuration of how item templates are loaded can be
found in `appsettings.json` at `Options:ItemTemplateCacheOptions`.

Item templates are initially serialized into [ItemTemplateSchema](<xref:Chaos.Schemas.Templates.ItemTemplateSchema>)
before being mapped to a non-schema type.

### ItemTemplateSchema

| Type                                                                              | Name           | Description                                                                                                                                                                                    |
|-----------------------------------------------------------------------------------|----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                                                            | TemplateKey    | A unique id specific to this template. This must match the file name                                                                                                                           |
| ICollection\<string\>                                                             | ScriptKeys     | A collection of names of scripts to attach to this object by default                                                                                                                           |
| IDictionary\<string, [DynamicVars](<xref:Chaos.Collections.Common.DynamicVars>)\> | ScriptVars     | A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs  |
| string                                                                            | Name           | The base name of the object                                                                                                                                                                    |
| string?                                                                           | Description    | A brief description of this entity                                                                                                                                                             |
| ushort                                                                            | PanelSprite    | The sprite id used to display the object in it's respective panel, minus the offset                                                                                                            |
| int                                                                               | Level          | The level required to use this object                                                                                                                                                          |
| bool                                                                              | RequiresMaster | Whether or not this object requires you to be a master                                                                                                                                         |
| [BaseClass](<xref:Chaos.Common.Definitions.BaseClass>)?                           | Class          | The class required to use this object                                                                                                                                                          |
| [AdvClass](<xref:Chaos.Common.Definitions.AdvClass>)?                             | AdvClass       | The advanced class required to use this object                                                                                                                                                 |
| int?                                                                              | CooldownMs     | Defaults to null<br />If specified, any on-use effect of this object will use this cooldown                                                                                                    |
| bool                                                                              | AccountBound   | If the item is account bound, it cannot be traded or dropped                                                                                                                                   |
| int                                                                               | BuyCost        | The amount of gold it costs to buy this item from a merchant                                                                                                                                   |
| string                                                                            | Category       | The category of the object, used for bank or shop sorting                                                                                                                                      |
| [DisplayColor](<xref:Chaos.Common.Definitions.DisplayColor>)                      | Color          | Defaults to None(lavender)<br />If the item is dyeable, this is the dye color                                                                                                                  |
| ushort?                                                                           | DisplaySprite  | Default null<br />If specified, this is the sprite value used to display the item on the character when equipped                                                                               |
| [EquipmentType](<xref:Chaos.Common.Definitions.EquipmentType>)?                   | EquipmentType  | Default null<br />If specified, this is the type of equipment this item is                                                                                                                     |
| [Gender](<xref:Chaos.Common.Definitions.Gender>)?                                 | Gender         | Default null<br />If specified, this is the gender required to use this item                                                                                                                   |
| bool                                                                              | IsDyeable      | Whether or not the item can be dyed. This is specifically if an item can have it's colors changed. Town dyes on pre-99 armor is not dye.                                                       |
| bool                                                                              | IsModifiable   | Whether or not an instance of the item can be modified. This is specifically if an item can have it's stats/modifiers changed. Also controls if an item can be affected by meta node mutators. |
| int?                                                                              | MaxDurability  | Defaults to null<br />If specified, the base max durability of the item                                                                                                                        |
| int                                                                               | MaxStacks      | The maximum number of this item that can be in a stack. If set to 1, the item will not be stackable                                                                                            |
| [AttributesSchema](<xref:Chaos.Schemas.Aisling.AttributesSchema>)?                | Modifiers      | Defaults to null<br />If specified, these are the stats this item grants when equipped                                                                                                         |
| [DisplayColor](<xref:Chaos.Common.Definitions.DisplayColor>)?                     | PantsColor     | Default null<br />If specified, this armor will have pants, and they will be this color                                                                                                        |
| int                                                                               | SellValue      | The amount of gold given for selling this item to a merchant                                                                                                                                   |
| byte                                                                              | Weight         | The weight of the item in the inventory, or equipped                                                                                                                                           |

### Example Item Template Json

Here is an example of an item template json. This item template is an apple. In the json you can see that the item uses
the [VitalityConsumable](<xref:Chaos.Scripting.ItemScripts.VitalityConsumableScript>) script. This script takes in some
configurable values that will determine how much health and mana are given when the item is used. The script will also
remove 1 of the item when it is used. In this case, our apple will provide 100 health when used.

[!code-json[](../../Data/Templates/Items/apple.json)]

## Modifiable Items

Sometimes you may want to implement systems into your game that allow players to modify items. There are a couple of
things to know before implementing these systems.

- If you are allowing players to modify the stats on an item, the item must be flagged with `IsModifiable`
- Items that have been modified must have a unique `DisplayName` that is specific to the modifications that were made
    - `DisplayName` is a property of the item, not the template, and overrides the template's `Name` property
- You will need to create an implementation
  of [IMetaNodeMutator](<xref:Chaos.Services.Storage.Abstractions.IMetaNodeMutator`1>) for each modification system
    - More details about these mutators can be found in [this article](MetaData.md#mutators)

Normally when an item is serialized, only the template key, and some other minimal information is serialized. For
modified items though, all modifications will be serialized with the item. This may be changed in the future to allow
for preset modifications to be applied to an item via some sort of key.