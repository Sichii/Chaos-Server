namespace Chaos.Scripting.Abstractions;

public interface ICompositeScript<TScript> : IEnumerable<TScript> where TScript: IScript
{
    void Add(TScript script);

    T? GetComponent<T>() where T: TScript;

    void Remove(TScript script);
}