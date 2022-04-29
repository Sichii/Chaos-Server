using AutoMapper;
using Chaos.Containers;
using Chaos.DataObjects.Serializable;

namespace Chaos.Mappers;

public class UserOptionsMapper : Profile
{
    public UserOptionsMapper()
    {
        CreateMap<SerializableOptions, UserOptions>(MemberList.None)
            .ForMember(u => u.Exchange,
                o => o.MapFrom(s => s.Exchange))
            .ForMember(u => u.Group,
                o => o.MapFrom(s => s.Group))
            .ForMember(u => u.Magic,
                o => o.MapFrom(s => s.Magic))
            .ForMember(u => u.Wisdom,
                o => o.MapFrom(s => s.Wisdom))
            .ForMember(u => u.Shout,
                o => o.MapFrom(s => s.Shout))
            .ForMember(u => u.Whisper,
                o => o.MapFrom(s => s.Whisper))
            .ForMember(u => u.FastMove,
                o => o.MapFrom(s => s.FastMove))
            .ForMember(u => u.GuildChat,
                o => o.MapFrom(s => s.GuildChat));

        CreateMap<UserOptions, SerializableOptions>(MemberList.None)
            .ForMember(s => s.Exchange,
                o => o.MapFrom(u => u.Exchange))
            .ForMember(s => s.Group,
                o => o.MapFrom(u => u.Group))
            .ForMember(s => s.Magic,
                o => o.MapFrom(u => u.Magic))
            .ForMember(s => s.Wisdom,
                o => o.MapFrom(u => u.Wisdom))
            .ForMember(s => s.Shout,
                o => o.MapFrom(u => u.Shout))
            .ForMember(s => s.Whisper,
                o => o.MapFrom(u => u.Whisper))
            .ForMember(s => s.FastMove,
                o => o.MapFrom(u => u.FastMove))
            .ForMember(s => s.GuildChat,
                o => o.MapFrom(u => u.GuildChat));
    }
}