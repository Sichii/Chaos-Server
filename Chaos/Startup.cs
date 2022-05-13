using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Chaos.Caches;
using Chaos.Caches.Interfaces;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Containers.Interfaces;
using Chaos.Core.Data;
using Chaos.Core.JsonConverters;
using Chaos.Cryptography;
using Chaos.Cryptography.Interfaces;
using Chaos.Effects.Interfaces;
using Chaos.Extensions;
using Chaos.Factories;
using Chaos.Factories.Interfaces;
using Chaos.Managers;
using Chaos.Managers.Interfaces;
using Chaos.Mappers;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Options;
using Chaos.Packets;
using Chaos.Packets.Interfaces;
using Chaos.Servers;
using Chaos.Servers.Interfaces;
using Chaos.Templates;
using Chaos.Utilities;
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
    ///     Configure all factory objects
    /// </summary>
    public void ConfigureFactories(IServiceCollection services)
    {
        services.AddSingleton<IItemScriptFactory, ItemScriptFactory>();
        services.AddSingleton<ISkillScriptFactory, SkillScriptFactory>();
        services.AddSingleton<ISpellScriptFactory, SpellScriptFactory>();
        services.AddTransient<IExchangeFactory, ExchangeFactory>();
        services.AddTransient<IClientFactory<ILobbyClient>, LobbyClientFactory>();
        services.AddTransient<IClientFactory<ILoginClient>, LoginClientFactory>();
        services.AddTransient<IClientFactory<IWorldClient>, WorldClientFactory>();
        services.AddTransient<IItemFactory, ItemFactory>();
        services.AddTransient<ISkillFactory, SkillFactory>();
        services.AddTransient<ISpellFactory, SpellFactory>();
    }

    /// <summary>
    ///     Configure all cache objects
    /// </summary>
    public void ConfigureCaches(IServiceCollection services)
    {
        services.AddSingleton<ISimpleCache<string, IEffect>, EffectCache>();
        services.AddSingleton<ISimpleCache<string, ItemTemplate>, ItemTemplateCache>();
        services.AddSingleton<ISimpleCache<string, SkillTemplate>, SkillTemplateCache>();
        services.AddSingleton<ISimpleCache<string, SpellTemplate>, SpellTemplateCache>();
        services.AddSingleton<ISimpleCache<string, MapTemplate>, MapTemplateCache>();
        services.AddSingleton<ISimpleCache<string, MapInstance>, MapInstanceCache>();
        services.AddSingleton<ISimpleCache<string, Metafile>, MetafileCache>();
    }
    
    /// <summary>
    ///     Configure all manager objects
    /// </summary>
    public void ConfigureManagers(IServiceCollection services)
    {
        services.AddSingleton<IRedirectManager, RedirectManager>();
        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
        services.AddSingleton<ISaveManager<User>, UserSaveManager>();
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

        services.AddOptionsFromConfig<ItemTemplateManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<SkillTemplateManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<SpellTemplateManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<UserSaveManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MapTemplateManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MapInstanceManagerOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<MetafileManagerOptions>(ConfigKeys.Options.Key);

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

    /// <summary>
    ///     Configure game servers
    /// </summary>
    public void ConfigureServers(IServiceCollection services)
    {
        services.AddScoped<ILobbyServer, LobbyServer>();
        services.AddScoped<ILoginServer, LoginServer>();
        services.AddScoped<IWorldServer, WorldServer>();
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

        var codePage = Configuration.GetValue<int>(ConfigKeys.Options.CodePage);
        var encoding = Encoding.GetEncoding(codePage);
        services.AddSingleton(encoding);
        services.AddSingleton<ItemUtility>();

        ConfigureOptions(services);

        services.AddSingleton<IPacketSerializer, PacketSerializer>();
        services.AddTransient<ICryptoClient, CryptoClient>();

        ConfigureCaches(services);
        ConfigureManagers(services);
        ConfigureMappings(services);
        ConfigureFactories(services);
        ConfigureServers(services);
    }
    
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class ConfigKeys
    {
        public static class Options
        {
            public static string CodePage => $"{Key}:CodePage";
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