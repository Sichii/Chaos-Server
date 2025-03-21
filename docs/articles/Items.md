# Items

Items in Chaos are both templated and scripted objects. When you define an item, you are actually defining an item
template that will be used to create new instances of that item as requested. Each item that is created is a fresh
instance of that item created from it's template.

## Item Templates

An item template is used to create new instances of items as required. The template defines the base properties of the
item, but the item itself can be modified separately (more on that later). As with all templates objects, the template
is shared between all instances of the item.

## How do I create them?

By default, item templates should be created in `Data\Configuration\Templates\Items`. Configuration of how item
templates are loaded can be found in `appsettings.json` at `Options:ItemTemplateCacheOptions`.

Item templates are initially serialized into [ItemTemplateSchema](<xref:Chaos.Schemas.Templates.ItemTemplateSchema>)
before being mapped to an [ItemTemplate](<xref:Chaos.Models.Templates.ItemTemplate>). The schema object is mapped via
the [ItemMapperProfile](<xref:Chaos.Services.MapperProfiles.ItemMapperProfile>).

See [ItemTemplateSchema](<xref:Chaos.Schemas.Templates.ItemTemplateSchema>) for a list of all configurable properties
with descriptions.

## How do I use them?

Items can be created by using the [ItemFactory](<xref:Chaos.Services.Factories.ItemFactory>), which is an implementation
of [IItemFactory](<xref:Chaos.Services.Factories.Abstractions.IItemFactory>).

> [!NOTE]
> Each item is a fresh instance of an item created from a template. Any changes made to the template will apply to all
> instances of that item.

```cs
private readonly IItemFactory ItemFactory;

public Foo(IItemFactory itemFactory) => ItemFactory = itemFactory;

public void Bar()
{
    // create a new instance of an apple (defaults to count of 1)
    // extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated item
    var item = ItemFactory.Create("apple", extraScriptKeys);
}
```

## Modifying items at runtime

Sometimes you may want to implement systems into your game that allow players to modify items. There are a couple of
things to know before implementing these systems.

- If you are allowing players to modify the stats on an item, the item must be flagged with `IsModifiable`. This allows
  the meta node of that item to be mutated.
- Modifications must be done through a script, preferably
  using [IEnchantmentScript](<xref:Chaos.Scripting.ItemScripts.Abstractions.IEnchantmentScript>)
- Items that have been modified must have a unique `DisplayName` that is specific to the modifications that were made
- `DisplayName` is a property of the item, not the template, and overrides the template's `Name` property
- You must mutate the meta node of the item so that there is a separate meta node for each modification.
  See [this article](MetaData.md#mutators) on how to do that

When the item is serialized back to file, the extra script key of the script you added that contains the modifications
will be serialized as well. This will allow that modification script to be re-applied when the item is deserialized.

To add a script to an object at runtime, just use the extension method `AddScript`.

## Notepad Text

Items have a `NotepadText` property that can be used to store text. This is in place to support the `DisplayNotepad` and
`SetNotepad` packets. This text is not guaranteed to persist through banking or stackable item transfer.

## Dyeable items

The color of the item (not the item template) can be changed, but in order for this change to be persisted,
the `ItemTemplate.IsDyeable` must be set to true.

## Scripting

Items are scripted via [IItemScript](<xref:Chaos.Scripting.ItemScripts.Abstractions.IItemScript>).

- Inherit from [ItemScriptBase](<xref:Chaos.Scripting.ItemScripts.Abstractions.ItemScriptBase>) for a basic script that
  requires no external configuration
- Inherit from [ConfigurableItemScriptBase](<xref:Chaos.Scripting.ItemScripts.Abstractions.ConfigurableItemScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `ItemTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Item` when it is created.

If the script is configurable, you must also have an entry for that script in the `ItemTemplate.ScriptVars` property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in item scripts:

| Event Name           | Description                                                                                                                                                                                                                                     |
|----------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| CanBeDropped         | Called before the item is dropped. Return false to prevent the item from being dropped                                                                                                                                                          |
| CanBeDroppedOn       | Called before the item is dropped on a creature. Return false to prevent the item from being dropped on the creature                                                                                                                            |
| CanBePickedUp        | Called before the item is picked up. Return false to prevent the item from being picked up                                                                                                                                                      |
| CanUse               | Called before the item is used. Return false to prevent the item from being used                                                                                                                                                                |
| OnDropped            | Called after the item is dropped, but before reactor tiles are notified                                                                                                                                                                         |
| OnEquipped           | Called after the item has been equipped                                                                                                                                                                                                         |
| OnNotepadTextUpdated | Called after the item's `NotepadText` property is updated                                                                                                                                                                                       |
| OnPickup             | Called after the item has been picked up. Beware that if a stackable item is picked up, this event will be fired from the item after it has been merged into another stack.<br/>Details about the original item will be properties in the event |
| OnUnEquipped         | Called after the item has been unequipped                                                                                                                                                                                                       |
| OnUse                | Called when the item is used. Provide functionality to items via this event                                                                                                                                                                     |
| Update               | Called every time the map updates. Items will update if they're on the ground, in an inventory, or equipped, but will not update if they are in the bank                                                                                        |

## Example

Here is an example of an item template json. This item template is for an apple. In the json you can see that the item
uses the [VitalityConsumable](<xref:Chaos.Scripting.ItemScripts.VitalityConsumableScript>) script. This script takes in
some configurable values that will determine how much health and mana are given when the item is used. The script will
also remove 1 of the item when it is used. In this case, our apple will provide 100 health when used.

[!code-json[](../../Data/Configuration/Templates/Items/apple.json)]