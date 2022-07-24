using Chaos.Containers;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class Door : VisibleEntity
{
    public bool Closed { get; set; }
    public DateTime LastClick { get; set; }
    public bool OpenRight { get; }
    public bool ShouldRegisterClick => DateTime.UtcNow.Subtract(LastClick).TotalSeconds > 1.5;

    public Door(bool openRight, ushort sprite, MapInstance mapInstance, IPoint point)
    :base(sprite, mapInstance, point)
    {
        OpenRight = openRight;
        LastClick = DateTime.Now.Subtract(TimeSpan.FromHours(1));
    }

    /*
    public override void OnClicked(User source)
    {
        if (ShouldRegisterClick)
        {
            Closed = !Closed;
            LastClick = DateTime.UtcNow;
            source.Client.SendDoors(this);
        }
    }*/
}