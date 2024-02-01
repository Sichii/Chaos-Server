using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="ExitResponseArgs" />
/// </summary>
public sealed class ExitResponseConverter : PacketConverterBase<ExitResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ExitResponse;

    /// <inheritdoc />
    public override ExitResponseArgs Deserialize(ref SpanReader reader)
    {
        var exitConfirmed = reader.ReadBoolean();

        //_ = reader.ReadBytes(2); LI: what does this do?

        return new ExitResponseArgs
        {
            ExitConfirmed = exitConfirmed
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ExitResponseArgs args)
    {
        writer.WriteBoolean(args.ExitConfirmed);
        writer.WriteBytes(new byte[2]); //LI: what does this do?
    }
}