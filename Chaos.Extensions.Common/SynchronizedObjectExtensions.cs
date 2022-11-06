using Chaos.Common.Abstractions;

namespace Chaos.Extensions.Common;

public static class SynchronizedObjectExtensions
{
    public static T? TryGet<T>(this IInterlockedObject<object> syncObj) where T: class => syncObj.Get() as T;
}