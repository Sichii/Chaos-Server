using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

public sealed class NoOpDisposable : IPolyDisposable
{
    /// <inheritdoc />
    public void Dispose() { }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => default;
}