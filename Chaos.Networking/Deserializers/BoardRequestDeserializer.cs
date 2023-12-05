using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="BoardRequestArgs" />
/// </summary>
public sealed record BoardRequestDeserializer : ClientPacketDeserializer<BoardRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.BoardRequest;

    /// <inheritdoc />
    public override BoardRequestArgs Deserialize(ref SpanReader reader)
    {
        var boardRequestType = (BoardRequestType)reader.ReadByte();

        switch (boardRequestType)
        {
            case BoardRequestType.BoardList:
            {
                return new BoardRequestArgs(boardRequestType);
            }
            case BoardRequestType.ViewBoard:
            {
                var boardId = reader.ReadUInt16();
                var startPostId = reader.ReadInt16();

                //var unknown = reader.ReadByte();

                return new BoardRequestArgs(boardRequestType, boardId, StartPostId: startPostId);
            }
            case BoardRequestType.ViewPost:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();
                var controls = (BoardControls)reader.ReadSByte();

                return new BoardRequestArgs(
                    boardRequestType,
                    boardId,
                    postId,
                    Controls: controls);
            }
            case BoardRequestType.NewPost:
            {
                var boardId = reader.ReadUInt16();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                return new BoardRequestArgs(
                    boardRequestType,
                    boardId,
                    Subject: subject,
                    Message: message);
            }
            case BoardRequestType.Delete:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();

                return new BoardRequestArgs(boardRequestType, boardId, postId);
            }
            case BoardRequestType.SendMail:
            {
                var boardId = reader.ReadUInt16();
                var to = reader.ReadString8();
                var subject = reader.ReadString8();
                var message = reader.ReadString16();

                return new BoardRequestArgs(
                    boardRequestType,
                    boardId,
                    To: to,
                    Subject: subject,
                    Message: message);
            }
            case BoardRequestType.Highlight:
            {
                var boardId = reader.ReadUInt16();
                var postId = reader.ReadInt16();

                return new BoardRequestArgs(boardRequestType, boardId, postId);
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /*
        //this packet is literally retarded
        private void Board(Client client, ClientPacket packet)
        {
            var type = (BoardRequestType)packet.ReadByte();

            switch (type) //request type
            {
                case BoardRequestType.BoardList:
                    //Board List
                    //client.Enqueue(client.ServerPackets.BulletinBoard);
                    break;
                case BoardRequestType.ViewBoard:
                    {
                        //Post list for boardNum
                        ushort boardNum = packet.ReadUInt16();
                        ushort startPostNum = packet.ReadUInt16(); //you send the newest mail first, which will have the highest number. startPostNum counts down.
                        //packet.ReadByte() is always 0xF0(240) ???
                        //the client spam requests this like holy fuck, put a timer on this so you only send 1 packet
                        break;
                    }
                case BoardRequestType.ViewPost:
                    {
                        //Post
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want, counting up (what the fuck?)
                        //mailbox = boardNum 0
                        //otherwise boardnum is the index of the board you're accessing
                        switch (packet.ReadSByte()) //board controls
                        {
                            case -1: //clicked next for older post
                                break;
                            case 0: //requested a specific post from the post list
                                break;
                            case 1: //clicked previous for newer post
                                break;
                        }
                        break;
                    }
                case BoardRequestType.NewPost: //new post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Delete: //delete post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want to delete, counting up
                        break;
                    }

                case BoardRequestType.SendMail: //send mail
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string targetName = packet.ReadString8();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Highlight: //highlight message
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16();
                        break;
                    }
            }

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type}", client);
            Game.Boards(client);
        }
         */
}