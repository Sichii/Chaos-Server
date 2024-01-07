using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="LocationArgs" /> into a buffer
/// </summary>
public sealed class LocationConverter : PacketConverterBase<LocationArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Location;

    /// <inheritdoc />
    public override LocationArgs Deserialize(ref SpanReader reader)
    {
        var point = reader.ReadPoint16();

        return new LocationArgs
        {
            X = point.X,
            Y = point.Y
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LocationArgs args) => writer.WritePoint16((ushort)args.X, (ushort)args.Y);
}