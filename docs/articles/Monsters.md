# Monsters

Monsters in Chaos are both templated and scripted objects. When you define a monster, you are actually defining a
monster template that will be used to create new instances of that monster as requested. Each monster that is created is
a fresh instance of that monster created from it's template.

## Monster Templates

A monster template is used to create new instances of monsters as required. It contains all the information required to
create an instance of a new monster.

## How do I create them?

By default, monster templates should be created in `Data\Configuration\Templates\Monsters`. Configuration of how
monsters are loaded can be found in `appsettings.json` at `Options:MonsterTemplateCacheOptions`.

Monster templates are initially serialized
into [MonsterTemplateSchema](<xref:Chaos.Schemas.Templates.MonsterTemplateSchema>) before being mapped to
a [MonsterTemplate](<xref:Chaos.Models.Templates.MonsterTemplate>). The schema object is mapped via
the [MonsterMapperProfile](<xref:Chaos.Services.MapperProfiles.MonsterMapperProfile>).

See [MonsterTemplateSchema](<xref:Chaos.Schemas.Templates.MonsterTemplateSchema>) for a list of all configurable
properties with descriptions.

## How do I use them?

Monsters can be created by using the [MonsterFactory](<xref:Chaos.Services.Factories.MonsterFactory>), which is an
implementation of [IMonsterFactory](<xref:Chaos.Services.Factories.Abstractions.IMonsterFactory>).

[MapInstances](Maps.md) have a section for adding monster spawns to a map by their template key, but you can also create
them using the example below.

> [!NOTE]
> Each monster is a fresh instance of a monster created from a template. Any changes made to the template will apply
> to all instances of that monster.

```cs
private readonly IMonsterFactory MonsterFactory;

public Foo(IMonsterFactory monsterFactory) => MonsterFactory = monsterFactory;

public void Bar()
{
    //create a new instance of the monster "myMonsterTemplateKey"
    //mapInstance is the map instance that the monster will be added to
    //point is the point on the map that the monster will be added to
    //extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated monster
    var monster = MonsterFactory.Create("myMonsterTemplateKey", mapInstance, point, extraScriptKeys);
    
    //the monster is not automatically added to the map when created, so you must do so yourself
    mapInstance.AddObject(monster, point);
}
```

## Scripting

Monsters are scripted via [IMonsterScript](<xref:Chaos.Scripting.MonsterScripts.Abstractions.IMonsterScript>).

- Inherit from [MonsterScriptBase](<xref:Chaos.Scripting.MonsterScripts.Abstractions.MonsterScriptBase>) for a basic
  script that requires no external configuration
- Inherit
  from [ConfigurableMonsterScriptBase](<xref:Chaos.Scripting.MonsterScripts.Abstractions.ConfigurableMonsterScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `MonsterTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Monster` when it is created.

If the script is configurable, you must also have an entry for that script in the `MonsterTemplate.ScriptVars` property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in monster scripts:

| Event Name      | Description                                                                                                                                                              |
|-----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| CanMove         | Called when the monster is about to move. Return false to prevent the monster from moving. This can be used to check for status effects and other things                 |
| CanSee          | Called to check if the monster is able to see a `VisibleEntity`.                                                                                                         |
| CanTalk         | Called when the monster is about to talk. Return false to prevent the monster from talking. This can be used to check for status effects and other things                |
| CanTurn         | Called when the monster is about to turn. Return false to prevent the monster from turning. This can be used to check for status effects and other things                |
| CanUseSkill     | Called when the monster is about to use a skill. Return false to prevent the monster from using the skill. This can be used to check for status effects and other things |
| CanUseSpell     | Called when the monster is about to use a spell. Return false to prevent the monster from using the spell. This can be used to check for status effects and other things |
| OnApproached    | Called when any observable creature enters the monster's awareness (15 tiles), or becomes observable within that area                                                    |
| OnAttacked      | Called after the monster is attacked. This is called before OnDeath                                                                                                      |
| OnClicked       | Called when the monster is clicked                                                                                                                                       |
| OnDeath         | Called when the monster dies. This is called after OnAttacked                                                                                                            |
| OnDeparture     | Called when any observable creature exits the monster's awareness (15 tiles), or becomes unobservable within that area                                                   |
| OnGoldDroppedOn | Called after gold is dropped on the monster                                                                                                                              |
| OnHealed        | Called after the monster is healed                                                                                                                                       |
| OnItemDroppedOn | Called after an item is dropped on the monster                                                                                                                           |
| OnPublicMessage | Called after any creature sends a public message within the monster's awareness (15 tiles)                                                                               |
| OnSpawn         | Called after a monster has spawned. This is called after OnEntered and OnApproach                                                                                        |
| Update          | Called every time the map updates                                                                                                                                        |

## Example

Here is an example of a rat. It's a very standard creature, uses Assail every 1.5s, wanders every 2s, moves towards
targets every 1.5s, and has an aggro range of 6 tiles. It drops 10-30 gold and rewards 50 exp on kill.

[!code-json[](../../Data/Configuration/Templates/Monsters/common_rat.json)]