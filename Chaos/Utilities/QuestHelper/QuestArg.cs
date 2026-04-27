namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Wraps a value of <typeparamref name="T" /> that may be supplied as a constant or as a
/// resolver evaluated per quest-step execution. Methods on <see cref="QuestStepBuilder{TStage}" />
/// accept <c>QuestArg&lt;T&gt;</c> so call sites can pass either a literal (lifted via the
/// implicit conversion) or a resolver via <see cref="QuestArg.From{T}" /> when the value
/// depends on per-execution state (random rolls, counters derived from the current Aisling,
/// etc.).
/// </summary>
/// <remarks>
/// Resolvers receive the non-generic <see cref="QuestContext" /> base. The base exposes
/// <c>Source</c>, <c>Subject</c>, <c>OptionIndex</c>, and <c>Services</c>, which is sufficient
/// for the common cases. A resolver that needs the typed <c>QuestContext&lt;TStage&gt;</c>
/// (for <c>CurrentStage</c> or stage-typed predicates) must cast inside the lambda.
/// </remarks>
public readonly struct QuestArg<T>
{
    private readonly T Value;
    private readonly Func<QuestContext, T>? Resolver;

    private QuestArg(T value)
    {
        Value = value;
        Resolver = null;
    }

    private QuestArg(Func<QuestContext, T> resolver)
    {
        Value = default!;
        Resolver = resolver;
    }

    /// <summary>
    /// Yields the wrapped value: either the captured constant or the resolver invoked with
    /// <paramref name="context" />.
    /// </summary>
    public T Resolve(QuestContext context) => Resolver is null ? Value : Resolver(context);

    /// <summary>Lifts a constant <typeparamref name="T" /> into a <see cref="QuestArg{T}" />.</summary>
    public static implicit operator QuestArg<T>(T value) => new(value);

    internal static QuestArg<T> Of(Func<QuestContext, T> resolver) => new(resolver);
}

/// <summary>
/// Static factory for building a <see cref="QuestArg{T}" /> from a per-execution resolver.
/// Pair with the implicit conversion on <see cref="QuestArg{T}" /> so builder methods can
/// take a single <c>QuestArg&lt;T&gt;</c> parameter and accept both constants and resolvers.
/// </summary>
public static class QuestArg
{
    /// <summary>
    /// Builds a <see cref="QuestArg{T}" /> that invokes <paramref name="resolver" /> once per
    /// chain execution. Use for values that must vary per dialog — randomized rewards,
    /// counters derived from current Aisling state, and similar.
    /// </summary>
    public static QuestArg<T> From<T>(Func<QuestContext, T> resolver) => QuestArg<T>.Of(resolver);
}
