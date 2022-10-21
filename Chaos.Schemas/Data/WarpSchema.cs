using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WarpSchema
{
    /// <summary>
    ///     A string representation of a location<br />The map instance id and coordinates the warp sends you to when stepped on<br /> Ex.
    ///     "mileth1:(10, 10)"
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Location Destination { get; init; }

    /// <summary>
    ///     A string representation of a point.<br />The tile coordinates the warp is on<br />Ex. "(50, 15)"
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; init; }
}