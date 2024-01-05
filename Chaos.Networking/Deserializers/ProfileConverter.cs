using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ProfileArgs" />
/// </summary>
public sealed class ProfileConverter : PacketConverterBase<ProfileArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Profile;

    /// <inheritdoc />
    public override ProfileArgs Deserialize(ref SpanReader reader)
    {
        // ReSharper disable once UnusedVariable
        var totalLength = reader.ReadUInt16();

        if (totalLength == 0)
            return new ProfileArgs
            {
                PortraitData = Array.Empty<byte>(),
                ProfileMessage = string.Empty
            };

        var portraitData = reader.ReadData16();
        var profileMessage = reader.ReadString16();

        return new ProfileArgs
        {
            PortraitData = portraitData,
            ProfileMessage = profileMessage
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ProfileArgs args)
    {
        writer.WriteUInt16((ushort)(4 + args.PortraitData.Length + args.ProfileMessage.Length));
        writer.WriteData16(args.PortraitData);
        writer.WriteString16(args.ProfileMessage);
    }
}