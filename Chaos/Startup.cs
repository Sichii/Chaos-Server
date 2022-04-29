using System.Diagnostics.CodeAnalysis;
using System.Text;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Chaos.Clients;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Cryptography;
using Chaos.Cryptography.Interfaces;
using Chaos.DataObjects;
using Chaos.Effects.Interfaces;
using Chaos.Extensions;
using Chaos.Factories;
using Chaos.Factories.Interfaces;
using Chaos.Managers;
using Chaos.Managers.Interfaces;
using Chaos.Mappers;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Options;
using Chaos.Packets;
using Chaos.Packets.Interfaces;
using Chaos.Servers;
using Chaos.Servers.Interfaces;
using Chaos.Templates;
using Chaos.WorldObjects;
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
    ///     Configure all clients and their factories
    /// </summary>
    public void ConfigureFactories(IServiceCollection services)
    {
        services.AddTransient<ICryptoClient, CryptoClient>();
        services.AddSingleton<IClientFactory<ILobbyClient>, LobbyClientFactory>();
        services.AddSingleton<IClientFactory<ILoginClient>, LoginClientFactory>();
        services.AddSingleton<IClientFactory<IWorldClient>, WorldClientFactory>();
        services.AddSingleton<IUserFactory, UserFactory>();
    }

    /// <summary>
    ///     Configure all manager objects
    /// </summary>
    public void ConfigureManagers(IServiceCollection services)
    {
        services.AddSingleton<IRedirectManager, RedirectManager>();
        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
        services.AddSingleton<ICacheManager<string, IEffect>, EffectManager>();
        services.AddSingleton<ICacheManager<string, ItemTemplate>, ItemTemplateManager>();
        services.AddSingleton<ICacheManager<string, SkillTemplate>, SkillTemplateManager>();
        services.AddSingleton<ICacheManager<string, SpellTemplate>, SpellTemplateManager>();
        services.AddSingleton<ICacheManager<short, MapTemplate>, MapTemplateManager>();
        services.AddSingleton<ICacheManager<string, MapInstance>, MapInstanceManager>();
        services.AddSingleton<ICacheManager<string, Metafile>, MetafileManager>();
        services.AddSingleton<ISaveManager<User>, UserSaveManager>();
    }

    /// <summary>
    ///     Configure automappers for converting between serializable objects and normal objects
    /// </summary>
    public void ConfigureMappings(IServiceCollection services)
    {
        //some of these mappers use injected cache managers to help with the mapping
        //so they must be added to the service provider, so that those managers can be resolved
        services.AddSingleton<EffectMapper>();
        services.AddSingleton<ItemMapper>();
        services.AddSingleton<EquipmentMapper>();
        services.AddSingleton<BankMapper>();
        services.AddSingleton<LegendMapper>();
        services.AddSingleton<SkillMapper>();
        services.AddSingleton<SpellMapper>();
        services.AddSingleton<UserMapper>();
        services.AddSingleton<UserOptionsMapper>();

        services.AddSingleton<IMapper>(provider =>
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
            var mapperConfig = new MapperConfiguration(cfg =>
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
    ///     Configure options objects used by other DI implementations. These are effectively serialized out of the config
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
            .PostConfigure(LoginOptions.PostConfigure);

        services.AddOptionsFromConfig<WorldOptions>(ConfigKeys.Options.Key)
            .PostConfigure(WorldOptions.PostConfigure);
    }

    /// <summary>
    ///     Configure game servers
    /// </summary>
    public void ConfigureServers(IServiceCollection services)
    {
        services.AddSingleton<ILobbyServer, LobbyServer>();
        services.AddSingleton<ILoginServer, LoginServer>();
        services.AddSingleton<IWorldServer, WorldServer>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        services.AddLogging(logging =>
        {
            logging.AddNLog(new NLogLoggingConfiguration(Configuration.GetRequiredSection(ConfigKeys.Logging.NLog.Key)));
        });

        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);

        var codePage = Configuration.GetValue<int>(ConfigKeys.Options.CodePage);
        var encoding = Encoding.GetEncoding(codePage);
        services.AddSingleton(encoding);

        ConfigureOptions(services);

        services.AddSingleton<IPacketSerializer, PacketSerializer>();

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