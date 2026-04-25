#region
using System.Net;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Board;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.FunctionalScripts;
using Chaos.Scripting.FunctionalScripts.AbilityDistribution;
using Chaos.Scripting.FunctionalScripts.AbilityUp;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Scripting.FunctionalScripts.NaturalRegeneration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            MaxAbilityLevel = 99,
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
        services.AddSingleton<DefaultExperienceDistributionScript>();
        services.AddSingleton<DefaultLevelUpScript>();
        services.AddSingleton<DefaultAbilityDistributionScript>();
        services.AddSingleton<DefaultAbilityUpScript>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        var sp = services.BuildServiceProvider();

        var registry = new FunctionalScriptRegistry(sp);
        registry.Register(DefaultNaturalRegenerationScript.Key, typeof(DefaultNaturalRegenerationScript));
        registry.Register(DefaultExperienceDistributionScript.Key, typeof(DefaultExperienceDistributionScript));
        registry.Register(DefaultLevelUpScript.Key, typeof(DefaultLevelUpScript));
        registry.Register(DefaultAbilityDistributionScript.Key, typeof(DefaultAbilityDistributionScript));
        registry.Register(DefaultAbilityUpScript.Key, typeof(DefaultAbilityUpScript));

        LogManager.Setup()
                  .SetupSerialization(builder =>
                  {
                      builder.RegisterObjectTransformation<Post>(obj => new
                      {
                          PostId = obj.PostId,
                          Author = obj.Author,
                          Subject = obj.Subject,
                          Message = obj.Message,
                          Creation = obj.CreationDate
                      });

                      builder.RegisterCollectionTransformations(
                          (BoardBase obj) => new
                          {
                              Key = obj.Key,
                              Name = obj.Name,
                              Posts = obj.Posts.Count
                          },
                          (MailBox obj) => new
                          {
                              Key = obj.Key,
                              Name = obj.Name,
                              Posts = obj.Posts.Count
                          },
                          (BulletinBoard obj) => new
                          {
                              Key = obj.Key,
                              Name = obj.Name,
                              Posts = obj.Posts.Count
                          },
                          (Group obj) => new
                          {
                              Id = obj.Id,
                              LeaderName = obj.Leader.Name,
                              MemberCount = obj.Count
                          });
                  });
    }
}