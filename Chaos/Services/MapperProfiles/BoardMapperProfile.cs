using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Abstractions;
using Chaos.Models.Board;
using Chaos.Models.Templates;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Boards;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Other;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.MapperProfiles;

public class BoardMapperProfile(
    ITypeMapper mapper,
    BulletinBoardKeyMapper keyMapper,
    ILoggerFactory loggerFactory,
    IScriptProvider scriptProvider,
    ISimpleCache simpleCache) : IMapperProfile<MailBox, MailBoxSchema>,
                                IMapperProfile<Post, PostSchema>,
                                IMapperProfile<BoardBase, BoardInfo>,
                                IMapperProfile<Post, PostInfo>,
                                IMapperProfile<BulletinBoardTemplate, BulletinBoardTemplateSchema>,
                                IMapperProfile<BulletinBoard, BulletinBoardSchema>
{
    private readonly BulletinBoardKeyMapper KeyMapper = keyMapper;
    private readonly ILoggerFactory LoggerFactory = loggerFactory;
    private readonly ITypeMapper Mapper = mapper;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    /// <inheritdoc />
    public MailBox Map(MailBoxSchema obj) => new(obj.Key, LoggerFactory.CreateLogger<MailBox>(), Mapper.MapMany<Post>(obj.Posts));

    /// <inheritdoc />
    public MailBoxSchema Map(MailBox obj)
        => new()
        {
            Key = obj.Key,
            Posts = Mapper.MapMany<Post, PostSchema>(obj)
                          .ToList()
        };

    /// <inheritdoc />
    public BoardBase Map(BoardInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    BoardInfo IMapperProfile<BoardBase, BoardInfo>.Map(BoardBase obj)
        => new()
        {
            BoardId = obj.BoardId,
            Name = obj.Name,
            Posts = Mapper.MapMany<Post, PostInfo>(obj)
                          .ToList()
        };

    /// <inheritdoc />
    public Post Map(PostSchema obj)
        => new(
            0,
            obj.Author,
            obj.Subject,
            obj.Message,
            obj.CreationDate,
            obj.IsHighlighted);

    /// <inheritdoc />
    public Post Map(PostInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    PostInfo IMapperProfile<Post, PostInfo>.Map(Post obj)
        => new()
        {
            PostId = obj.PostId,
            Author = obj.Author,
            Subject = obj.Subject,
            Message = obj.Message,
            CreationDate = obj.CreationDate,
            IsHighlighted = obj.IsHighlighted
        };

    /// <inheritdoc />
    public PostSchema Map(Post obj)
        => new()
        {
            Author = obj.Author,
            Subject = obj.Subject,
            Message = obj.Message,
            CreationDate = obj.CreationDate,
            IsHighlighted = obj.IsHighlighted
        };

    /// <inheritdoc />
    public BulletinBoardTemplate Map(BulletinBoardTemplateSchema obj)
        => new()
        {
            TemplateKey = obj.TemplateKey,
            Id = KeyMapper.GetId(obj.TemplateKey),
            Name = obj.Name,
            ScriptKeys = obj.ScriptKeys.ToHashSet(StringComparer.OrdinalIgnoreCase),
            ScriptVars = obj.ScriptVars.ToDictionary(kvp => kvp.Key, kvp => (IScriptVars)kvp.Value, StringComparer.OrdinalIgnoreCase)
        };

    /// <inheritdoc />
    public BulletinBoardTemplateSchema Map(BulletinBoardTemplate obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public BulletinBoard Map(BulletinBoardSchema obj)
    {
        var template = SimpleCache.Get<BulletinBoardTemplate>(obj.TemplateKey);
        var posts = Mapper.MapMany<Post>(obj.Posts);

        var board = new BulletinBoard(
            template,
            LoggerFactory.CreateLogger<BulletinBoard>(),
            ScriptProvider,
            posts);

        return board;
    }

    /// <inheritdoc />
    public BulletinBoardSchema Map(BulletinBoard obj)
        => new()
        {
            TemplateKey = obj.Template.TemplateKey,
            Posts = Mapper.MapMany<Post, PostSchema>(obj)
                          .ToList()
        };
}