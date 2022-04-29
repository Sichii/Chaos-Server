using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record CooldownSerializer : ServerPacketSerializer<CooldownArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Cooldown;

    public override void Serialize(ref SpanWriter writer, CooldownArgs args)
    {
        writer.WriteBoolean(args.IsSkill);
        writer.WriteByte(args.Slot);
        writer.WriteUInt32(args.CooldownSecs);
    }
}