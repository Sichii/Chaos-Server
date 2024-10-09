using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayBoardArgs" />
/// </summary>
public sealed class DisplayBoardConverter : PacketConverterBase<DisplayBoardArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayBoard;

    /// <inheritdoc />
    public override DisplayBoardArgs Deserialize(ref SpanReader reader)
    {
        var type = reader.ReadByte();

        var args = new DisplayBoardArgs
        {
            Type = (BoardOrResponseType)type
        };

        switch (args.Type)
        {
            case BoardOrResponseType.BoardList:
            {
                var count = reader.ReadUInt16();
                args.Boards = new List<BoardInfo>(count);

                for (var i = 0; i < count; i++)
                {
                    var boardId = reader.ReadUInt16();
                    var boardName = reader.ReadString8();

                    args.Boards.Add(
                        new BoardInfo
                        {
                            BoardId = boardId,
                            Name = boardName
                        });
                }

                break;
            }
            case BoardOrResponseType.PublicBoard:
            {
                _ = reader.ReadBoolean();
                var boardId = reader.ReadUInt16();
                var boardName = reader.ReadString8();

                args.Board = new BoardInfo
                {
                    BoardId = boardId,
                    Name = boardName
                };

                break;
            }
            case BoardOrResponseType.PublicPost:
            {
                var enablePrevBtn = reader.ReadBoolean();
                _ = reader.ReadByte(); //LI: what is this for?
                var postId = reader.ReadInt16();
                var author = reader.ReadString8();
                var month = reader.ReadByte();
                var day = reader.ReadByte();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                args.EnablePrevBtn = enablePrevBtn;

                args.Post = new PostInfo
                {
                    PostId = postId,
                    Author = author,
                    MonthOfYear = month,
                    DayOfMonth = day,
                    Subject = subject,
                    Message = message
                };

                break;
            }
            case BoardOrResponseType.MailBoard:
            {
                _ = reader.ReadBoolean();
                var boardId = reader.ReadUInt16();
                var boardName = reader.ReadString8();
                var postCount = reader.ReadSByte();
                var posts = new List<PostInfo>(postCount);

                args.Board = new BoardInfo
                {
                    BoardId = boardId,
                    Name = boardName
                };

                for (var i = 0; i < postCount; i++)
                {
                    var isHighlighted = reader.ReadBoolean();
                    var postId = reader.ReadInt16();
                    var author = reader.ReadString8();
                    var month = reader.ReadByte();
                    var day = reader.ReadByte();
                    var subject = reader.ReadString8();

                    posts.Add(
                        new PostInfo
                        {
                            IsHighlighted = isHighlighted,
                            PostId = postId,
                            Author = author,
                            MonthOfYear = month,
                            DayOfMonth = day,
                            Subject = subject
                        });
                }

                args.Board.Posts = posts;

                break;
            }
            case BoardOrResponseType.MailPost:
            {
                args.EnablePrevBtn = reader.ReadBoolean();
                _ = reader.ReadByte(); //LI: what is this for?
                var postId = reader.ReadInt16();
                var author = reader.ReadString8();
                var month = reader.ReadByte();
                var day = reader.ReadByte();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                args.Post = new PostInfo
                {
                    PostId = postId,
                    Author = author,
                    MonthOfYear = month,
                    DayOfMonth = day,
                    Subject = subject,
                    Message = message
                };

                break;
            }
            case BoardOrResponseType.SubmitPostResponse:
            case BoardOrResponseType.DeletePostResponse:
            case BoardOrResponseType.HighlightPostResponse:
            {
                var success = reader.ReadBoolean();
                var responseMessage = reader.ReadString8();

                args.Success = success;
                args.ResponseMessage = responseMessage;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayBoardArgs args)
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
                var orderedPosts = (IEnumerable<PostInfo>)args.Board.Posts.OrderByDescending(p => p.PostId);

                //if there's a StartPostId, only send posts with an id greater than or equal to it
                if (args.StartPostId.HasValue)
                    orderedPosts = orderedPosts.Where(p => p.PostId <= args.StartPostId.Value);

                //only send up to 127 posts (i have no fucking clue why its sbyte.MaxValue)
                var posts = orderedPosts.Take(sbyte.MaxValue)
                                        .ToList();

                writer.WriteSByte((sbyte)posts.Count);

                foreach (var post in posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.MonthOfYear);
                    writer.WriteByte((byte)post.DayOfMonth);
                    writer.WriteString8(post.Subject);
                }

                break;
            }
            case BoardOrResponseType.PublicPost:
            {
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //LI: what is this for?
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.MonthOfYear);
                writer.WriteByte((byte)args.Post.DayOfMonth);
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
                var posts = orderedPosts.Take(sbyte.MaxValue)
                                        .ToList();

                writer.WriteSByte((sbyte)posts.Count);

                foreach (var post in posts)
                {
                    writer.WriteBoolean(post.IsHighlighted);
                    writer.WriteInt16(post.PostId);
                    writer.WriteString8(post.Author);
                    writer.WriteByte((byte)post.MonthOfYear);
                    writer.WriteByte((byte)post.DayOfMonth);
                    writer.WriteString8(post.Subject);
                }

                break;
            }
            case BoardOrResponseType.MailPost:
            {
                writer.WriteBoolean(args.EnablePrevBtn);
                writer.WriteByte(0); //LI: what is this for?
                writer.WriteInt16(args.Post!.PostId);
                writer.WriteString8(args.Post.Author);
                writer.WriteByte((byte)args.Post.MonthOfYear);
                writer.WriteByte((byte)args.Post.DayOfMonth);
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