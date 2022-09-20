using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Data;

public record WarpSchema
{
    /// <summary>
    ///     A string representation of a location<br />The map instance id and coordinates the warp sends you to when stepped on<br /> Ex.
    ///     "mileth1:(10, 10)"
    /// </summary>
    public required Location Destination { get; init; }

    /// <summary>
    ///     A string representation of a point.<br />The tile coordinates the warp is on<br />Ex. "(50, 15)"
    /// </summary>
    public required Point Source { get; init; }
}