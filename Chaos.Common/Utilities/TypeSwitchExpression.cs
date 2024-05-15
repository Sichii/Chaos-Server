using System.Collections.Frozen;

namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class that simulates switch-expression behavior on type objects
/// </summary>
/// <typeparam name="TResult">
///     The return type of the switch expression
/// </typeparam>
public class TypeSwitchExpression<TResult>
{
    /// <summary>
    ///     The default case if no other case is matched
    /// </summary>
    protected virtual Func<TResult?> DefaultCase { get; set; } = () => throw new InvalidOperationException("No case was matched");

    /// <summary>
    ///     The cases to switch on
    /// </summary>
    protected virtual IDictionary<Type, Func<TResult>> Cases { get; } = new Dictionary<Type, Func<TResult>>();

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="func">
    ///     The function to perform
    /// </param>
    /// <typeparam name="T">
    ///     The type of this case
    /// </typeparam>
    public TypeSwitchExpression<TResult> Case<T>(Func<TResult> func) => Case(typeof(T), func);

    /// <summary>
    ///     Adds the specified value to the switch on the specified type
    /// </summary>
    /// <param name="value">
    ///     The value to return
    /// </param>
    /// <typeparam name="T">
    ///     The type of this case
    /// </typeparam>
    public TypeSwitchExpression<TResult> Case<T>(TResult value) => Case(typeof(T), () => value);

    /// <summary>
    ///     Adds the specified value to the switch on the specified type
    /// </summary>
    /// <param name="value">
    ///     The value to return
    /// </param>
    /// <typeparam name="T">
    ///     The type of this case
    /// </typeparam>
    public TypeSwitchExpression<TResult> Case<T>(object value) => Case(typeof(T), () => (TResult)value);

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="type">
    ///     The type of this case
    /// </param>
    /// <param name="func">
    ///     The function to perform
    /// </param>
    public TypeSwitchExpression<TResult> Case(Type type, Func<TResult> func)
    {
        Cases.Add(type, func);

        return this;
    }

    /// <summary>
    ///     Adds the specified default function if no other cases are hit
    /// </summary>
    /// <param name="func">
    ///     The function to perform if no other cases are hit
    /// </param>
    public TypeSwitchExpression<TResult> Default(Func<TResult> func)
    {
        DefaultCase = func;

        return this;
    }

    /// <summary>
    ///     Adds the specified default value if no other cases are hit
    /// </summary>
    /// <param name="value">
    ///     The value to return if no other cases are hit
    /// </param>
    public TypeSwitchExpression<TResult> Default(TResult value)
    {
        DefaultCase = () => value;

        return this;
    }

    /// <summary>
    ///     Adds the specified default value if no other cases are hit
    /// </summary>
    /// <param name="value">
    ///     The value to return if no other cases are hit
    /// </param>
    public TypeSwitchExpression<TResult> Default(object value)
    {
        DefaultCase = () => (TResult)value;

        return this;
    }

    /// <summary>
    ///     Freezes the cases so that no more can be added
    /// </summary>
    /// <returns>
    ///     A frozen, optimized version of the type switch that is meant to be reused
    /// </returns>
    public TypeSwitchExpression<TResult> Freeze() => new FrozenTypeSwitchExpression<TResult>(this);

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <param name="type">
    ///     The type used to select the case to execute
    /// </param>
    public TResult? Switch(Type type)
    {
        if (Cases.TryGetValue(type, out var @case))
            return @case();

        return DefaultCase();
    }

    /// <summary>
    ///     Executes the function associated with the specified type
    /// </summary>
    /// <typeparam name="T">
    ///     The type used to select the case to execute
    /// </typeparam>
    public TResult? Switch<T>() => Switch(typeof(T));

    /// <summary>
    ///     A helper class that simulates switch-expression behavior on type objects. The cases are frozen and this object is
    ///     meant to be reused.
    /// </summary>
    /// <typeparam name="T">
    ///     The return type of the switch expression
    /// </typeparam>
    private sealed class FrozenTypeSwitchExpression<T>(TypeSwitchExpression<T> tse) : TypeSwitchExpression<T>
    {
        /// <inheritdoc />
        protected override Func<T?> DefaultCase { get; set; } = tse.DefaultCase;

        /// <inheritdoc />
        protected override FrozenDictionary<Type, Func<T>> Cases { get; } = tse.Cases.ToFrozenDictionary();
    }
}