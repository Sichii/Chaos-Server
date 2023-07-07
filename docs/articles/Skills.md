# Skills

Skills in Chaos are both templated and scripted objects. When you define a skill, you are actually defining a skill
template, and that skill template can be further changed via in-game systems. The skill remains associated to the
template to avoid having to serialize data that you have already defined.

## Skill Templates

A skill template is the definition of a skill. It contains all data that is common to all instances of that skill.

By default, Skill Templates are stored at `Data\Configuration\Templates\Skills`. Configuration of how skill templates
are loaded can
be found in `appsettings.json` at `Options:SkillTemplateCacheOptions`.

Skill templates are initially serialized into [SkillTemplateSchema](<xref:Chaos.Schemas.Templates.SkillTemplateSchema>)
before being mapped to a non-schema type.

### SkillTemplateSchema

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
| bool                                                                                | IsAssail             | Whether or not the skill is an assail and should be used when spacebar is pressed<br/>Assail cooldowns are handled by AssailIntervalMs and AtkSpeedPct                                        |
| [LearningRequirementsSchema](<xref:Chaos.Schemas.Data.LearningRequirementsSchema>)? | LearningRequirements | Defaults to null<br/>If set, these are the requirements for the skill to be learned<br/>If null, the skill can't be learned                                                                   |

### Example Skill Template Json

Here is an example of a skill template json. The skill is a basic assail that all classes can learn. It requires level
1, 2 str, and 20 apples to learn.

[!code-json[](../../Data/Configuration/Templates/Skills/assail.json)

## Modifying Skills

While skills are templated and technically modifiable, there is currently very little to actually modify. Most of the
behavior of skills is contained within scripts, which are not modifiable.