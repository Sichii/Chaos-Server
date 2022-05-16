namespace Chaos.Core.Identity;

public static class ClientId
{
    private static uint CurrentId;
    public static uint NextId => Interlocked.Increment(ref CurrentId);
}