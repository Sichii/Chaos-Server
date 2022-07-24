using AutoMapper;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Serializable;

namespace Chaos.Mappers;

public class LegendMapper : Profile
{
    public LegendMapper()
    {
        CreateMap<LegendMark, SerializableLegendMark>(MemberList.None);
        
        CreateMap<Legend, List<SerializableLegendMark>>(MemberList.None)
            .ConstructUsing((e, rc) => new List<SerializableLegendMark>(rc.MapEnumerable<LegendMark, SerializableLegendMark>(e)));
    }
}