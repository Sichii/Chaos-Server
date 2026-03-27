#region
using Chaos.Collections;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockGroundItem
{
    public static GroundItem Create(
        MapInstance? mapInstance = null,
        string name = "TestItem",
        bool canBePickedUp = true,
        IPoint? position = null,
        Action<Item>? itemSetup = null)
    {
        mapInstance ??= MockMapInstance.Create();
        position ??= new Point(5, 5);

        var item = MockItem.Create(name, setup: itemSetup);

        Mock.Get(item.Script)
            .Setup(s => s.CanBePickedUp(It.IsAny<Aisling>(), It.IsAny<Point>()))
            .Returns(canBePickedUp);

        return new GroundItem(item, mapInstance, position);
    }
}