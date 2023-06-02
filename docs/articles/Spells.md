# Spells

Spells in Chaos are both templated and scripted objects. When you define a spell, you are actually defining a spell
template, and that spell template can be further changed via in-game systems. The spell remains associated to the
template to avoid having to serialize data that you have already defined.

## Spell Templates

A spell template is the definition of a spell. It contains all data that is common to all instances of that spell.

By default, Spell Templates are stored at `Data\Templates\Spells`. Configuration of how spell templates are loaded can
be found in `appsettings.json` at `Options:SpellTemplateCacheOptions`.

Spell templates are initially serialized into [SpellTemplateSchema](<xref:Chaos.Schemas.Templates.SpellTemplateSchema>)
before being mapped to a non-schema type.

### SpellTemplateSchema

| Type                                                                                | Name                 | Description                                                                                                                                                                                   |
|-------------------------------------------------------------------------------------|----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                                                              | TemplateKey          | A unique id specific to this template. This must match the file name                                                                                                                          |
| ICollection\<string\>                                                               | ScriptKeys           | A collection of names of scripts to attach to this object by default                                                                                                                          |
| IDictionary\<string, [DynamicVars](<xref:Chaos.Collections.Common.DynamicVars>)\>   | ScriptVars           | A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |
| string                                                                              | Name                 | The base name of the object                                                                                                                                                                   |
| string?                                                                             | Description          | A brief description of this entity                                                                                                                                                            |
| ushort                                                                              | PanelSprite          | The sprite id used to display the object in it's respective panel, minus the offset                                                                                                           |
| int                                                                                 | Level                | The level required to use this object                                                                                                                                                         |
| bool                                                                                | RequiresMaster       | Whether or not this object requires you to be a master                                                                                                                                        |
| [BaseClass](<xref:Chaos.Common.Definitions.BaseClass>)?                             | Class                | The class required to use this object                                                                                                                                                         |
| [AdvClass](<xref:Chaos.Common.Definitions.AdvClass>)?                               | AdvClass             | The advanced class required to use this object                                                                                                                                                |
| int?                                                                                | CooldownMs           | Defaults to null<br />If specified, any on-use effect of this object will use this cooldown                                                                                                   |
| [LearningRequirementsSchema]?(<xref:Chaos.Schemas.Data.LearningRequirementsSchema>) | LearningRequirements | Defaults to null<br/>If set, these are the requirements for the spell to be learned<br/>If null, the spell can't be learned                                                                   |
| byte                                                                                | CastLines            | The number of chant lines this spell requires by default                                                                                                                                      |
| string?                                                                             | Prompt               | Defaults to null<br/>Should be specified with a spell type of "Prompt", this is the prompt the spell will offer when used in game                                                             |
| [SpellType](<xref:Chaos.Common.Definitions.SpellType>)                              | SpellType            | The way the spell is cast by the player                                                                                                                                                       |

### Example Spell Template Json

Here is an example of a spell template json. This spell puts a regenerative effect on friendly targets in an area around
the primary target. It requires level 10, 2 wis, and 15 apples to learn.

[!code-json[](../../Data/Templates/Spells/regrowth.json)

## Modifying Spells

While spells are templated and technically modifiable, there is currently very little to actually modify. Most of the
behavior of spells is contained within scripts, which are not modifiable.