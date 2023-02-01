namespace Chaos.Services.Storage.Abstractions;

public interface IMetaNodeMutator<T> where T: class
{
    IEnumerable<T> Mutate(T obj);
}