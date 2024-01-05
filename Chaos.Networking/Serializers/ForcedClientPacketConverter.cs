using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="ForceClientPacketArgs" /> into a buffer
/// </summary>
public sealed class ForcedClientPacketConverter : PacketConverterBase<ForceClientPacketArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ForceClientPacket;

    /// <inheritdoc />
    public override ForceClientPacketArgs Deserialize(ref SpanReader reader)
    {
        var length = reader.ReadUInt16();
        var opCode = reader.ReadByte();
        var data = reader.ReadBytes(length - 1);

        return new ForceClientPacketArgs
        {
            ClientOpCode = (ClientOpCode)opCode,
            Data = data
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ForceClientPacketArgs args)
    {
        writer.WriteUInt16((ushort)(args.Data.Length + 1));
        writer.WriteByte((byte)args.ClientOpCode);
        writer.WriteData(args.Data);
    }
}