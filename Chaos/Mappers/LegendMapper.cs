using System.Collections.Generic;
using AutoMapper;
using Chaos.Containers;
using Chaos.Core.Data;
using Chaos.DataObjects.Serializable;
using Chaos.Extensions;

namespace Chaos.Mappers;

public class LegendMapper : Profile
{
    public LegendMapper()
    {
        CreateMap<SerializableLegendMark, LegendMark>(MemberList.None)
            .ConstructUsing(s => new LegendMark(s.Text,
                s.Key,
                s.Icon,
                s.Color,
                s.Count,
                new GameTime(s.Added)));

        CreateMap<LegendMark, SerializableLegendMark>(MemberList.None);

        CreateMap<IEnumerable<SerializableLegendMark>, Legend>(MemberList.None)
            .ConstructUsing((e, rc) => new Legend(rc.MapEnumerable<SerializableLegendMark, LegendMark>(e)));

        CreateMap<Legend, List<SerializableLegendMark>>(MemberList.None)
            .ConstructUsing((e, rc) => new List<SerializableLegendMark>(rc.MapEnumerable<LegendMark, SerializableLegendMark>(e)));
    }
}