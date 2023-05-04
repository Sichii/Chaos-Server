using Chaos.Networking.Abstractions;

namespace Chaos.Services.Other;

public sealed class WorldClientRegistry : ClientRegistry<IWorldClient>
{
    /// <inheritdoc />
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public override IEnumerator<IWorldClient> GetEnumerator() => Clients.Values.Where(c => c.Aisling != null).GetEnumerator();
}