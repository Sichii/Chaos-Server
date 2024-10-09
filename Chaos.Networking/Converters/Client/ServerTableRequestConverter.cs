using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ServerTableRequestArgs" />
/// </summary>
public sealed class ServerTableRequestConverter : PacketConverterBase<ServerTableRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ServerTableRequest;

    /// <inheritdoc />
    public override ServerTableRequestArgs Deserialize(ref SpanReader reader)
    {
        var serverTableRequestType = reader.ReadByte();

        var args = new ServerTableRequestArgs
        {
            ServerTableRequestType = (ServerTableRequestType)serverTableRequestType
        };

        if (args.ServerTableRequestType == ServerTableRequestType.ServerId)
        {
            var serverId = reader.ReadByte();

            args.ServerId = serverId;
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ServerTableRequestArgs args)
    {
        writer.WriteByte((byte)args.ServerTableRequestType);

        if (args.ServerTableRequestType == ServerTableRequestType.ServerId)
            writer.WriteByte(args.ServerId!.Value);
    }
}