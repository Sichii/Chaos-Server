using System.Drawing;
using Chaos.MetaData.Abstractions;
using Chaos.MetaData.EventMetaData;
using Chaos.MetaData.LightMetaData;
using Chaos.MetaData.MundaneMetaData;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.MetaData;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class MetaDataMapperProfile : IMapperProfile<IMetaDataDescriptor, MetaDataInfo>,
                                     IMapperProfile<EventMetaSchema, EventMetaNode>,
                                     IMapperProfile<MundaneIllustrationMetaSchema, MundaneIllustrationMetaNode>,
                                     IMapperProfile<LightMetaSchema, LightPropertyMetaNode>
{
    /// <inheritdoc />
    public IMetaDataDescriptor Map(MetaDataInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public MetaDataInfo Map(IMetaDataDescriptor obj)
        => new()
        {
            Name = obj.Name,
            CheckSum = obj.CheckSum,
            Data = obj.Data
        };

    /// <inheritdoc />
    public EventMetaSchema Map(EventMetaNode obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public EventMetaNode Map(EventMetaSchema obj)
    {
        string? classStr = null;
        string? circlesStr = null;
        var page = obj.PageOverride;

        if (obj.QualifyingClasses is { Count: > 0 })
            classStr = string.Join("", obj.QualifyingClasses.Select(c => (int)c));

        if (obj.QualifyingCircles is { Count: > 0 })
        {
            circlesStr = string.Join("", obj.QualifyingCircles!.Select(c => (int)c));
            page ??= (int)obj.QualifyingCircles!.Min();
        }

        var node = new EventMetaNode(obj.Title, page ?? 1)
        {
            Id = obj.Id,
            QualifyingClasses = classStr,
            QualifyingCircles = circlesStr,
            Rewards = obj.Rewards,
            PrerequisiteEventId = obj.PrerequisiteEventId,
            Summary = obj.Summary,
            Result = obj.Result
        };

        return node;
    }

    /// <inheritdoc />
    public MundaneIllustrationMetaSchema Map(MundaneIllustrationMetaNode obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public MundaneIllustrationMetaNode Map(MundaneIllustrationMetaSchema obj) => new(obj.Name, obj.ImageName);

    /// <inheritdoc />
    public LightMetaSchema Map(LightPropertyMetaNode obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public LightPropertyMetaNode Map(LightMetaSchema obj)
        => new(obj.LightTypeName)
        {
            StartHour = obj.StartHour,
            EndHour = obj.EndHour,
            EnumValue = obj.EnumValue,
            Color = Color.FromArgb(
                obj.Alpha,
                obj.Red,
                obj.Green,
                obj.Blue)
        };
}