using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData;

/// <summary>
///     Represents a generic meta node mutator
/// </summary>
/// <typeparam name="T">The type of meta node to mutate</typeparam>
public sealed class MetaNodeMutator<T> : IMetaNodeMutator<T> where T: class
{
    private readonly Func<T, IEnumerable<T>> MutateAction;

    private MetaNodeMutator(Func<T, IEnumerable<T>> mutateAction) => MutateAction = mutateAction;

    /// <inheritdoc />
    public IEnumerable<T> Mutate(T obj) => MutateAction(obj);

    /// <summary>
    ///     Creates a new instance of the <see cref="MetaNodeMutator{T}" /> class
    /// </summary>
    /// <param name="mutateAction">The mutate action to perform</param>
    public static IMetaNodeMutator<T> Create(Func<T, IEnumerable<T>> mutateAction) => new MetaNodeMutator<T>(mutateAction);
}