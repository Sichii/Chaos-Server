// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.Common;

public static class EnumExtensions
{
    public static bool IsFlagEnum<T>() where T: Enum => IsFlagEnum(typeof(T));

    public static bool IsFlagEnum(this Type type) => type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();
}