using System.Collections;

namespace Chaos.Scripting.Abstractions;

public abstract class CompositeScriptBase<TScript> : ScriptBase, ICompositeScript<TScript> where TScript: IScript
{
    protected ICollection<TScript> Components { get; }

    protected CompositeScriptBase() => Components = new List<TScript>();

    public void Add(TScript script) => Components.Add(script);

    /// <inheritdoc />
    public T? GetComponent<T>() where T: TScript => this.OfType<T>().FirstOrDefault();

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

    public void Remove(TScript script) => Components.Remove(script);
}