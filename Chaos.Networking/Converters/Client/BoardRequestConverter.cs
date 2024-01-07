using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="BoardRequestArgs" />
/// </summary>
public sealed class BoardRequestConverter : PacketConverterBase<BoardRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.BoardRequest;

    /// <inheritdoc />
    public override BoardRequestArgs Deserialize(ref SpanReader reader)
    {
        var boardRequestType = reader.ReadByte();

        var args = new BoardRequestArgs
        {
            BoardRequestType = (BoardRequestType)boardRequestType
        };

        switch (args.BoardRequestType)
        {
            case BoardRequestType.BoardList:
                break;
            case BoardRequestType.ViewBoard:
            {
                var boardId = reader.ReadUInt16();
                var startPostId = reader.ReadInt16();

                //var unknown = reader.ReadByte(); //idk but it's always 240
                args.BoardId = boardId;
                args.StartPostId = startPostId;

                break;
            }
            case BoardRequestType.ViewPost:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();
                var controls = (BoardControls)reader.ReadSByte();

                args.BoardId = boardId;
                args.PostId = postId;
                args.Controls = controls;

                break;
            }
            case BoardRequestType.NewPost:
            {
                var boardId = reader.ReadUInt16();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                args.BoardId = boardId;
                args.Subject = subject;
                args.Message = message;

                break;
            }
            case BoardRequestType.Delete:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();

                args.BoardId = boardId;
                args.PostId = postId;

                break;
            }
            case BoardRequestType.SendMail:
            {
                var boardId = reader.ReadUInt16();
                var to = reader.ReadString8();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                args.BoardId = boardId;
                args.To = to;
                args.Subject = subject;
                args.Message = message;

                break;
            }
            case BoardRequestType.Highlight:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();

                args.BoardId = boardId;
                args.PostId = postId;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, BoardRequestArgs args)
    {
        writer.WriteByte((byte)args.BoardRequestType);

        switch (args.BoardRequestType)
        {
            case BoardRequestType.BoardList:
            {
                break;
            }
            case BoardRequestType.ViewBoard:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteInt16(args.StartPostId!.Value);
                writer.WriteByte(240); //idk

                break;
            }
            case BoardRequestType.ViewPost:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteInt16(args.PostId!.Value);
                writer.WriteSByte((sbyte)args.Controls!.Value);

                break;
            }
            case BoardRequestType.NewPost:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteString8(args.Subject!);
                writer.WriteString16(args.Message!);

                break;
            }
            case BoardRequestType.Delete:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteInt16(args.PostId!.Value);

                break;
            }
            case BoardRequestType.SendMail:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteString8(args.To!);
                writer.WriteString8(args.Subject!);
                writer.WriteString16(args.Message!);

                break;
            }
            case BoardRequestType.Highlight:
            {
                writer.WriteUInt16(args.BoardId!.Value);
                writer.WriteInt16(args.PostId!.Value);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}