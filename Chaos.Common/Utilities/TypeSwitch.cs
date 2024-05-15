using System.Collections.Frozen;

namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class that simulates switch-case behavior on type objects
/// </summary>
public class TypeSwitch
{
    /// <summary>
    ///     The default case if no other case is matched
    /// </summary>
    protected virtual Action DefaultCase { get; set; } = () => throw new InvalidOperationException("No case was matched");

    /// <summary>
    ///     The cases to switch on
    /// </summary>
    protected virtual IDictionary<Type, Action> Cases { get; } = new Dictionary<Type, Action>();

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="action">
    ///     The action to perform
    /// </param>
    /// <typeparam name="T">
    ///     The type of this case
    /// </typeparam>
    public TypeSwitch Case<T>(Action action) => Case(typeof(T), action);

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="type">
    ///     The type of this case
    /// </param>
    /// <param name="action">
    ///     The action to perform
    /// </param>
    public TypeSwitch Case(Type type, Action action)
    {
        Cases.Add(type, action);

        return this;
    }

    /// <summary>
    ///     Adds the specified default action if no other cases are hit
    /// </summary>
    /// <param name="action">
    ///     The action to perform if no other cases are hit
    /// </param>
    public TypeSwitch Default(Action action)
    {
        DefaultCase = action;

        return this;
    }

    /// <summary>
    ///     Executes the action associated with the specified type
    /// </summary>
    /// <param name="type">
    ///     The type used to select the case to execute
    /// </param>
    public void Switch(Type type)
    {
        if (Cases.TryGetValue(type, out var @case))
            @case();
        else
            DefaultCase();
    }

    /// <summary>
    ///     Executes the action associated with the specified type
    /// </summary>
    /// <typeparam name="T">
    ///     The type used to select the case to execute
    /// </typeparam>
    public void Switch<T>() => Switch(typeof(T));

    /// <summary>
    ///     A helper class that simulates switch-case behavior on type objects. The cases are frozen and this object is meant
    ///     to be reused.
    /// </summary>
    private class FrozenTypeSwitch(TypeSwitch ts) : TypeSwitch
    {
        /// <inheritdoc />
        protected override Action DefaultCase { get; set; } = ts.DefaultCase;

        /// <inheritdoc />
        protected override FrozenDictionary<Type, Action> Cases { get; } = ts.Cases.ToFrozenDictionary();
    }
}