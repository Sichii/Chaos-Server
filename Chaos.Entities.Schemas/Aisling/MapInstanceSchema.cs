using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Data;

namespace Chaos.Entities.Schemas.Aisling;

public record MapInstanceSchema
{
    public required MapFlags Flags { get; init; }
    public required string InstanceId { get; init; }
    public required sbyte Music { get; init; }
    public required string Name { get; init; }
    public required string TemplateKey { get; init; }
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required WarpSchema[] Warps { get; init; } = Array.Empty<WarpSchema>();
}