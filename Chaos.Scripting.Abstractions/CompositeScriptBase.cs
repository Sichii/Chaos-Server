using System.Collections;

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
    protected List<TScript> Components { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeScriptBase{TScript}" /> class.
    /// </summary>
    protected CompositeScriptBase() => Components = new List<TScript>();

    /// <inheritdoc />
    public void Add(TScript script) => Components.Add(script);

    /// <inheritdoc />
    public T? GetComponent<T>() => this.OfType<T>().FirstOrDefault();

    /// <inheritdoc />
    public IEnumerable<T> GetComponents<T>() => this.OfType<T>();

    /// <inheritdoc />
    public IEnumerator<TScript> GetEnumerator()
    {
        foreach (var component in Components)
            if (component is ICompositeScript<TScript> composite)
            {
                yield return component;

                foreach (var subComponent in composite)
                    yield return subComponent;
            } else
                yield return component;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Remove(TScript script) => Components.Remove(script);
}