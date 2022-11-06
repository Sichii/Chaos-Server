using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

public class NoOpDisposable : IPolyDisposable
{
    /// <inheritdoc />
    public void Dispose() { }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => default;
}