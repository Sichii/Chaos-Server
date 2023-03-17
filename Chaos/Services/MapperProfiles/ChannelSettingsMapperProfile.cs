using Chaos.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class ChannelSettingsMapperProfile : IMapperProfile<ChannelSettings, ChannelSettingsSchema>
{
    /// <inheritdoc />
    public ChannelSettings Map(ChannelSettingsSchema obj) => new(obj.ChannelName, obj.MessageColor);

    /// <inheritdoc />
    public ChannelSettingsSchema Map(ChannelSettings obj) => new()
    {
        ChannelName = obj.ChannelName,
        MessageColor = obj.MessageColor
    };
}