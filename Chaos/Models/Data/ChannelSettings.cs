using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;

namespace Chaos.Models.Data;

public sealed record ChannelSettings(string ChannelName, MessageColor? MessageColor = null)
{
    public string ChannelName { get; set; } = ChannelName;
    public MessageColor? MessageColor { get; set; } = MessageColor;

    /// <inheritdoc />
    public bool Equals(ChannelSettings? other) => other is not null && ChannelName.EqualsI(other.ChannelName);

    /// <inheritdoc />

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => ChannelName.GetHashCode(StringComparison.OrdinalIgnoreCase);
}