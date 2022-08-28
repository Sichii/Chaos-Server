using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Data;

namespace Chaos.Entities.Schemas.World;

public record MapInstanceSchema
{
    public MapFlags Flags { get; init; }
    public string InstanceId { get; init; } = null!;
    public sbyte Music { get; init; }
    public string Name { get; init; } = null!;
    public string TemplateKey { get; init; } = null!;
    public WarpSchema[] Warps { get; init; } = Array.Empty<WarpSchema>();
}