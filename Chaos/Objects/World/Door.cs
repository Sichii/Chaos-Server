using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
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

    public override void HideFrom(Aisling aisling) { }

    public override void OnClicked(Aisling source)
    {
        if (ShouldRegisterClick)
        {
            var allDoors = MapInstance.GetEntitiesWithinRange<Door>(this)
                                      .ToDictionary(d => d, PointEqualityComparer.Instance);

            IEnumerable<Door> GetSurroundingDoors(IPoint doorPoint)
            {
                foreach (var cardinalPoint in doorPoint.GetCardinalPoints())
                    if (allDoors.TryGetValue(cardinalPoint, out var adjacentDoor))
                        yield return adjacentDoor;
            }

            var allTouchingDoors = new HashSet<Door> { this };
            var pendingDiscovery = new Stack<Door>();
            pendingDiscovery.Push(this);

            //floodfill to find all touching doors
            while (pendingDiscovery.Any())
            {
                var popped = pendingDiscovery.Pop();

                foreach (var innerDoor in GetSurroundingDoors(popped))
                    if (allTouchingDoors.Add(innerDoor))
                        pendingDiscovery.Push(innerDoor);
            }

            foreach (var door in allTouchingDoors)
            {
                door.Closed = !door.Closed;
                door.LastClick = DateTime.UtcNow;
            }
            
            foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this, 20)
                                               .ThatCanSee(this))
            {
                var doorsInRange = allTouchingDoors
                    .ThatAreWithinRange(aisling);

                aisling.Client.SendDoors(doorsInRange);
            }
        }
    }

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDoors(this);
}