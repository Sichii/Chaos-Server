using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="ForceClientPacketArgs" /> into a buffer
/// </summary>
public sealed record ForcedClientPacketSerializer : ServerPacketSerializer<ForceClientPacketArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.ForceClientPacket;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ForceClientPacketArgs args)
    {
        writer.WriteUInt16((ushort)(args.Data.Length + 1));
        writer.WriteByte((byte)args.ClientOpCode);
        writer.WriteData(args.Data);
    }
}