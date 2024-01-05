using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="ConfirmExitArgs" /> into a buffer
/// </summary>
public sealed class ConfirmExitConverter : PacketConverterBase<ConfirmExitArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ConfirmExit;

    /// <inheritdoc />
    public override ConfirmExitArgs Deserialize(ref SpanReader reader)
    {
        var exitConfirmed = reader.ReadBoolean();

        //_ = reader.ReadBytes(2); LI: what does this do?

        return new ConfirmExitArgs
        {
            ExitConfirmed = exitConfirmed
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ConfirmExitArgs args)
    {
        writer.WriteBoolean(args.ExitConfirmed);
        writer.WriteBytes(new byte[2]); //LI: what does this do?
    }
}