using Chaos.Core.Utilities;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Serializers;

public abstract record ServerPacketSerializer<T> : IServerPacketSerializer<T> where T: ISendArgs
{
    public abstract ServerOpCode ServerOpCode { get; }

    public void Serialize(ref SpanWriter writer, object args)
    {
        if (args is T tArgs)
            Serialize(ref writer, tArgs);
    }

    public abstract void Serialize(ref SpanWriter writer, T args);
}