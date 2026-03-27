#region
using Chaos.Geometry;
using Chaos.Models.Map;
using Chaos.Models.Templates;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class MapTemplateTests
{
    private MapTemplate CreateTemplate(byte width = 10, byte height = 10)
        => new()
        {
            TemplateKey = "100",
            Width = width,
            Height = height,
            Bounds = new Rectangle(
                0,
                0,
                width,
                height),
            Tiles = new Tile[width, height],
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

    #region MapId
    [Test]
    public void MapId_ShouldParseTemplateKeyAsShort()
    {
        var template = CreateTemplate();

        template.MapId
                .Should()
                .Be(100);
    }
    #endregion

    #region IsWithinMap
    [Test]
    public void IsWithinMap_ShouldReturnTrue_WhenPointIsWithinBounds()
    {
        var template = CreateTemplate();

        template.IsWithinMap(new Point(5, 5))
                .Should()
                .BeTrue();
    }

    [Test]
    public void IsWithinMap_ShouldReturnFalse_WhenPointIsOutsideBounds()
    {
        var template = CreateTemplate();

        template.IsWithinMap(new Point(100, 100))
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsWithinMap_ShouldReturnTrue_WhenPointIsAtOrigin()
    {
        var template = CreateTemplate();

        template.IsWithinMap(new Point(0, 0))
                .Should()
                .BeTrue();
    }

    [Test]
    public void IsWithinMap_ShouldReturnFalse_WhenPointHasNegativeCoordinates()
    {
        var template = CreateTemplate();

        template.IsWithinMap(new Point(-1, -1))
                .Should()
                .BeFalse();
    }
    #endregion

    #region IsWall
    [Test]
    public void IsWall_ShouldReturnTrue_WhenPointIsOutsideMap()
    {
        var template = CreateTemplate();

        template.IsWall(new Point(100, 100))
                .Should()
                .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnFalse_WhenPointIsInsideMap_AndTileIsNotWall()
    {
        var template = CreateTemplate();

        // Default Tile[,] has all zeros — no foreground, so not a wall
        template.IsWall(new Point(0, 0))
                .Should()
                .BeFalse();
    }
    #endregion

    #region GetRowData
    [Test]
    public void GetRowData_ShouldReturnCorrectNumberOfBytes()
    {
        var template = CreateTemplate(3, 3);

        var rowData = template.GetRowData(0)
                              .ToList();

        // 3 tiles × 6 bytes each = 18
        rowData.Should()
               .HaveCount(18);
    }

    [Test]
    public void GetRowData_ShouldReturnTileByteData()
    {
        var template = CreateTemplate(1, 1);
        template.Tiles[0, 0] = new Tile(0x0102, 0x0304, 0x0506);

        var rowData = template.GetRowData(0)
                              .ToList();

        // Background high, low, Left high, low, Right high, low
        rowData.Should()
               .Equal(
                   0x01,
                   0x02,
                   0x03,
                   0x04,
                   0x05,
                   0x06);
    }
    #endregion
}