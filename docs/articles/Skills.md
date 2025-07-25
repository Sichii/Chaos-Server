# Skills

Skills in Chaos are both templated and scripted objects. When you define a skill, you are actually defining a skill
template that will be used to create new instances of that skill as requested. Each skill that is created is a fresh
instance of that skill created from it's template.

## Skill Templates

A skill template is used to create new instances of skills as required. The template defines the base properties of the
skill, but most of the effects of skills are determine through their scripts.

## How do I create them?

By default, skill templates should be created in `Data\Configuration\Templates\Skills`. Configuration of how skill
templates are loaded can be found in `appsettings.json` at `Options:SkillTemplateCacheOptions`.

Skill templates are initially serialized into [SkillTemplateSchema](<xref:Chaos.Schemas.Templates.SkillTemplateSchema>)
before being mapped to a [SkillTemplate](<xref:Chaos.Models.Templates.SkillTemplate>). The schema object is mapped via
the [SkillMapperProfile](<xref:Chaos.Services.MapperProfiles.SkillMapperProfile>).

See [SkillTemplateSchema](<xref:Chaos.Schemas.Templates.SkillTemplateSchema>) for a list of all configurable properties
with descriptions.

## How do I use them?

Skills can be created by using the [SkillFactory](<xref:Chaos.Services.Factories.SkillFactory>), which is an
implementation of [ISkillFactory](<xref:Chaos.Services.Factories.Abstractions.ISkillFactory>).

> [!NOTE]
> Each skill is a fresh instance of a skill created from a template. Any changes made to the template will apply to all
> instances of that skill.

```cs
private readonly ISkillFactory SkillFactory;

public Foo(ISkillFactory skillFactory) => SkillFactory = skillFactory;

public void Bar()
{
    // create a new instance of the "assail" skill
    // extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated skill
    var skill = SkillFactory.Create("assail", extraScriptKeys);
}
```

Every skill has a unique id, however, if you only want to create a skill for the purposes of finding or displaying
information about it, you can instead use `CreateFaux`. This will create a skill without a unique id.

## Scripting

Skills are scripted via [ISkillScript](<xref:Chaos.Scripting.SkillScripts.Abstractions.ISkillScript>).

- Inherit from [SkillScriptBase](<xref:Chaos.Scripting.SkillScripts.Abstractions.SkillScriptBase>) for a basic script
  that requires no external configuration
- Inherit
  from [ConfigurableSkillScriptBase](<xref:Chaos.Scripting.SkillScripts.Abstractions.ConfigurableSkillScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `SkillTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Skill` when it is created.

If the script is configurable, you must also have an entry for the script in the `SkillTemplate.ScriptVars` property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in skill scripts:

| Event Name | Description                                                                                     |
|------------|-------------------------------------------------------------------------------------------------|
| CanUse     | Called when a skill is about to be used. Return false to prevent the skill from being used      |
| OnUse      | Called when a skill is used. Provide functionality to skills via this event                     |
| Update     | Called every time the map updates. Skills will update only if they're in a creature's skillbook |

## Example

Here is an example of a skill that functions similarly to "Assail" from original DarkAges.

[!code-json[](../../Data/Configuration/Templates/Skills/assail.json)]