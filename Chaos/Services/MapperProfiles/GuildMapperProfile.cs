using Chaos.Collections;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Schemas.Guilds;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class GuildMapperProfile(IChannelService channelService, IClientRegistry<IWorldClient> clientRegistry)
    : IMapperProfile<Guild, GuildSchema>, IMapperProfile<GuildRank, GuildRankSchema>
{
    private readonly IChannelService ChannelService = channelService;
    private readonly IClientRegistry<IWorldClient> ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public Guild Map(GuildSchema obj)
        => new(
            obj.Name,
            obj.Guid,
            ChannelService,
            ClientRegistry);

    /// <inheritdoc />
    public GuildSchema Map(Guild obj)
        => new()
        {
            Name = obj.Name,
            Guid = obj.Guid
        };

    /// <inheritdoc />
    public GuildRank Map(GuildRankSchema obj) => new(obj.RankName, obj.Tier, obj.Members);

    /// <inheritdoc />
    public GuildRankSchema Map(GuildRank obj)
        => new()
        {
            RankName = obj.Name,
            Tier = obj.Tier,
            Members = obj.GetMemberNames()
                         .ToList()
        };
}