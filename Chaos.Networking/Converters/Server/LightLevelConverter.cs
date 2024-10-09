using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="LightLevelArgs" />
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