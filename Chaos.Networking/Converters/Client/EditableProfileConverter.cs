using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="EditableProfileArgs" />
/// </summary>
public sealed class EditableProfileConverter : PacketConverterBase<EditableProfileArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.EditableProfile;

    /// <inheritdoc />
    public override EditableProfileArgs Deserialize(ref SpanReader reader)
    {
        // ReSharper disable once UnusedVariable
        var totalLength = reader.ReadUInt16();

        if (totalLength == 0)
            return new EditableProfileArgs
            {
                PortraitData = [],
                ProfileMessage = string.Empty
            };

        var portraitData = reader.ReadData16();
        var profileMessage = reader.ReadString16();

        return new EditableProfileArgs
        {
            PortraitData = portraitData,
            ProfileMessage = profileMessage
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, EditableProfileArgs args)
    {
        writer.WriteUInt16((ushort)(4 + args.PortraitData.Length + args.ProfileMessage.Length));
        writer.WriteData16(args.PortraitData);
        writer.WriteString16(args.ProfileMessage);
    }
}