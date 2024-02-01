using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="IgnoreArgs" />
/// </summary>
public sealed class IgnoreConverter : PacketConverterBase<IgnoreArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Ignore;

    /// <inheritdoc />
    public override IgnoreArgs Deserialize(ref SpanReader reader)
    {
        var ignoreType = reader.ReadByte();

        var args = new IgnoreArgs
        {
            IgnoreType = (IgnoreType)ignoreType
        };

        if (args.IgnoreType != IgnoreType.Request)
        {
            var targetName = reader.ReadString8();

            args.TargetName = targetName;
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, IgnoreArgs args)
    {
        writer.WriteByte((byte)args.IgnoreType);

        if (args.IgnoreType != IgnoreType.Request)
            writer.WriteString8(args.TargetName!);
    }
}