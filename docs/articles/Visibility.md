# Visibility

Chaos has a 2 layer visibility system. To logically seperate these layers in your mind, you can think of them
as `Observability` and `Visibility`.

All [VisibleEntities](<xref:Chaos.Models.World.Abstractions.VisibleEntity>) have
a [VisibilityType](<xref:Chaos.Definitions.VisibilityType>). You can set this by calling
the [SetVisibility](<xref:Chaos.Models.World.Abstractions.VisibleEntity.SetVisibility*>) method.

The first layer, `Observability`, controls whether or not an entity will be sent to another entity. If an entity can not
observe another, it will never receive any information about it. This is controlled
by [CanObserve](<xref:Chaos.Models.World.Abstractions.VisibleEntity.CanObserve*>), and is not intended to be changed.

The second layer, `Visibility`, controls whether or not an entity is hidden to another entity. An entity can
be `Observable`, but not `Visible`. This means that EntityA will be sent to EntityB, but EntityB will not be able to
clearly see it. However, just like in retail DarkAges, they can be casted on. This is controlled by the behavior
in [CanSee](<xref:Chaos.Scripting.Abstractions.ICreatureScript.CanSee*>). `CanSee` should be overridden in scripts
and/or components to check for effects that cause you to be hidden, or see hidden/true hidden entities.

All `VisibleEntitities` are able to have their `VisibilityType` changed. This means you can also hide entities
like `Monsters` and `Items`.

Another way of thinking of things is the following chart.

| VisibilityType | CanObserve | CanObserveWhenCanSee | ExpectedEffect                                                                                                                           |
|----------------|------------|----------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| Normal         | True       | True                 | Entity is displayed normally                                                                                                             |
| Hidden         | True       | True                 | Entity has no body sprite by default<br/>Aisling has transparent image when CanSee<br/>Other entities has alternative sprite when CanSee |
| TrueHidden     | False      | True                 | Entity is not observed by default<br/>Aisling has transparent image when CanSee<br/>Other entities has alternative sprite when CanSee    |
| GmHidden       | False      | False                | Unobservable, except by admins                                                                                                           |

> [!TIP]
> All visibility types are visible to admins