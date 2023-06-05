using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

/// <summary>
///     A class that does nothing when disposed
/// </summary>
[ExcludeFromCodeCoverage(Justification = "No logic by design")]
public sealed class NoOpDisposable : IPolyDisposable
{
    /// <inheritdoc />
    public void Dispose() { }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => default;
}