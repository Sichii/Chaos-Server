namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class that simulates switch-expression behavior on type objects
/// </summary>
/// <typeparam name="TResult">The return type of the switch expression</typeparam>
public sealed class TypeSwitchValueExpression<TResult>
{
    private readonly Dictionary<Type, TResult> Cases = new();
    private TResult? DefaultCase;
    private bool DefaultSet;

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="value">The value to return</param>
    /// <typeparam name="T">The type of this case</typeparam>
    public TypeSwitchValueExpression<TResult> Case<T>(TResult value) => Case(typeof(T), value);

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="type">The type of this case</param>
    /// <param name="value">The value to return</param>
    public TypeSwitchValueExpression<TResult> Case(Type type, TResult value)
    {
        Cases.Add(type, value);

        return this;
    }

    /// <summary>
    ///     Adds the specified default function if no other cases are hit
    /// </summary>
    /// <param name="value">The value to return if no other cases are hit</param>
    public TypeSwitchValueExpression<TResult> Default(TResult value)
    {
        DefaultSet = true;
        DefaultCase = value;

        return this;
    }

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <param name="type">The type used to select the case to execute</param>
    public TResult? Switch(Type type)
    {
        if (Cases.TryGetValue(type, out var @case))
            return @case;

        if (!DefaultSet)
            throw new InvalidOperationException("No case was matched");

        return DefaultCase;
    }

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <typeparam name="T">The type used to select the case to execute</typeparam>
    public TResult? Switch<T>() => Switch(typeof(T));
}