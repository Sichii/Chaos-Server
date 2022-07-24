using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Chaos.Caches;
using Chaos.Caches.Interfaces;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Cryptography.Extensions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Factories;
using Chaos.Factories.Interfaces;
using Chaos.Geometry.JsonConverters;
using Chaos.Managers;
using Chaos.Managers.Interfaces;
using Chaos.Mappers;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Objects.World;
using Chaos.Options;
using Chaos.Packets.Extensions;
using Chaos.Servers;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddSingleton<IItemScriptFactory, ItemScriptFactory>();
        services.AddSingleton<ISkillScriptFactory, SkillScriptFactory>();
        services.AddSingleton<ISpellScriptFactory, SpellScriptFactory>();
        services.AddSingleton<IEffectFactory, EffectFactory>();
        services.AddTransient<IExchangeFactory, ExchangeFactory>();
        services.AddTransient<IClientFactory<ILobbyClient>, LobbyClientFactory>();
        services.AddTransient<IClientFactory<ILoginClient>, LoginClientFactory>();
        services.AddTransient<IClientFactory<IWorldClient>, WorldClientFactory>();
        services.AddTransient<IItemFactory, ItemFactory>();
        services.AddTransient<ISkillFactory, SkillFactory>();
        services.AddTransient<ISpellFactory, SpellFactory>();
        services.AddTransient<IWorldObjectFactory, WorldObjectFactory>();
    }

    /// <summary>
    ///     Configure all manager objects
    /// </summary>
    public void ConfigureManagers(IServiceCollection services)
    {
        services.AddSingleton<IRedirectManager, RedirectManager>();
        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
        services.AddSingleton<ISaveManager<Aisling>, UserSaveManager>();
    }

    /// <summary>
    ///     Configure automappers for converting between serializable objects and normal objects
    /// </summary>
    public void ConfigureMappings(IServiceCollection services)
    {
        //some of these mappers use injected cache managers to help with the mapping
        //so they must be added to the service provider, so that those managers can be resolved
        services.AddTransient<EffectMapper>();
        services.AddTransient<ItemMapper>();
        services.AddTransient<EquipmentMapper>();
        services.AddTransient<BankMapper>();
        services.AddTransient<LegendMapper>();
        services.AddTransient<SkillMapper>();
        services.AddTransient<SpellMapper>();
        services.AddTransient<UserMapper>();
        services.AddTransient<UserOptionsMapper>();

        services.AddSingleton<IMapper>(
            provider =>
            {
                var effectMapper = provider.GetRequiredService<EffectMapper>();
                var itemMapper = provider.GetRequiredService<ItemMapper>();
                var equipmentMapper = provider.GetRequiredService<EquipmentMapper>();
                var bankMapper = provider.GetRequiredService<BankMapper>();
                var legendMapper = provider.GetRequiredService<LegendMapper>();
                var skillMapper = provider.GetRequiredService<SkillMapper>();
                var spellMapper = provider.GetRequiredService<SpellMapper>();
                var userOptionsMapper = provider.GetRequiredService<UserOptionsMapper>();
                var userMapper = provider.GetRequiredService<UserMapper>();

                //the profiles must be added this way
                //otherwise AutoMapper will complain about the profiles not having a parameterless constructor
                var mapperConfig = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddCollectionMappers();
                        cfg.AddProfile(effectMapper);
                        cfg.AddProfile(itemMapper);
                        cfg.AddProfile(equipmentMapper);
                        cfg.AddProfile(bankMapper);
                        cfg.AddProfile(legendMapper);
                        cfg.AddProfile(skillMapper);
                        cfg.AddProfile(spellMapper);
                        cfg.AddProfile(userOptionsMapper);
                        cfg.AddProfile(userMapper);
                    });

                return mapperConfig.CreateMapper(provider.GetRequiredService);
            });
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

        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);
        
        services.AddPacketSerialization();
        services.AddCryptography();
        services.AddHostedService<LobbyServer>();
        services.AddHostedService<LoginServer>();
        services.AddHostedService<WorldServer>();
        
        ConfigureOptions(services);
        ConfigureCaches(services);
        ConfigureManagers(services);
        ConfigureMappings(services);
        ConfigureFactories(services);
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