# Spells

Spells in Chaos are both templated and scripted objects. When you define a spell, you are actually defining a spell
template that will be used to create new instances of that spell as requested. Each spell that is created is a fresh
instance of that spell created from it's template.

## Spell Templates

A spell template is used to create new instances of spells as required. The template defines the base properties of the
spell, but most of the effects of spells are determine through their scripts.

## How do I create them?

By default, spell templates should be created in `Data\Configuration\Templates\Spells`. Configuration of how spell
templates are loaded can be found in `appsettings.json` at `Options:SpellTemplateCacheOptions`.

Spell templates are initially serialized into [SpellTemplateSchema](<xref:Chaos.Schemas.Templates.SpellTemplateSchema>)
before being mapped to a [SpellTemplate](<xref:Chaos.Models.Templates.SpellTemplate>). The schema object is mapped via
the [SpellMapperProfile](<xref:Chaos.Services.MapperProfiles.SpellMapperProfile>).

See [SpellTemplateSchema](<xref:Chaos.Schemas.Templates.SpellTemplateSchema>) for a list of all configurable properties
with descriptions.

## How do I use them?

Spells can be created by using the [SpellFactory](<xref:Chaos.Services.Factories.SpellFactory>), which is an
implementation of [ISpellFactory](<xref:Chaos.Services.Factories.Abstractions.ISpellFactory>).

> [!NOTE]
> Each spell is a fresh instance of an spell created from a template. Any changes made to the template will apply to all
> instances of that spell.

```cs
private readonly ISpellFactory SpellFactory;

public Foo(ISpellFactory spellFactory) => SpellFactory = spellFactory;

public void Bar()
{
    // create a new instance of the "hide" spell
    // extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated spell
    var spell = SpellFactory.Create("hide", extraScriptKeys);
}
```

Every spell has a unique id, however, if you only want to create a spell for the purposes of finding or displaying
information about it, you can instead use `CreateFaux`. This will create a spell without a unique id.

## Scripting

Spells are scripted via [ISpellScript](<xref:Chaos.Scripting.SpellScripts.Abstractions.ISpellScript>).

- Inherit from [SpellScriptBase](<xref:Chaos.Scripting.SpellScripts.Abstractions.SpellScriptBase>) for a basic script
  that requires no external configuration
- Inherit
  from [ConfigurableSpellScriptBase](<xref:Chaos.Scripting.SpellScripts.Abstractions.ConfigurableSpellScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `SpellTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Spell` when it is created.

If the script is configurable, you must also have an entry for the script in the `SpellTemplate.ScriptVars` property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in spell scripts:

| Event Name | Description                                                                                   |
|------------|-----------------------------------------------------------------------------------------------|
| CanUse     | Called when a spell is about to be used. Return false to prevent the spell from being used    |
| OnUse      | Called when a spell is used. Provide functionality to spells via this event                   |
| Update     | Called every time the map updates. Spells will update only if they're in a player's spellbook |

## Example

Here is an example of a spell that functions similarly to "Hide" from original DarkAges.

[!code-json[](../../Data/Configuration/Templates/Spells/hide.json)]