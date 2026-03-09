#region
using System.Net;
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.FunctionalScripts;
using Chaos.Scripting.FunctionalScripts.NaturalRegeneration;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ConnectionInfo = Chaos.Networking.Options.ConnectionInfo;
#endregion

namespace Chaos.Testing.Infrastructure;

/// <summary>
///     Shared setup for test projects that create domain objects requiring static singletons (WorldOptions,
///     FunctionalScriptRegistry, NLog transformations). Call <see cref="InitializeWorld" /> from a [ModuleInitializer] in
///     your test project.
/// </summary>
public static class ServerSetup
{
    /// <summary>
    ///     Initializes WorldOptions.Instance, FunctionalScriptRegistry, and NLog collection transformations. Safe to call
    ///     multiple times — subsequent calls are no-ops.
    /// </summary>
    public static void InitializeWorld()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (WorldOptions.Instance is not null)
            return;

        WorldOptions.Instance = new WorldOptions
        {
            Address = IPAddress.Loopback,
            Port = 4202,
            AislingAssailIntervalMs = 500,
            DefaultChannels = [],
            DropRange = 1,
            F1MerchantTemplateKey = "test_merchant",
            GroundItemDespawnTimeMins = 30,
            GroupChatName = "!group",
            GroupMessageColor = MessageColor.Orange,
            GuildChatName = "!guild",
            GuildMessageColor = MessageColor.Default,
            LoginRedirect = new ConnectionInfo
            {
                Address = IPAddress.Loopback,
                Port = 4201
            },
            MaxActionsPerSecond = 10,
            MaxChantTimeBurdenMs = 5000,
            MaxGoldHeld = 10_000_000,
            MaxGroupSize = 6,
            MaximumAislingAc = 100,
            MaximumMonsterAc = 100,
            MaxItemsPerSecond = 5,
            MaxLevel = 99,
            MaxSkillsPerSecond = 5,
            MaxSpellsPerSecond = 5,
            MinimumAislingAc = -100,
            MinimumMonsterAc = -100,
            PickupRange = 1,
            ProhibitF5Walk = false,
            ProhibitItemSwitchWalk = false,
            ProhibitSpeedWalk = false,
            RefreshIntervalMs = 75,
            SaveIntervalMins = 5,
            TradeRange = 1,
            UpdatesPerSecond = 15
        };

        var services = new ServiceCollection();
        services.AddSingleton<DefaultNaturalRegenerationScript>();
        var sp = services.BuildServiceProvider();

        var registry = new FunctionalScriptRegistry(sp);
        registry.Register(DefaultNaturalRegenerationScript.Key, typeof(DefaultNaturalRegenerationScript));

        LogManager.Setup()
                  .SetupSerialization(builder => builder.RegisterCollectionTransformations((Group obj) => new
                  {
                      Id = obj.Id,
                      LeaderName = obj.Leader.Name,
                      MemberCount = obj.Count
                  }));
    }
}