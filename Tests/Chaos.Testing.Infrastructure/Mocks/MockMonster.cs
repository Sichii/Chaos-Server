#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockMonster
{
    private static int Counter;

    public static Monster Create(
        MapInstance? mapInstance = null,
        string? name = null,
        int level = 1,
        Func<MonsterTemplate, MonsterTemplate>? templateSetup = null,
        Action<Monster>? setup = null)
    {
        mapInstance ??= MockMapInstance.Create();
        name ??= $"TestMonster{Interlocked.Increment(ref Counter)}";

        var template = new MonsterTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            AggroRange = 5,
            AssailIntervalMs = 1000,
            ExpReward = 100,
            AbilityReward = 0,
            MinGoldDrop = 0,
            MaxGoldDrop = 0,
            MoveIntervalMs = 1000,
            SkillIntervalMs = 1000,
            SpellIntervalMs = 1000,
            WanderIntervalMs = 1000,
            Sprite = 1,
            Type = CreatureType.Normal,
            LootTables = [],
            SkillTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            SpellTemplateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            StatSheet = new StatSheet
            {
                Level = level,
                CurrentHp = 1000,
                CurrentMp = 500
            }
        };

        if (templateSetup is not null)
            template = templateSetup(template);

        var loggerMock = new Mock<ILogger<Monster>>();

        var monster = new Monster(
            template,
            mapInstance,
            new Point(5, 5),
            loggerMock.Object,
            MockScriptProvider.Instance.Object);

        setup?.Invoke(monster);

        return monster;
    }

    /// <summary>
    ///     Resets the monster's script mock and optionally configures it
    /// </summary>
    public static void SetupScript(Monster monster, Action<Mock<IMonsterScript>>? setup = null)
    {
        var scriptMock = Mock.Get(monster.Script);
        scriptMock.Reset();
        setup?.Invoke(scriptMock);
    }
}