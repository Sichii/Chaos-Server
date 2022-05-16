using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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