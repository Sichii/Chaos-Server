using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extensions methods for <see cref="Chaos.Common.Definitions.BaseClass" />
/// </summary>
public static class BaseClassExtensions
{
    /// <summary>
    ///     Determines whether or not a BaseClass contains another BaseClass
    /// </summary>
    /// <param name="baseClass">The encompassing baseClass</param>
    /// <param name="other">The other baseClass</param>
    /// <returns><c>true</c> if <paramref name="baseClass" /> contains <paramref name="other" />, otherwise <c>false</c></returns>
    public static bool ContainsClass(this BaseClass baseClass, BaseClass other) => baseClass switch
    {
        BaseClass.Any => true,
        _             => baseClass == other
    };

    /// <summary>
    ///     Determines whether or not BaseClass is another BaseClass
    /// </summary>
    /// <param name="baseclass">The specific baseclass</param>
    /// <param name="other">The other baseClass</param>
    /// <returns><c>true</c> if <paramref name="baseclass" /> IS <paramref name="other" />, otherwise <c>false</c></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     baseClass can not be <see cref="Chaos.Common.Definitions.BaseClass.Any">BaseClass.Any</see>
    /// </exception>
    public static bool IsClass(this BaseClass baseclass, BaseClass other) => baseclass switch
    {
        //Any "is" not a class
        BaseClass.Any => throw new ArgumentOutOfRangeException(nameof(baseclass), baseclass, null),
        //Diacht "is" all classes
        BaseClass.Diacht => true,
        _                => (baseclass == other) || (other == BaseClass.Any)
    };
}