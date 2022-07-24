using AutoMapper;
using Chaos.Containers;
using Chaos.Objects.Serializable;

namespace Chaos.Mappers;

public class UserOptionsMapper : Profile
{
    public UserOptionsMapper()
    {
        CreateMap<SerializableOptions, UserOptions>();
        CreateMap<UserOptions, SerializableOptions>();
    }
}