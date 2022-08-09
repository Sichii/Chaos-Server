using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Cryptography.Extensions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Geometry.JsonConverters;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;
using Chaos.Packets.Extensions;
using Chaos.Pathfinding.Extensions;
using Chaos.Scripts.Interfaces;
using Chaos.Services.Caches;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Caches.Options;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Hosted;
using Chaos.Services.Hosted.Interfaces;
using Chaos.Services.Hosted.Options;
using Chaos.Services.Providers;
using Chaos.Services.Providers.Interfaces;
using Chaos.Services.Serialization;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Services.Serialization.Options;
using Chaos.Services.Utility;
using Chaos.Services.Utility.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Chaos;

public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    /// <summary>
    ///     Configure all cache objects
    /// </summary>
    public void ConfigureCaches(IServiceCollection services)
    {
        services.AddSingleton<ISimpleCache<ItemTemplate>, ItemTemplateCache>();
        services.AddSingleton<ISimpleCache<SkillTemplate>, SkillTemplateCache>();
        services.AddSingleton<ISimpleCache<SpellTemplate>, SpellTemplateCache>();
        services.AddSingleton<ISimpleCache<MapTemplate>, MapTemplateCache>();
        services.AddSingleton<ISimpleCache<MapInstance>, MapInstanceCache>();
        services.AddSingleton<ISimpleCache<Metafile>, MetafileCache>();
    }

    /// <summary>
    ///     Configure all factory objects
    /// </summary>
    public void ConfigureFactories(IServiceCollection services)
    {
        services.AddSingleton<IScriptFactory<IItemScript, Item>, IItemScriptFactory, ItemScriptFactory>();
        services.AddSingleton<IScriptFactory<ISkillScript, Skill>, ISkillScriptFactory, SkillScriptFactory>();
        services.AddSingleton<IScriptFactory<ISpellScript, Spell>, ISpellScriptFactory, SpellScriptFactory>();
        services.AddSingleton<IEffectFactory, EffectFactory>();

        services.AddTransient<IExchangeFactory, ExchangeFactory>();
        services.AddTransient<IPanelObjectFactory<Item>, IItemFactory, ItemFactory>();
        services.AddTransient<IPanelObjectFactory<Skill>, ISkillFactory, SkillFactory>();
        services.AddTransient<IPanelObjectFactory<Spell>, ISpellFactory, SpellFactory>();
        services.AddTransient<IClientFactory<ILobbyClient>, LobbyClientFactory>();
        services.AddTransient<IClientFactory<ILoginClient>, LoginClientFactory>();
        services.AddTransient<IClientFactory<IWorldClient>, WorldClientFactory>();
    }

    /// <summary>
    ///     Configure options objects used by other DI implementations. These are generally serialized out of the config
    /// </summary>
    public void ConfigureOptions(IServiceCollection services)
    {
        services.AddOptions();

        services.AddOptionsFromConfig<ActiveDirectoryCredentialManagerOptions>(ConfigKeys.Options.Key)
                .PostConfigure(ActiveDirectoryCredentialManagerOptions.PostConfigure);

        services.AddOptionsFromConfig<ItemTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<SkillTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<SpellTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<UserSaveManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MapTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MapInstanceCacheOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MetafileCacheOptions>(ConfigKeys.Options.Key);

        services.AddOptionsFromConfig<LobbyOptions>(ConfigKeys.Options.Key)
                .PostConfigure(LobbyOptions.PostConfigure)
                .Validate<ILogger<LobbyOptions>>(LobbyOptions.Validate);

        services.AddOptionsFromConfig<LoginOptions>(ConfigKeys.Options.Key)
                .PostConfigure<ILogger<LoginOptions>>(LoginOptions.PostConfigure);

        services.AddOptionsFromConfig<WorldOptions>(ConfigKeys.Options.Key)
                .PostConfigure(WorldOptions.PostConfigure);

        services.AddOptions<JsonSerializerOptions>()
                .Configure(
                    o =>
                    {
                        o.WriteIndented = true;
                        o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                        o.PropertyNameCaseInsensitive = true;
                        o.IgnoreReadOnlyProperties = true;
                        o.IgnoreReadOnlyFields = true;
                        o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        o.AllowTrailingCommas = true;
                        o.Converters.Add(new PointConverter());
                        o.Converters.Add(new JsonStringEnumConverter());
                    });
    }

    public void ConfigureProviders(IServiceCollection services)
    {
        services.AddTransient<IScriptFactoryProvider, ScriptFactoryProvider>();
        services.AddTransient<IPanelObjectFactoryProvider, PanelObjectFactoryProvider>();
        services.AddTransient<ISerialTransformProvider, SerialTransformProvider>();
        services.AddTransient<ISimpleCacheProvider, SimpleCacheProvider>();
    }

    /// <summary>
    ///     Configure all manager objects
    /// </summary>
    public void ConfigureSecurity(IServiceCollection services)
    {
        services.AddCryptography();
        services.AddSingleton<IRedirectManager, RedirectManager>();
        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
    }

    public void ConfigureSerialization(IServiceCollection services)
    {
        services.AddPacketSerialization();
        services.AddSingleton<ISaveManager<Aisling>, UserSaveManager>();
        services.AddTransient<ISerialTransformService<Aisling, SerializableAisling>, AislingSerialTransformService>();
        services.AddTransient<ISerialTransformService<Item, SerializableItem>, ItemSerialTransformService>();
        services.AddTransient<ISerialTransformService<Skill, SerializableSkill>, SkillSerialTransformService>();
        services.AddTransient<ISerialTransformService<Spell, SerializableSpell>, SpellSerialTransformService>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        services.AddLogging(
            logging =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(Configuration.GetSection(ConfigKeys.Logging.Key));

                logging.AddNLog(
                    Configuration,
                    new NLogProviderOptions
                    {
                        LoggingConfigurationSectionName = ConfigKeys.Logging.NLog.Key
                    });
            });

        services.AddPathfinding();

        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);

        services.AddSingleton<ILobbyServer, IHostedService, LobbyServer>();
        services.AddSingleton<ILoginServer, IHostedService, LoginServer>();
        services.AddSingleton<IWorldServer, IHostedService, WorldServer>();
        services.AddTransient<ICloningService<Item>, ItemCloningService>();

        ConfigureOptions(services);
        ConfigureSerialization(services);
        ConfigureCaches(services);
        ConfigureSecurity(services);
        ConfigureFactories(services);
        ConfigureProviders(services);
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class ConfigKeys
    {
        public static class Options
        {
            public static string Key => "Options";
        }

        public static class Logging
        {
            public static string Key => "Logging";

            public static class NLog
            {
                public static string Key => $"{Logging.Key}:NLog";
            }
        }
    }
}