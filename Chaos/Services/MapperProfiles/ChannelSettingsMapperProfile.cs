#region
using Chaos.Models.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class ChannelSettingsMapperProfile : IMapperProfile<ChannelSettings, ChannelSettingsSchema>
{
    /// <inheritdoc />
    public ChannelSettings Map(ChannelSettingsSchema obj) => new(obj.ChannelName, obj.CustomChannel, obj.MessageColor);

    /// <inheritdoc />
    public ChannelSettingsSchema Map(ChannelSettings obj)
        => new()
        {
            ChannelName = obj.ChannelName,
            MessageColor = obj.MessageColor,
            CustomChannel = obj.CustomChannel
        };
}