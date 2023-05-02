using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.World;

public sealed class Door : VisibleEntity
{
    public bool Closed { get; set; }
    public DateTime LastClick { get; set; }
    public bool OpenRight { get; }
    public bool ShouldRegisterClick => DateTime.UtcNow.Subtract(LastClick).TotalSeconds > 1.5;

    public Door(
        bool openRight,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(sprite, mapInstance, point)
    {
        Closed = true;
        OpenRight = openRight;
        LastClick = DateTime.Now.Subtract(TimeSpan.FromHours(1));
    }

    public Door(DoorTemplate doorTemplate, MapInstance mapInstance)
        : this(
            doorTemplate.OpenRight,
            doorTemplate.Sprite,
            mapInstance,
            doorTemplate.Point) { }

    public IEnumerable<Door> GetCluster() => MapInstance.GetEntitiesWithinRange<Door>(this)
                                                        .FloodFill(this);

    public override void HideFrom(Aisling aisling) { }

    public override void OnClicked(Aisling source)
    {
        if (ShouldRegisterClick)
        {
            var doorCluster = GetCluster().ToList();

            foreach (var door in doorCluster)
            {
                door.Closed = !door.Closed;
                door.LastClick = DateTime.UtcNow;
            }

            foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this, 15)
                                               .ThatCanObserve(this))
                aisling.Client.SendDoors(doorCluster);
        }
    }

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDoors(GetCluster());
}