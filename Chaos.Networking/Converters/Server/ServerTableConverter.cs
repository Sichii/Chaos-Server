using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="ServerTableArgs" /> into a buffer
/// </summary>
public sealed class ServerTableConverter : PacketConverterBase<ServerTableArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ServerTable;

    /// <inheritdoc />
    public override ServerTableArgs Deserialize(ref SpanReader reader)
    {
        var serverTable = reader.ReadData16();

        return new ServerTableArgs
        {
            ServerTable = serverTable
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ServerTableArgs args) => writer.WriteData16(args.ServerTable);
}