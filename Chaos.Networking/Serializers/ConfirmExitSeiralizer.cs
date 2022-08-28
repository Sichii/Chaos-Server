using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record ConfirmExitSerializer : ServerPacketSerializer<ConfirmExitArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ConfirmExit;

    public override void Serialize(ref SpanWriter writer, ConfirmExitArgs args)
    {
        writer.WriteBoolean(args.ExitConfirmed);
        writer.WriteBytes(new byte[2]);
    }
}