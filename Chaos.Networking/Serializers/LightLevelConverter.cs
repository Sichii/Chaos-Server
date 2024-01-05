using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="LightLevelArgs" /> into a buffer
/// </summary>
public sealed class LightLevelConverter : PacketConverterBase<LightLevelArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.LightLevel;

    /// <inheritdoc />
    public override LightLevelArgs Deserialize(ref SpanReader reader)
    {
        var lightLevel = reader.ReadByte();

        return new LightLevelArgs
        {
            LightLevel = (LightLevel)lightLevel
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LightLevelArgs args) => writer.WriteByte((byte)args.LightLevel);
}