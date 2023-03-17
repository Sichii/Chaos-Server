using Chaos.Common.Definitions;
using Chaos.Extensions.Common;

namespace Chaos.Data;

public sealed record ChannelSettings(string ChannelName, MessageColor? MessageColor = null)
{
    public MessageColor? MessageColor { get; set; } = MessageColor;

    /// <inheritdoc />
    public bool Equals(ChannelSettings? other) => other is not null && ChannelName.EqualsI(other.ChannelName);

    /// <inheritdoc />
    public override int GetHashCode() => ChannelName.GetHashCode(StringComparison.OrdinalIgnoreCase);
}