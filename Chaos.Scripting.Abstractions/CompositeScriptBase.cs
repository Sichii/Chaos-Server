using System.Collections;
using System.Runtime.InteropServices;

namespace Chaos.Scripting.Abstractions;

/// <summary>
///     A script that is composed of multiple scripts
/// </summary>
/// <inheritdoc cref="Chaos.Scripting.Abstractions.ICompositeScript{T}" />
public abstract class CompositeScriptBase<TScript> : ScriptBase, ICompositeScript<TScript> where TScript: IScript
{
    /// <summary>
    ///     The components of this script
    /// </summary>
    protected List<TScript> Scripts { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeScriptBase{TScript}" /> class.
    /// </summary>
    protected CompositeScriptBase() => Scripts = new List<TScript>();

    /// <inheritdoc />
    public void Add(TScript script) => Scripts.Add(script);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<TScript> GetEnumerator()
    {
        foreach (var component in Scripts)
            if (component is ICompositeScript<TScript> composite)
            {
                yield return component;

                foreach (var subComponent in composite)
                    yield return subComponent;
            } else
                yield return component;
    }

    /// <inheritdoc />
    public T? GetScript<T>()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            switch (script)
            {
                case T tScript:
                    return tScript;
                case ICompositeScript<TScript> composite:
                    return composite.GetScript<T>();
            }

        return default;
    }

    /// <inheritdoc />
    public IEnumerable<T> GetScripts<T>()
    {
        foreach (var script in Scripts)
            switch (script)
            {
                case T tScript:
                    yield return tScript;

                    break;
                case ICompositeScript<TScript> composite:
                    foreach (var subScript in composite.GetScripts<T>())
                        yield return subScript;

                    break;
            }
    }

    /// <inheritdoc />
    public void Remove(TScript script)
    {
        if (Scripts.Remove(script))
            return;

        foreach (var s in Scripts.ToList())
            if (s is ICompositeScript<TScript> composite)
                composite.Remove(script);
    }
}