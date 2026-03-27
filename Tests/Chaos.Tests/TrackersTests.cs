#region
using Chaos.Collections;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class TrackersTests
{
    #region Update
    [Test]
    public void Update_ShouldNotThrow()
    {
        var trackers = new Trackers();

        var act = () => trackers.Update(TimeSpan.FromSeconds(1));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region AislingTrackers.LastEquipOrUnequip
    [Test]
    public void LastEquipOrUnequip_ShouldReturnLastEquip_WhenMoreRecent()
    {
        var trackers = new AislingTrackers
        {
            LastEquip = new DateTime(2026, 1, 2),
            LastUnequip = new DateTime(2026, 1, 1)
        };

        trackers.LastEquipOrUnequip
                .Should()
                .Be(new DateTime(2026, 1, 2));
    }

    [Test]
    public void LastEquipOrUnequip_ShouldReturnLastUnequip_WhenMoreRecent()
    {
        var trackers = new AislingTrackers
        {
            LastEquip = new DateTime(2026, 1, 1),
            LastUnequip = new DateTime(2026, 1, 2)
        };

        trackers.LastEquipOrUnequip
                .Should()
                .Be(new DateTime(2026, 1, 2));
    }

    [Test]
    public void LastEquipOrUnequip_ShouldReturnMinValue_WhenBothNull()
    {
        var trackers = new AislingTrackers();

        trackers.LastEquipOrUnequip
                .Should()
                .Be(DateTime.MinValue);
    }

    [Test]
    public void LastEquipOrUnequip_ShouldReturnLastEquip_WhenUnequipIsNull()
    {
        var trackers = new AislingTrackers
        {
            LastEquip = new DateTime(2026, 5, 5)
        };

        trackers.LastEquipOrUnequip
                .Should()
                .Be(new DateTime(2026, 5, 5));
    }

    [Test]
    public void LastEquipOrUnequip_ShouldReturnLastUnequip_WhenEquipIsNull()
    {
        var trackers = new AislingTrackers
        {
            LastUnequip = new DateTime(2026, 5, 5)
        };

        trackers.LastEquipOrUnequip
                .Should()
                .Be(new DateTime(2026, 5, 5));
    }
    #endregion
}