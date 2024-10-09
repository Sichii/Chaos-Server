using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SocialStatusArgs" />
/// </summary>
public sealed class SocialStatusConverter : PacketConverterBase<SocialStatusArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SocialStatus;

    /// <inheritdoc />
    public override SocialStatusArgs Deserialize(ref SpanReader reader)
    {
        var socialStatus = reader.ReadByte();

        return new SocialStatusArgs
        {
            SocialStatus = (SocialStatus)socialStatus
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SocialStatusArgs args) => writer.WriteByte((byte)args.SocialStatus);
}