using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;

namespace Chaos.Scripting.Components.Utilities;

public sealed class ComponentExecutor
{
    private readonly ComponentVars Vars;
    private ActivationContext Context;

    public ComponentExecutor(ActivationContext context, ComponentVars vars)
    {
        Context = context;
        Vars = vars;
    }

    public ComponentExecutor(ComponentVars vars)
    {
        Vars = vars;
        Context = new ActivationContext(null!, null!);
    }

    public ComponentExecutor(ActivationContext context)
        : this(context, new ComponentVars()) { }

    public ComponentExecutor(Creature source, Creature target)
        : this(new ActivationContext(source, target)) { }

    public ComponentExecutor()
    {
        Vars = new ComponentVars();
        Context = new ActivationContext(null!, null!);
    }

    public ComponentExecutor? Check(Func<ComponentVars, bool> predicate)
    {
        if (predicate(Vars))
            return this;

        return null;
    }

    public ComponentExecutor Execute<TComponent>()
        where TComponent: IComponent, new()
    {
        var component = new TComponent();
        component.Execute(Context, Vars);

        return this;
    }

    public ComponentExecutor? ExecuteAndCheck<TComponent>() where TComponent: IConditionalComponent, new()
    {
        var component = new TComponent();

        if (component.Execute(Context, Vars))
            return this;

        return null;
    }

    public ComponentExecutor WithContext(ActivationContext context)
    {
        Context = context;

        return this;
    }

    public ComponentExecutor WithOptions(object options)
    {
        Vars.SetOptions(options);

        return this;
    }

    public ComponentExecutor WithSource(Creature source)
    {
        Context = new ActivationContext(source, Context.Target, Context.TargetMap!);

        return this;
    }

    public ComponentExecutor WithTarget(Creature target)
    {
        Context = new ActivationContext(Context.Source, target);

        return this;
    }

    public ComponentExecutor WithTarget(IPoint target, MapInstance targetMap)
    {
        Context = new ActivationContext(Context.Source, target, targetMap);

        return this;
    }
}