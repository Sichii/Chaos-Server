using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record DialogResponseDeserializer : ClientPacketDeserializer<DialogResponseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.DialogResponse;

    public override DialogResponseArgs Deserialize(ref SpanReader reader)
    {
        var worldObjectType = (GameObjectType)reader.ReadByte();
        var objectId = reader.ReadUInt32();
        var pursuitId = reader.ReadUInt16();
        var dialogId = reader.ReadUInt16();
        var dialogArgsType = default(DialogArgsType?);
        var option = default(byte?);
        var args = default(string[]?);

        if (!reader.EndOfSpan)
        {
            dialogArgsType = (DialogArgsType)reader.ReadByte();

            if (dialogArgsType == DialogArgsType.MenuResponse)
                option = reader.ReadByte();
            else if (dialogArgsType == DialogArgsType.TextResponse)
            {
                args = reader.ReadArgs8().ToArray();

                if (args.Length == 0)
                    args = null;
            }
        }

        return new DialogResponseArgs(
            worldObjectType,
            objectId,
            pursuitId,
            dialogId,
            dialogArgsType,
            option,
            args);
    }
}