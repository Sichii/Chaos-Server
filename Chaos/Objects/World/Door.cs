using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.World;

public class Door : VisibleEntity
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
            var allDoors = MapInstance
                           .ObjectsWithinRange<Door>(this)
                           .ToDictionary(Point.From);

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

            foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this, 20))
            {
                var doorsInRange = allTouchingDoors
                    .Where(touchingDoor => touchingDoor.WithinRange(aisling));

                aisling.Client.SendDoors(doorsInRange);
            }

            foreach (var door in allTouchingDoors)
            {
                door.Closed = !door.Closed;
                door.LastClick = DateTime.UtcNow;
            }
        }
    }

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDoors(this);
}