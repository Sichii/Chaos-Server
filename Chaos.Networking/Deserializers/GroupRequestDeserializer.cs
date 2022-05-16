using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record GroupRequestDeserializer : ClientPacketDeserializer<GroupRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.GroupRequest;

    public override GroupRequestArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = (GroupRequestType)reader.ReadByte();

        if (groupRequestType == GroupRequestType.Groupbox)
        {
            var leader = reader.ReadString8();
            var test = reader.ReadString8();
            reader.ReadByte(); //unknown
            var minLevel = reader.ReadByte();
            var maxLevel = reader.ReadByte();
            var maxOfClass = new byte[6];

            maxOfClass[(byte)BaseClass.Warrior] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Wizard] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Rogue] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Priest] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Monk] = reader.ReadByte();
        }

        var targetName = reader.ReadString8();

        return new GroupRequestArgs(groupRequestType, targetName);
    }
}