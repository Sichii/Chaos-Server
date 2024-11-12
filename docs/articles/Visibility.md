# Visibility

Chaos has a 2 layer visibility system. To logically separate these layers in your mind, you can think of them
as `Observability` and `Visibility`.

The first layer, `Observability`, controls whether or not an entity will be sent to another entity. If an entity can not
observe another, it will never receive any information about it. This is controlled
by [CanObserve](<xref:Chaos.Models.World.Abstractions.Creature.CanObserve*>), and is not intended to be changed.

The second layer, `Visibility`, controls whether or not an entity is hidden to another entity. An entity can
be `Observable`, but not `Visible`. This means that EntityA will be sent to EntityB, but EntityB will not be able to see
it. However, just like in retail DarkAges, they can be casted on by clicking randomly. The client also knows that they
are there, so you wont be able to walk onto the same tile. This is controlled by the behavior
in [CanSee](<xref:Chaos.Scripting.CreatureScripts.Abstractions.ICreatureScript.CanSee*>). `CanSee` should be overridden
in scripts
and/or components to check for effects that cause you to be hidden, or see hidden/true hidden entities.

All [VisibleEntities](<xref:Chaos.Models.World.Abstractions.VisibleEntity>) have
a [VisibilityType](<xref:Chaos.Definitions.VisibilityType>). You can set this by calling
the [SetVisibility](<xref:Chaos.Models.World.Abstractions.VisibleEntity.SetVisibility*>) method. This means you can also
hide entities like `Monsters` and `Items`.

Another way of thinking of things is the following chart.

| VisibilityType | CanObserve | CanSee            | ExpectedEffect                                                                                                                           |
|----------------|------------|-------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| Normal         | True       | True              | Entity is displayed normally                                                                                                             |
| Hidden         | True       | Decided by script | Entity has no body sprite by default<br/>Aisling has transparent image when CanSee<br/>Other entities has alternative sprite when CanSee |
| TrueHidden     | False      | Decided by script | Entity is not observed by default<br/>Aisling has transparent image when CanSee<br/>Other entities has alternative sprite when CanSee    |
| GmHidden       | False      | False             | Unobservable, except by admins                                                                                                           |

> [!TIP]
> All visibility types are visible to admins