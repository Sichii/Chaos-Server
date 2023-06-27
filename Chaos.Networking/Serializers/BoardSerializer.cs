using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="BoardArgs" /> into a buffer
/// </summary>
public sealed record BoardSerializer : ServerPacketSerializer<BoardArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Board;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, BoardArgs args)
    {
        writer.WriteByte((byte)args.Type);

        switch (args.Type)
        {
            case BoardOrResponseType.BoardList:
                writer.WriteUInt16((ushort)args.Boards!.Count);

                foreach (var board in args.Boards)
                {
                    writer.WriteUInt16(board.BoardId);
                    writer.WriteString8(board.Name);
                }

                break;
            case BoardOrResponseType.PublicBoard:
                writer.WriteBoolean(false);
                writer.WriteUInt16(args.Board!.BoardId);
                writer.WriteString8(args.Board.Name);
                writer.WriteInt16((short)args.Board.Posts.Count);

                foreach (var post in args.Board.Posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.CreationDate.Month);
                    writer.WriteByte((byte)post.CreationDate.Day);
                    writer.WriteString8(post.Subject);
                }

                break;
            case BoardOrResponseType.PublicPost:
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //dunno
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.CreationDate.Month);
                writer.WriteByte((byte)args.Post.CreationDate.Day);
                writer.WriteString8(args.Post.Subject);
                writer.WriteString16(args.Post.Message);

                break;
            case BoardOrResponseType.MailBoard:
                writer.WriteBoolean(false);
                writer.WriteUInt16(args.Board!.BoardId);
                writer.WriteString8(args.Board.Name);
                writer.WriteUInt16((ushort)args.Board.Posts.Count);

                foreach (var post in args.Board.Posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.CreationDate.Month);
                    writer.WriteByte((byte)post.CreationDate.Day);
                    writer.WriteString8(post.Subject);
                }

                break;
            case BoardOrResponseType.MailPost:
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //dunno
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.CreationDate.Month);
                writer.WriteByte((byte)args.Post.CreationDate.Day);
                writer.WriteString8(args.Post.Subject);
                writer.WriteString16(args.Post.Message);

                break;
            case BoardOrResponseType.SubmitPostResponse:
            case BoardOrResponseType.DeletePostResponse:
            case BoardOrResponseType.HighlightPostResponse:
                writer.WriteBoolean(args.Success!.Value);
                writer.WriteString8(args.ResponseMessage!);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}