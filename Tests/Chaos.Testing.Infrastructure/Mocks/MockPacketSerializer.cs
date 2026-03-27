#region
using System.Buffers;
using System.Text;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockPacketSerializer : IPacketSerializer
{
    public int DeserializeCallCount { get; private set; }

    /// <summary>
    ///     If set, Deserialize will return this value (cast to T) instead of default.
    /// </summary>
    public object? DeserializeResult { get; set; }

    public IPacketSerializable? LastSerialized { get; private set; }
    public int SerializeCallCount { get; private set; }
    public Encoding Encoding { get; } = Encoding.ASCII;

    public T Deserialize<T>(in Packet packet) where T: IPacketSerializable
    {
        DeserializeCallCount++;

        if (DeserializeResult is T result)
            return result;

        return default!;
    }

    public Packet Serialize(IPacketSerializable obj)
    {
        LastSerialized = obj;
        SerializeCallCount++;

        var owner = MemoryPool<byte>.Shared.Rent(16);

        return new Packet(0x01, owner, 1);
    }

    public static MockPacketSerializer Create() => new();
}