namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class that simulates switch-case behavior on type objects
/// </summary>
public sealed class TypeSwitch
{
    private readonly Dictionary<Type, Action> Cases = new();
    private Action DefaultCase = () => throw new InvalidOperationException("No case was matched");

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="action">The action to perform</param>
    /// <typeparam name="T">The type of this case</typeparam>
    public TypeSwitch Case<T>(Action action) => Case(typeof(T), action);

    /// <summary>
    ///     Adds the specified action to the switch on the specified type
    /// </summary>
    /// <param name="type">The type of this case</param>
    /// <param name="action">The action to perform</param>
    public TypeSwitch Case(Type type, Action action)
    {
        Cases.Add(type, action);

        return this;
    }

    /// <summary>
    ///     Adds the specified default action if no other cases are hit
    /// </summary>
    /// <param name="action">The action to perform if no other cases are hit</param>
    public TypeSwitch Default(Action action)
    {
        DefaultCase = action;

        return this;
    }

    /// <summary>
    ///     Executes the action associated with the specified type
    /// </summary>
    /// <param name="type">The type used to select the case to execute</param>
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
    /// <typeparam name="T">The type used to select the case to execute</typeparam>
    public void Switch<T>() => Switch(typeof(T));
}