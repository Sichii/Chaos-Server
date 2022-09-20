using Chaos.Common.Definitions;
using Chaos.Geometry.Definitions;

namespace Chaos.Entities.Schemas.Aisling;

public record AislingSchema
{
    public required BodyColor BodyColor { get; init; }
    public required BodySprite BodySprite { get; init; }
    public required Direction Direction { get; init; }
    public required ICollection<EffectSchema> Effects { get; init; } = Array.Empty<EffectSchema>();
    public required int FaceSprite { get; init; }
    public required int GamePoints { get; init; }
    public required Gender Gender { get; init; }
    public required int Gold { get; init; }
    public required string? GuildName { get; init; }
    public required string? GuildTitle { get; init; }
    public required DisplayColor HairColor { get; init; }
    public required int HairStyle { get; init; }
    public required ICollection<string> IgnoreList { get; init; } = Array.Empty<string>();
    public required bool IsAdmin { get; init; }
    public required string MapInstanceId { get; init; }
    public required string Name { get; init; }
    public required Nation Nation { get; init; }
    public required UserStatSheetSchema StatSheet { get; init; }
    public required ICollection<string> Titles { get; init; } = Array.Empty<string>();
    public required UserOptionsSchema UserOptions { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
}