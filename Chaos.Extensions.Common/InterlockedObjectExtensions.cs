using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="IInterlockedObject{T}" />.
/// </summary>
public static class InterlockedObjectExtensions
{
    /// <summary>
    ///     Tries to get the underlying objects and convert it to the specified type.
    /// </summary>
    /// <param name="syncObj"></param>
    /// <typeparam name="T">The type to convert the returned object to</typeparam>
    /// <returns>An object of the specified type if an object exists and is of that type, otherwise <c>null</c></returns>
    [ExcludeFromCodeCoverage(Justification = "Nothing to test, just a shorthand")]
    public static T? TryGet<T>(this IInterlockedObject<object> syncObj) where T: class => syncObj.Get() as T;
}