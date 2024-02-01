using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="DialogInteractionArgs" />
/// </summary>
public sealed class DialogInteractionConverter : PacketConverterBase<DialogInteractionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.DialogInteraction;

    /// <inheritdoc />
    public override DialogInteractionArgs Deserialize(ref SpanReader reader)
    {
        var entityType = reader.ReadByte();
        var entityId = reader.ReadUInt32();
        var pursuitId = reader.ReadUInt16();
        var dialogId = reader.ReadUInt16();

        var args = new DialogInteractionArgs
        {
            EntityType = (EntityType)entityType,
            EntityId = entityId,
            PursuitId = pursuitId,
            DialogId = dialogId
        };

        if (!reader.EndOfSpan)
        {
            var dialogArgsType = reader.ReadByte();

            args.DialogArgsType = (DialogArgsType)dialogArgsType;

            switch (args.DialogArgsType)
            {
                case DialogArgsType.MenuResponse:
                {
                    var option = reader.ReadByte();

                    args.Option = option;

                    break;
                }
                case DialogArgsType.TextResponse:
                {
                    var dialogArgs = reader.ReadArgs8();

                    if (dialogArgs.Count != 0)
                        args.Args = dialogArgs;

                    break;
                }
                case DialogArgsType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DialogInteractionArgs args)
    {
        writer.WriteByte((byte)args.EntityType);
        writer.WriteUInt32(args.EntityId);
        writer.WriteUInt16(args.PursuitId);
        writer.WriteUInt16(args.DialogId);
        writer.WriteByte((byte)args.DialogArgsType);

        switch (args.DialogArgsType)
        {
            case DialogArgsType.MenuResponse:
                writer.WriteByte(args.Option!.Value);

                break;
            case DialogArgsType.TextResponse:
                if (!args.Args.IsNullOrEmpty())
                    foreach (var arg in args.Args.TakeLast(2))
                        writer.WriteString8(arg);

                break;
            case DialogArgsType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}