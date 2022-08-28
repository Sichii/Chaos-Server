using Chaos.Containers;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class UserOptionsTypeMapper : ITypeMapper<UserOptions, UserOptionsSchema>
{
    public UserOptions Map(UserOptionsSchema obj) => new(obj);

    public UserOptionsSchema Map(UserOptions obj) => new()
    {
        Exchange = obj.Exchange,
        FastMove = obj.FastMove,
        Group = obj.Group,
        GuildChat = obj.GuildChat,
        Magic = obj.Magic,
        Shout = obj.Shout,
        Whisper = obj.Whisper,
        Wisdom = obj.Wisdom
    };
}