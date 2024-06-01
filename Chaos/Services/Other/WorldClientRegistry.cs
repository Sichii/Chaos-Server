using Chaos.Networking.Abstractions;

namespace Chaos.Services.Other;

public sealed class WorldClientRegistry : ClientRegistry<IChaosWorldClient>
{
    /// <inheritdoc />

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public override IEnumerator<IChaosWorldClient> GetEnumerator()
        => Clients.Values
                  .Where(c => c.Aisling != null)
                  .GetEnumerator();
}