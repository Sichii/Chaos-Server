// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Type" />.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Determines if a type is a flag enum.
    /// </summary>
    /// <typeparam name="T">The type to check</typeparam>
    /// <returns><c>true</c> if the provided type is an enum and has the <see cref="FlagsAttribute" /> attribute, otherwise <c>false</c></returns>
    public static bool IsFlagEnum<T>() where T: Enum => IsFlagEnum(typeof(T));

    /// <summary>
    ///     Determines if a type is a flag enum.
    /// </summary>
    /// <returns><c>true</c> if the provided type is an enum and has the <see cref="FlagsAttribute" /> attribute, otherwise <c>false</c></returns>
    public static bool IsFlagEnum(this Type type) => type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();

    /// <summary>
    ///     Determines if a type is a primitive type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPrimitive(this Type type) =>
        (type == typeof(string)) || (type == typeof(decimal)) || type is { IsValueType: true, IsPrimitive: true };
}