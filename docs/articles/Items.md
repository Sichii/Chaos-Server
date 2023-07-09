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
before being mapped to an [ItemTemplate](<xref:Chaos.Models.Template.ItemTemplate>). The schema object is mapped via
the [ItemMapperProfile](<xref:Chaos.Services.MapperProfiles.ItemMapperProfile>).

See [ItemTemplateSchema](<xref:Chaos.Schemas.Templates.ItemTemplateSchema>) for a list of all configurable properties
with descriptions.

## How do I use them?

Items can be created by using the [ItemFactory](<xref:Chaos.Services.Factories.ItemFactory>), which is an implementation
of [IItemFactory](<xref:Chaos.Services.Factories.Abstractions.IItemFactory).

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

When the item is serialize back to file, the extra script key of the script you added that contains the modifications
will be serialized as well. This will allow that modification script to be re-applied when the item is deserialized.

To add a script to an object at runtime, just use the extension method `AddScript`.

## Dyeable items

The color of the item (not the item template) can be changed, but in order for this change to be persisted,
the `ItemTemplate.IsDyeable` must be set to true.

## Example

Here is an example of an item template json. This item template is for an apple. In the json you can see that the item
uses the [VitalityConsumable](<xref:Chaos.Scripting.ItemScripts.VitalityConsumableScript>) script. This script takes in
some configurable values that will determine how much health and mana are given when the item is used. The script will
also remove 1 of the item when it is used. In this case, our apple will provide 100 health when used.

[!code-json[](../../Data/Configuration/Templates/Items/apple.json)]