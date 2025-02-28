#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
#endregion

namespace Chaos.Models.Data;

public sealed record ChannelSettings
{
    public string ChannelName { get; set; }
    public bool CustomChannel { get; set; }
    public MessageColor? MessageColor { get; set; }

    public ChannelSettings(string channelName, bool customChannel = false, MessageColor? messageColor = null)
    {
        ChannelName = channelName;
        MessageColor = messageColor;
        CustomChannel = customChannel;
    }

    /// <inheritdoc />
    public bool Equals(ChannelSettings? other) => other is not null && ChannelName.EqualsI(other.ChannelName);

    public void Deconstruct(out string channelName, out bool customChannel, out MessageColor? messageColor)
    {
        channelName = ChannelName;
        customChannel = CustomChannel;
        messageColor = MessageColor;
    }

    /// <inheritdoc />

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => ChannelName.GetHashCode(StringComparison.OrdinalIgnoreCase);
}