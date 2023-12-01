namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class that simulates switch-expression behavior on type objects
/// </summary>
/// <typeparam name="TResult">The return type of the switch expression</typeparam>
public sealed class TypeSwitchExpression<TResult>
{
    private readonly Dictionary<Type, Func<TResult>> Cases = new();
    private Func<TResult?> DefaultCase = () => throw new InvalidOperationException("No case was matched");

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="func">The function to perform</param>
    /// <typeparam name="T">The type of this case</typeparam>
    public TypeSwitchExpression<TResult> Case<T>(Func<TResult> func) => Case(typeof(T), func);

    /// <summary>
    ///     Adds the specified value to the switch on the specified type
    /// </summary>
    /// <param name="value">The value to return</param>
    /// <typeparam name="T">The type of this case</typeparam>
    public TypeSwitchExpression<TResult> Case<T>(TResult value) => Case(typeof(T), () => value);

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="type">The type of this case</param>
    /// <param name="func">The function to perform</param>
    public TypeSwitchExpression<TResult> Case(Type type, Func<TResult> func)
    {
        Cases.Add(type, func);

        return this;
    }

    /// <summary>
    ///     Adds the specified default function if no other cases are hit
    /// </summary>
    /// <param name="func">The function to perform if no other cases are hit</param>
    public TypeSwitchExpression<TResult> Default(Func<TResult> func)
    {
        DefaultCase = func;

        return this;
    }

    /// <summary>
    ///     Adds the specified default value if no other cases are hit
    /// </summary>
    /// <param name="value">The value to return if no other cases are hit</param>
    public TypeSwitchExpression<TResult> Default(TResult value)
    {
        DefaultCase = () => value;

        return this;
    }

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <param name="type">The type used to select the case to execute</param>
    public TResult? Switch(Type type)
    {
        if (Cases.TryGetValue(type, out var @case))
            return @case();

        return DefaultCase();
    }

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <typeparam name="T">The type used to select the case to execute</typeparam>
    public TResult? Switch<T>() => Switch(typeof(T));
}