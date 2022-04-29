namespace Chaos.Core.Utilities;

public static class ClientId
{
    private static uint CurrentId;
    public static uint NextId => Interlocked.Increment(ref CurrentId);
}