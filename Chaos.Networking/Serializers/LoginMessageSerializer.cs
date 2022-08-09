using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record LoginMessageSerializer : ServerPacketSerializer<LoginMessageArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.LoginMessage;

    public override void Serialize(ref SpanWriter writer, LoginMessageArgs args)
    {
        writer.WriteByte((byte)args.LoginMessageType);
        writer.WriteString8(args.LoginMessageType == LoginMessageType.Confirm ? "\0" : args.Message!);
    }
}