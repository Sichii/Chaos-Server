#region
using Chaos.Collections;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Time;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class RegenTimerTests
{
    #region Update
    [Test]
    public void Update_ShouldNotRegenerate_WhenIntervalHasNotElapsed()
    {
        var map = MockMapInstance.Create();
        var monster = MockMonster.Create(map, "TestMonster");

        var regenFormulaMock = new Mock<IRegenFormula>();

        regenFormulaMock.Setup(f => f.CalculateIntervalSecs(It.IsAny<Creature>()))
                        .Returns(10);

        var regenScriptMock = new Mock<INaturalRegenerationScript>();

        regenScriptMock.SetupGet(s => s.RegenFormula)
                       .Returns(regenFormulaMock.Object);

        var timer = new RegenTimer(monster, regenScriptMock.Object);

        // Update with less than the interval
        timer.Update(TimeSpan.FromSeconds(5));

        regenScriptMock.Verify(s => s.Regenerate(It.IsAny<Creature>()), Times.Never);
    }

    [Test]
    public void Update_ShouldRegenerate_WhenIntervalHasElapsed()
    {
        var map = MockMapInstance.Create();
        var monster = MockMonster.Create(map, "TestMonster");

        var regenFormulaMock = new Mock<IRegenFormula>();

        regenFormulaMock.Setup(f => f.CalculateIntervalSecs(It.IsAny<Creature>()))
                        .Returns(5);

        var regenScriptMock = new Mock<INaturalRegenerationScript>();

        regenScriptMock.SetupGet(s => s.RegenFormula)
                       .Returns(regenFormulaMock.Object);

        var timer = new RegenTimer(monster, regenScriptMock.Object);

        // Update with more than the interval
        timer.Update(TimeSpan.FromSeconds(6));

        regenScriptMock.Verify(s => s.Regenerate(monster), Times.Once);
    }

    [Test]
    public void Update_ShouldRecalculateInterval_AfterRegeneration()
    {
        var map = MockMapInstance.Create();
        var monster = MockMonster.Create(map, "TestMonster");

        var callCount = 0;
        var regenFormulaMock = new Mock<IRegenFormula>();

        regenFormulaMock.Setup(f => f.CalculateIntervalSecs(It.IsAny<Creature>()))
                        .Returns(() =>
                        {
                            callCount++;

                            // Return 5 for constructor, 10 after first regen
                            return callCount == 1 ? 5 : 10;
                        });

        var regenScriptMock = new Mock<INaturalRegenerationScript>();

        regenScriptMock.SetupGet(s => s.RegenFormula)
                       .Returns(regenFormulaMock.Object);

        var timer = new RegenTimer(monster, regenScriptMock.Object);

        // First regen at 5 seconds
        timer.Update(TimeSpan.FromSeconds(6));

        regenScriptMock.Verify(s => s.Regenerate(monster), Times.Once);

        // CalculateIntervalSecs should have been called twice:
        // once in constructor, once after regen
        regenFormulaMock.Verify(f => f.CalculateIntervalSecs(monster), Times.Exactly(2));
    }
    #endregion
}