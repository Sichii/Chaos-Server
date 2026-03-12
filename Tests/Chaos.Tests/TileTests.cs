#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Map;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class TileTests
{
    #region IsWall
    [Test]
    public void IsWall_ShouldReturnFalse_WhenBothForegroundsAreZero()
    {
        var tile = new Tile(1, 0, 0);

        tile.IsWall
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_WhenLeftForegroundIsWall()
    {
        // Find a foreground index whose Sotp entry has the Wall flag
        var wallIndex = Array.FindIndex(Tile.Sotp, f => f.HasFlag(TileFlags.Wall));

        // Skip if no wall tile in sotp data (shouldn't happen in practice)
        if (wallIndex < 0)
            return;

        var tile = new Tile(1, (ushort)(wallIndex + 1), 0);

        tile.IsWall
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_WhenRightForegroundIsWall()
    {
        var wallIndex = Array.FindIndex(Tile.Sotp, f => f.HasFlag(TileFlags.Wall));

        if (wallIndex < 0)
            return;

        var tile = new Tile(1, 0, (ushort)(wallIndex + 1));

        tile.IsWall
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnFalse_WhenLeftForegroundIsNotWall()
    {
        // Find a foreground index that does NOT have Wall flag
        var nonWallIndex = Array.FindIndex(Tile.Sotp, f => !f.HasFlag(TileFlags.Wall));

        if (nonWallIndex < 0)
            return;

        var tile = new Tile(1, (ushort)(nonWallIndex + 1), 0);

        tile.IsWall
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsWall_ShouldReturnFalse_WhenRightForegroundIsNotWall()
    {
        var nonWallIndex = Array.FindIndex(Tile.Sotp, f => !f.HasFlag(TileFlags.Wall));

        if (nonWallIndex < 0)
            return;

        var tile = new Tile(1, 0, (ushort)(nonWallIndex + 1));

        tile.IsWall
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_WhenBothForegroundsAreWalls()
    {
        var wallIndex = Array.FindIndex(Tile.Sotp, f => f.HasFlag(TileFlags.Wall));

        if (wallIndex < 0)
            return;

        var tile = new Tile(1, (ushort)(wallIndex + 1), (ushort)(wallIndex + 1));

        tile.IsWall
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsWall_ShouldReturnTrue_WhenLeftNotWallButRightIsWall()
    {
        var nonWallIndex = Array.FindIndex(Tile.Sotp, f => !f.HasFlag(TileFlags.Wall));
        var wallIndex = Array.FindIndex(Tile.Sotp, f => f.HasFlag(TileFlags.Wall));

        if ((nonWallIndex < 0) || (wallIndex < 0))
            return;

        var tile = new Tile(1, (ushort)(nonWallIndex + 1), (ushort)(wallIndex + 1));

        tile.IsWall
            .Should()
            .BeTrue();
    }
    #endregion

    #region IsWater
    [Test]
    public void IsWater_ShouldReturnTrue_WhenBackgroundIsWaterTile()
    {
        // 16977 - 1 = 16976, which is in WATER_TILE_IDS
        var tile = new Tile(16977, 0, 0);

        tile.IsWater
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsWater_ShouldReturnFalse_WhenBackgroundIsNotWaterTile()
    {
        var tile = new Tile(1, 0, 0);

        tile.IsWater
            .Should()
            .BeFalse();
    }
    #endregion

    #region Equals(Tile)
    [Test]
    public void Equals_SameTile_ShouldReturnTrue()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(1, 2, 3);

        tile1.Equals(tile2)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Equals_DifferentBackground_ShouldReturnFalse()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(99, 2, 3);

        tile1.Equals(tile2)
             .Should()
             .BeFalse();
    }

    [Test]
    public void Equals_DifferentLeftForeground_ShouldReturnFalse()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(1, 99, 3);

        tile1.Equals(tile2)
             .Should()
             .BeFalse();
    }

    [Test]
    public void Equals_DifferentRightForeground_ShouldReturnFalse()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(1, 2, 99);

        tile1.Equals(tile2)
             .Should()
             .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameTile_ShouldReturnTrue()
    {
        var tile1 = new Tile(1, 2, 3);
        object tile2 = new Tile(1, 2, 3);

        tile1.Equals(tile2)
             .Should()
             .BeTrue();
    }

    [Test]
    public void EqualsObject_NotATile_ShouldReturnFalse()
    {
        var tile = new Tile(1, 2, 3);

        // ReSharper disable once SuspiciousTypeConversion.Global
        tile.Equals("not a tile")
            .Should()
            .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ShouldReturnFalse()
    {
        var tile = new Tile(1, 2, 3);

        tile.Equals(null)
            .Should()
            .BeFalse();
    }
    #endregion

    #region Operators
    [Test]
    public void OperatorEquals_SameTile_ShouldReturnTrue()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(1, 2, 3);

        (tile1 == tile2).Should()
                        .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentTile_ShouldReturnTrue()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(4, 5, 6);

        (tile1 != tile2).Should()
                        .BeTrue();
    }
    #endregion

    #region GetHashCode
    [Test]
    public void GetHashCode_SameTiles_ShouldBeEqual()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(1, 2, 3);

        tile1.GetHashCode()
             .Should()
             .Be(tile2.GetHashCode());
    }

    [Test]
    public void GetHashCode_DifferentTiles_ShouldTypicallyDiffer()
    {
        var tile1 = new Tile(1, 2, 3);
        var tile2 = new Tile(4, 5, 6);

        tile1.GetHashCode()
             .Should()
             .NotBe(tile2.GetHashCode());
    }
    #endregion
}