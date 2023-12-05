using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Options;

/// <inheritdoc cref="ILoginServerInfo" />
public record LoginServerInfo : ConnectionInfo, ILoginServerInfo
{
    /// <inheritdoc />
    public string Description { get; set; } = null!;

    /// <inheritdoc />
    public byte Id { get; set; } = 0;

    /// <inheritdoc />
    public string Name { get; set; } = null!;
}