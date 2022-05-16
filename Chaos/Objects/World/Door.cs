using Chaos.Containers;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

/// <summary>
///     Represents a clickable door, which can open or close in game.
/// </summary>
public class Door : VisibleObject
{
    public bool Closed { get; set; }
    public DateTime LastClick { get; set; } = DateTime.MinValue;
    public bool OpenRight { get; }
    public bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;

    public Door(
        MapInstance mapInstance,
        Point point,
        ushort sprite,
        bool openRight
    )
        : base(mapInstance, point, sprite) =>
        OpenRight = openRight;

    //constructor for MapTemplate doors
    public Door(Point point, ushort sprite, bool openRight)
        : base(null!, point, sprite) => OpenRight = openRight;

    public override void OnClicked(User source)
    {
        if (!RecentlyClicked)
        {
            Closed = !Closed;
            LastClick = DateTime.UtcNow;
            source.Client.SendDoors(this);
        }
    }
}