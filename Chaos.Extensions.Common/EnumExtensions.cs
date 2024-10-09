namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Enum" />
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Gets the names of the enum values of the specified type
    /// </summary>
    public static IEnumerable<string> GetEnumNames<T>()
    {
        var type = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType is not null)
            type = underlyingType;

        if (!type.IsEnum)
            throw new InvalidOperationException($"{type.Name} is not an enum");

        if (underlyingType is not null)
            return Enum.GetNames(type)
                       .Prepend(string.Empty);

        return Enum.GetNames(type);
    }

    /// <summary>
    ///     Gets the individual flag parts of a flag enum value />
    /// </summary>
    /// <param name="input">
    ///     An enum value with one or more flags
    /// </param>
    /// <typeparam name="T">
    ///     An enum type
    /// </typeparam>
    public static IEnumerable<T> GetFlags<T>(this T input) where T: Enum
    {
        foreach (T value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }
}