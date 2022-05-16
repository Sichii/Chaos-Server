using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public abstract class CompositeScriptBase<TScript> : ScriptBase, ICompositeScript<TScript> where TScript: IScript
{
    protected ICollection<TScript> Components { get; }

    protected CompositeScriptBase() => Components = new HashSet<TScript>();

    public void Add(TScript script) => Components.Add(script);
    public IEnumerator<TScript> GetEnumerator() => Components.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Remove(TScript script) => Components.Remove(script);
}