# Scripts

The server runs on scripts. Skills, Spells, Items, Dialogs, Monsters, Merchants, MapInstances, ReactorTiles, etc.. are all scripted objects.
To control
how these objects respond to input, a script must be written. Scripts can be found under `Chaos.Scripts`. There are a number of
default scripts created as examples, but writing your own scripts is the only way to truly customize the server.

There are 2 main types of scripts for most entities. `Base Scripts` and `Configurable Scripts`. Let's take item scripts as an example. There
are 2 base types you can choose to use, the [ItemScriptBase](<xref:Chaos.Scripts.ItemScripts.Abstractions.ItemScriptBase>) and
the [ConfigurableItemScriptBase](<xref:Chaos.Scripts.ItemScripts.Abstractions.ConfigurableItemScriptBase>).

### Base Scripts

Base scripts are just empty scripts with no functionality. They have empty virtual implementations of all of the possible events that could
occur for that object. These events are intended to be overridden and have functionality added.
> [!TIP]
> Base scripts are intended for use when no external configuration is necessary. A script inheriting from a BaseScript should generally do
> something specific

### Configurable Scripts

Configurable scripts are also empty scripts with all empty virtual implementations. The difference here is that these scripts are intended
to have variables passed into them from json. Configurable scripts have an underlying mechanism that populates protected read/writeable
variables
automatically if it can find that variable within the source json the entity was created from. Configurable scripts read from
the `scriptVars` property from whatever entity is the subject of that script.

For example, here is a script that is used for consumable items that give back health or mana.  
[!code-csharp[](../../Chaos/Scripts/ItemScripts/VitalityConsumableScript.cs)]  
Note the `scriptVars` region. These properties don't appear to be set anywhere, but they are automatically populated by the script's base
implementation from the item's json.

Here is a json for an `apple` item, note the `scriptVars` property  
[!code-json[](../../Data/Templates/Items/apple.json)]

Here the `scriptKeys` collection contains the `vitalityConsumable` script key, signaling that the `VitalityConsumableScript` should be
attached to this entity.
The `scriptVars` object here provides configuration for that script. If multiple configurable scripts are used, multiple configurations can
be specified.

> [!NOTE]
> The key of a script is the name of the class without "Script" at the end. So `VitalityConsumableScript` would have the
> key `VitalityConsumable`

```json
"scriptKeys": {
  "vitalityConsumable": {
    ...
  },
  "someQuest": {
    ...
  }
}
```