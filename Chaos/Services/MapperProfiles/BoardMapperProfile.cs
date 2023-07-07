using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Models.Board;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Boards;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Other;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.MapperProfiles;

public class BoardMapperProfile : IMapperProfile<MailBox, MailBoxSchema>,
                                  IMapperProfile<BulletinBoard, BulletinBoardSchema>,
                                  IMapperProfile<Post, PostSchema>,
                                  IMapperProfile<BoardBase, BoardInfo>,
                                  IMapperProfile<Post, PostInfo>
{
    private readonly BoardKeyMapper KeyMapper;
    private readonly ILoggerFactory LoggerFactory;
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;

    public BoardMapperProfile(
        ITypeMapper mapper,
        BoardKeyMapper keyMapper,
        ILoggerFactory loggerFactory,
        IScriptProvider scriptProvider
    )
    {
        Mapper = mapper;
        KeyMapper = keyMapper;
        LoggerFactory = loggerFactory;
        ScriptProvider = scriptProvider;
    }

    /// <inheritdoc />
    public MailBox Map(MailBoxSchema obj) => new(obj.Key, LoggerFactory.CreateLogger<MailBox>(), Mapper.MapMany<Post>(obj.Posts));

    /// <inheritdoc />
    public MailBoxSchema Map(MailBox obj) => new()
    {
        Key = obj.Key,
        Posts = Mapper.MapMany<Post, PostSchema>(obj).ToList()
    };

    /// <inheritdoc />
    public BulletinBoard Map(BulletinBoardSchema obj) => new(
        KeyMapper.GetId(obj.Key),
        obj.Name,
        obj.Key,
        Mapper.MapMany<Post>(obj.Posts),
        obj.Moderators,
        LoggerFactory.CreateLogger<BulletinBoard>(),
        ScriptProvider,
        obj.ScriptKeys);

    /// <inheritdoc />
    public BulletinBoardSchema Map(BulletinBoard obj) => new()
    {
        Name = obj.Name,
        Key = obj.Key,
        Moderators = obj.Moderators,
        Posts = Mapper.MapMany<Post, PostSchema>(obj).ToList()
    };

    /// <inheritdoc />
    public BoardBase Map(BoardInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    BoardInfo IMapperProfile<BoardBase, BoardInfo>.Map(BoardBase obj) => new()
    {
        BoardId = obj.BoardId,
        Name = obj.Name,
        Posts = Mapper.MapMany<Post, PostInfo>(obj).ToList()
    };

    /// <inheritdoc />
    public Post Map(PostSchema obj) => new(
        0,
        obj.Author,
        obj.Subject,
        obj.Message,
        obj.CreationDate,
        obj.IsHighlighted);

    /// <inheritdoc />
    public Post Map(PostInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    PostInfo IMapperProfile<Post, PostInfo>.Map(Post obj) => new()
    {
        PostId = obj.PostId,
        Author = obj.Author,
        Subject = obj.Subject,
        Message = obj.Message,
        CreationDate = obj.CreationDate,
        IsHighlighted = obj.IsHighlighted
    };

    /// <inheritdoc />
    public PostSchema Map(Post obj) => new()
    {
        Author = obj.Author,
        Subject = obj.Subject,
        Message = obj.Message,
        CreationDate = obj.CreationDate,
        IsHighlighted = obj.IsHighlighted
    };
}