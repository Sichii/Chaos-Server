# Components

Components are a way of separating functionality into smaller, more manageable pieces. They let you pull pieces of
logic out of scripts so that you can reuse them in multiple places and multiple different script types. For example,
both skills and spells can have animations, body animations, sounds, deal damage, and more. Each of these things can be
separated into their own components.

## Component types

There are 2 kinds of components: [IComponent](<xref:Chaos.Scripting.Components.Abstractions.IComponent>)
and [IConditionalComponent](<xref:Chaos.Scripting.Components.Abstractions.IConditionalComponent>).

`IComponent` is used to execute a piece of logic when nothing depends on its execution. For example, animation or
sound. There is no way for these actions to "fail", and thus nothing is going to depend on those components' execution.
Most components will be of this type.

```csharp
public class MyComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        // Do something
    }
}
```

`IConditionalComponent` is used when something depends on the execution of the component. For example, a component that
makes an ability occasionally fail, or a component to make abilities cost mana. If components like these fail, the rest
of the components that come after it should not execute.

```csharp
public class MyConditionalComponent : IConditionalComponent
{
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        // Do something
        
        // Return true if the component succeeded, false if it failed
        return true;
    }
}
```

## Component options

Any component that has options associated with it should create an interface that specifies those options. The component
should expect its options to be contained within the `ComponentVars` passed to the `Execute` method of the component.
The options can be obtained via `ComponentVars.GetOptions<TOptions>()`.

```csharp
public class MyComponent : IComponent
{
    public interface IMyComponentOptions
    {
        // Options go here
    }

    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IMyComponentOptions>();
        
        // Do something with the options
    }
}
```

Any component that utilizes another component should ensure that its own options interface inherits from the options
interfaces of its subcomponents.

```csharp
public class MyComponent : IComponent
{
    public interface IMyComponentOptions : SubComponent.ISubComponentOptions
    {
        // Options go here
        // Since this inherits from SubComponent.ISubComponentOptions, it will also contain the options for SubComponent
    }

    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IMyComponentOptions>();
        
        // Do something with the options
        
        new ComponentExecutor(context, vars)
            .Execute<SubComponent>();
    }
}
```

## Variable Sharing

Components can share variables with each other via the `ComponentVars` object. Some examples of this
are `vars.GetPoints()` and `vars.GetTargets<T>()`. These are populated by another component,
the [GetTargetsComponent](<xref:Chaos.Scripting.Components.AbilityComponents.GetTargetsAbilityComponent`1>). The only
caveat is, any
components that use variables populated by another component must be executed after that component.

```csharp
public class MyComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();
        
        // Do something with the targets
    }
}
```

If you create a component that needs to share variables with other components, you should create methods
inside `ComponentVars` to store and retrieve that information.

## Scripting

Components are intended to be used in scripts. The first step is to know what components you will be utilizing within
your script. For example, if you want to create a script that plays a body animation, gets targets, deals damage to
those targets, animates those targets, and plays a sound, it might look something like this.

```csharp
    public class DamageScript : ConfigurableSpellScriptBase,
                                BodyAnimationComponent.IBodyAnimationComponentOptions,
                                GetTargetsComponent<Creature>.IGetTargetsComponentOptions,
                                DamageComponent.IDamageComponentOptions,
                                AnimationComponent.IAnimationComponentOptions,
                                SoundComponent.ISoundComponentOptions
    {
        public override OnUse(SpellContext context) =>
            new ComponentExecutor(context)
                .WithOptions(this)
                .Execute<BodyAnimationComponent>()
                .ExecuteAndCheck<GetTargetsComponent<Creature>>()
                ?
                .Execute<DamageComponent>()
                .Execute<AnimationComponent>()
                .Execute<SoundComponent>();
        
        #region ScriptVars
        // All of the combined options from the component options interfaces
        #endregion
    }
```

In the above example, the script is utilizing 5 components. Each of those components has an options interface that the
script inherits from. The script also inherits from the `ConfigurableSpellScriptBase`. More about this script base can
be found in the [Scripting article](Scripting.md).

Another important thing to notice is that the `GetTargetsComponent<T>` is an `IConditionalComponent`. These types of
components are executed via `ExecuteAndCheck<TComponent>()`. If the conditional part of the component fails, it causes
the execution of `ExecuteAndCheck<TComponent>()` to return `null`. By using the `Save Navigation Operator (?)`, we can
cause all of the method invocations that come after it to not occur if it returns `null`.