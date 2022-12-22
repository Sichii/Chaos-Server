using Chaos.Common.Abstractions;
using Chaos.Objects.World;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class ReactorTileMapperProfile : IMapperProfile<ReactorTile, ReactorTileSchema>,
                                        IMapperProfile<ReactorTileTemplate, ReactorTileTemplateSchema>
{
    /// <inheritdoc />
    public ReactorTile Map(ReactorTileSchema obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public ReactorTileSchema Map(ReactorTile obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public ReactorTileTemplate Map(ReactorTileTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        ShouldBlockPathfinding = obj.ShouldBlockPathfinding
    };

    /// <inheritdoc />
    public ReactorTileTemplateSchema Map(ReactorTileTemplate obj) => throw new NotImplementedException();
}