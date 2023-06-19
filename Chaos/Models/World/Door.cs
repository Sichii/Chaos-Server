using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;

namespace Chaos.Models.World;

public sealed class Door : VisibleEntity
{
    public bool Closed { get; set; }
    public bool OpenRight { get; }

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
        if (!ShouldRegisterClick(source.Id))
            return;

        var doorCluster = GetCluster().ToList();

        foreach (var door in doorCluster)
        {
            door.Closed = !door.Closed;
            door.LastClicked[source.Id] = DateTime.UtcNow;
        }

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this, 15)
                                           .ThatCanObserve(this))
            aisling.Client.SendDoors(doorCluster);
    }

    public override bool ShouldRegisterClick(uint fromId) =>
        !LastClicked.Any() || (DateTime.UtcNow.Subtract(LastClicked.Values.Max()).TotalMilliseconds > 1500);

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDoors(GetCluster());
}