using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SpellUseArgs" />
/// </summary>
public sealed class SpellUseConverter : PacketConverterBase<SpellUseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SpellUse;

    /// <inheritdoc />
    public override SpellUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var argsData = reader.ReadData();

        return new SpellUseArgs
        {
            SourceSlot = sourceSlot,
            ArgsData = argsData
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SpellUseArgs args)
    {
        writer.WriteByte(args.SourceSlot);
        writer.WriteData(args.ArgsData);
    }
}