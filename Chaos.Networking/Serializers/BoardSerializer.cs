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
            {
                writer.WriteUInt16((ushort)args.Boards!.Count);

                foreach (var board in args.Boards)
                {
                    writer.WriteUInt16(board.BoardId);
                    writer.WriteString8(board.Name);
                }

                break;
            }
            case BoardOrResponseType.PublicBoard:
            {
                writer.WriteBoolean(false);
                writer.WriteUInt16(args.Board!.BoardId);
                writer.WriteString8(args.Board.Name);

                //order posts newest to oldest
                var orderedPosts = (IEnumerable<PostInfo>)args.Board.Posts.OrderByDescending(p => p.CreationDate);

                //if there's a StartPostId, only send posts with an id greater than or equal to it
                if (args.StartPostId.HasValue)
                    orderedPosts = orderedPosts.Where(p => p.PostId <= args.StartPostId.Value);

                //only send up to 127 posts (i have no fucking clue why its sbyte.MaxValue)
                var posts = orderedPosts.Take(sbyte.MaxValue).ToList();

                writer.WriteSByte((sbyte)posts.Count);

                foreach (var post in posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.CreationDate.Month);
                    writer.WriteByte((byte)post.CreationDate.Day);
                    writer.WriteString8(post.Subject);
                }

                break;
            }
            case BoardOrResponseType.PublicPost:
            {
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //dunno
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.CreationDate.Month);
                writer.WriteByte((byte)args.Post.CreationDate.Day);
                writer.WriteString8(args.Post.Subject);
                writer.WriteString16(args.Post.Message);

                break;
            }
            case BoardOrResponseType.MailBoard:
            {
                writer.WriteBoolean(false);
                writer.WriteUInt16(args.Board!.BoardId);
                writer.WriteString8(args.Board.Name);

                //order posts newest to oldest
                var orderedPosts = (IEnumerable<PostInfo>)args.Board.Posts.OrderByDescending(p => p.PostId);

                //if there's a StartPostId, only send posts with an id greater than or equal to it
                if (args.StartPostId.HasValue)
                    orderedPosts = orderedPosts.Where(p => p.PostId <= args.StartPostId.Value);

                //only send up to 127 posts (i have no fucking clue why its sbyte.MaxValue)
                var posts = orderedPosts.Take(sbyte.MaxValue).ToList();

                writer.WriteSByte((sbyte)posts.Count);

                foreach (var post in posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.CreationDate.Month);
                    writer.WriteByte((byte)post.CreationDate.Day);
                    writer.WriteString8(post.Subject);
                }

                break;
            }
            case BoardOrResponseType.MailPost:
            {
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //dunno
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.CreationDate.Month);
                writer.WriteByte((byte)args.Post.CreationDate.Day);
                writer.WriteString8(args.Post.Subject);
                writer.WriteString16(args.Post.Message);

                break;
            }
            case BoardOrResponseType.SubmitPostResponse:
            case BoardOrResponseType.DeletePostResponse:
            case BoardOrResponseType.HighlightPostResponse:
            {
                writer.WriteBoolean(args.Success!.Value);
                writer.WriteString8(args.ResponseMessage!);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}