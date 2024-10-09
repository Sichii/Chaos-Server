using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="LocationArgs" />
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