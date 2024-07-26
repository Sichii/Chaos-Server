using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AutoMapper.EquivalencyExpression;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Common.Configuration;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.DependencyInjection;
using Chaos.Geometry.JsonConverters;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.WorldMap;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using Chaos.Scripting;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.EffectScripts;
using Chaos.Scripting.FunctionalScripts;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Configuration;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.MapperProfiles;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Options;
using ItemMapperProfile = Chaos.Site.Services.MapperProfiles.ItemMapperProfile;
using SkillMapperProfile = Chaos.Site.Services.MapperProfiles.SkillMapperProfile;
using SpellMapperProfile = Chaos.Site.Services.MapperProfiles.SpellMapperProfile;
using MonsterMapperProfile = Chaos.Site.Services.MapperProfiles.MonsterMapperProfile;

namespace Chaos.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly SerializationContext JsonContext;
    private static readonly JsonSerializerOptions JsonSerializerOptions;
    private static bool IsInitialized { get; set; }

    static ServiceCollectionExtensions()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        JsonSerializerOptions.Converters.Add(new PointConverter());
        JsonSerializerOptions.Converters.Add(new LocationConverter());
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        JsonContext = new SerializationContext(JsonSerializerOptions);
    }

    public static void AddChaosOptions(this IServiceCollection services)
    {
        services.AddOptions();
        services.ConfigureOptions<OptionsConfigurer>();
        services.ConfigureOptions<OptionsValidator>();

        services.AddOptionsFromConfig<ChaosOptions>(ConfigKeys.Options.Key);
        services.AddOptionsFromConfig<SiteOptions>(ConfigKeys.Options.Key);

        services.AddSingleton<IStagingDirectory, ChaosOptions>(
            p => p.GetRequiredService<IOptionsSnapshot<ChaosOptions>>()
                  .Value);
    }

    public static void AddFunctionalScriptRegistry(this IServiceCollection services)
        => services.AddSingleton<IScriptRegistry, FunctionalScriptRegistry>(
            p =>
            {
                var registry = new FunctionalScriptRegistry(p);

                var scriptTypes = typeof(IFunctionalScript).LoadImplementations();

                foreach (var type in scriptTypes)
                    registry.Register(ScriptBase.GetScriptKey(type), type);

                return registry;
            });

    public static void AddJsonSerializerOptions(this IServiceCollection services)
        => services.AddOptions<JsonSerializerOptions>()
                   .Configure<ILogger<WarningJsonTypeInfoResolver>>(
                       (options, logger) =>
                       {
                           if (!IsInitialized)
                           {
                               IsInitialized = true;
                               var defaultResolver = new WarningJsonTypeInfoResolver(logger);
                               var combinedResolver = JsonTypeInfoResolver.Combine(JsonContext, defaultResolver);

                               JsonSerializerOptions.SetTypeResolver(combinedResolver);
                           }

                           ShallowCopy<JsonSerializerOptions>.Merge(JsonSerializerOptions, options);
                       });

    public static void AddLobbyServer(this IServiceCollection services)
    {
        services.AddSingleton<IClientRegistry<IChaosLobbyClient>, ClientRegistry<IChaosLobbyClient>>();

        services.AddOptionsFromConfig<LobbyOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<ILobbyServer<IChaosLobbyClient>, IHostedService, LobbyServer>();
    }

    public static void AddLoginserver(this IServiceCollection services)
    {
        services.AddSingleton<IClientRegistry<IChaosLoginClient>, ClientRegistry<IChaosLoginClient>>();

        services.AddOptionsFromConfig<LoginOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<ILoginServer<IChaosLoginClient>, IHostedService, LoginServer>();
    }

    public static void AddScripting(this IServiceCollection services)
    {
        services.AddScriptFactory<IItemScript, Item>();
        services.AddScriptFactory<ISkillScript, Skill>();
        services.AddScriptFactory<ISpellScript, Spell>();

        services.AddScriptFactory<IMonsterScript, Monster>();
        services.AddScriptFactory<IMerchantScript, Merchant>();
        services.AddScriptFactory<IAislingScript, Aisling>();

        services.AddScriptFactory<IMapScript, MapInstance>();
        services.AddScriptFactory<IReactorTileScript, ReactorTile>();

        services.AddScriptFactory<IDialogScript, Dialog>();
        services.AddScriptFactory<IBulletinBoardScript, BulletinBoard>();

        services.AddTransient<IScriptProvider, ScriptProvider>();
        services.AddTransient<ICloningService<Item>, ItemCloningService>();
    }

    public static void AddServerAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IRedirectManager, IHostedService, RedirectManager>();
        services.AddSecurity(ConfigKeys.Options.Key);
    }

    public static void AddSiteDtoMappings(this IServiceCollection services)
    {
        services.AddSingleton<SkillMapperProfile>();
        services.AddSingleton<SpellMapperProfile>();
        services.AddSingleton<MonsterMapperProfile>();

        services.AddAutoMapper(
            (provider, cfg) =>
            {
                cfg.AddCollectionMappers();
                cfg.AddProfile<ItemMapperProfile>();
                cfg.AddProfile(provider.GetRequiredService<SkillMapperProfile>());
                cfg.AddProfile(provider.GetRequiredService<SpellMapperProfile>());
                cfg.AddProfile(provider.GetRequiredService<MonsterMapperProfile>());
            },
            Array.Empty<Type>());
    }

    public static void AddStorage(this IServiceCollection services)
    {
        services.AddTransient<IEntityRepository, EntityRepository>();

        //add mail store with backup service
        services.AddOptionsFromConfig<MailStoreOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<IStore<MailBox>, IHostedService, MailStore>();
        services.AddHostedService<DirectoryBackupService<MailStoreOptions>>();
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<MailStoreOptions>>();

        //add guild store with backup service
        services.AddOptionsFromConfig<GuildStoreOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<IStore<Guild>, IHostedService, GuildStore>();
        services.AddHostedService<DirectoryBackupService<GuildStoreOptions>>();
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<GuildStoreOptions>>();

        //add bulletinboard store with backup service
        services.AddOptionsFromConfig<BulletinBoardStoreOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<IStore<BulletinBoard>, IHostedService, BulletinBoardStore>();
        services.AddHostedService<DirectoryBackupService<BulletinBoardStoreOptions>>();
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<BulletinBoardStoreOptions>>();

        //add aisling store with backup service
        services.AddOptionsFromConfig<AislingStoreOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<IAsyncStore<Aisling>, IStore<Aisling>, AislingStore>();
        services.AddHostedService<DirectoryBackupService<AislingStoreOptions>>();
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<AislingStoreOptions>>();

        //add metadata store
        services.AddOptionsFromConfig<MetaDataStoreOptions>(ConfigKeys.Options.Key); //bound
        services.AddSingleton<IMetaDataStore, MetaDataStore>();

        //add bulletinboard key mapper service
        services.AddSingleton<BulletinBoardKeyMapper>();

        //add caches
        services.AddExpiringCache<ItemTemplate, ItemTemplateSchema, ItemTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCache<SkillTemplate, SkillTemplateSchema, SkillTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCache<SpellTemplate, SpellTemplateSchema, SpellTemplateCacheOptions>(ConfigKeys.Options.Key);

        services.AddExpiringCache<MonsterTemplate, MonsterTemplateSchema, MonsterTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCache<MerchantTemplate, MerchantTemplateSchema, MerchantTemplateCacheOptions>(ConfigKeys.Options.Key);

        services.AddExpiringCache<LootTable, LootTableSchema, LootTableCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCache<DialogTemplate, DialogTemplateSchema, DialogTemplateCacheOptions>(ConfigKeys.Options.Key);

        services.AddExpiringCache<WorldMap, WorldMapSchema, WorldMapCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCache<WorldMapNode, WorldMapNodeSchema, WorldMapNodeCacheOptions>(ConfigKeys.Options.Key);

        services.AddExpiringCache<BulletinBoardTemplate, BulletinBoardTemplateSchema, BulletinBoardTemplateCacheOptions>(
            ConfigKeys.Options.Key);

        services.AddExpiringCache<ReactorTileTemplate, ReactorTileTemplateSchema, ReactorTileTemplateCacheOptions>(ConfigKeys.Options.Key);

        //add custom cache implementations
        services.AddExpiringCacheImpl<MapTemplate, ExpiringMapTemplateCache, MapTemplateCacheOptions>(ConfigKeys.Options.Key);
        services.AddExpiringCacheImpl<MapInstance, ExpiringMapInstanceCache, MapInstanceCacheOptions>(ConfigKeys.Options.Key);

        //add cache locator
        services.AddSingleton<ISimpleCache, ISimpleCacheProvider, SimpleCache>();
    }

    public static void AddTransient<TI1, TI2, T>(this IServiceCollection services) where T: class, TI1, TI2
                                                                                   where TI1: class
                                                                                   where TI2: class
    {
        services.AddTransient<TI1, T>();
        services.AddTransient<TI2, T>();
    }

    public static void AddWorldFactories(this IServiceCollection services)
    {
        services.AddTransient<IPanelEntityFactory<Item>, IItemFactory, ItemFactory>();
        services.AddTransient<IPanelEntityFactory<Skill>, ISkillFactory, SkillFactory>();
        services.AddTransient<IPanelEntityFactory<Spell>, ISpellFactory, SpellFactory>();
        services.AddTransient<IReactorTileFactory, ReactorTileFactory>();
        services.AddTransient<IMonsterFactory, MonsterFactory>();
        services.AddTransient<IMerchantFactory, MerchantFactory>();
        services.AddTransient<IDialogFactory, DialogFactory>();
        
        services.AddSingleton<IEffectFactory, EffectFactory>();

        services.AddSimpleFactory<Guild>(typeof(string), typeof(string));
        services.AddSimpleFactory<IChaosLobbyClient, ChaosLobbyClient>(typeof(Socket));
        services.AddSimpleFactory<IChaosLoginClient, ChaosLoginClient>(typeof(Socket));
        services.AddSimpleFactory<IChaosWorldClient, ChaosWorldClient>(typeof(Socket));
        services.AddSimpleFactory<Exchange>(typeof(Aisling), typeof(Aisling));
        services.AddSimpleFactory<MailBox>(typeof(string));
    }

    public static void AddWorldServer(this IServiceCollection services)
    {
        services.AddSingleton<IGroupService, GroupService>();
        services.AddSingleton<IClientRegistry<IChaosWorldClient>, WorldClientRegistry>();

        services.AddOptionsFromConfig<WorldOptions>(ConfigKeys.Options.Key);
        services.AddSingleton<IWorldServer<IChaosWorldClient>, IHostedService, WorldServer>();
    }

    public static void ConfigureSite(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.AddSingleton<QueryService>();
        builder.Services.AddSingleton<ItemDtoRepository>();
        builder.Services.AddSingleton<SkillDtoRepository>();
        builder.Services.AddSingleton<SpellDtoRepository>();
        builder.Services.AddSingleton<MonsterDtoRepository>();
        builder.Services.AddSiteDtoMappings();
    }

    public static async Task RunHostedServicesAsync(this IServiceProvider provider, CancellationToken cancellationToken)
    {
        var hostedServices = provider.GetServices<IHostedService>();

        var startFuncs = hostedServices.Select<IHostedService, Func<CancellationToken, Task>>(s => s.StartAsync)
                                       .ToArray();

        await cancellationToken.WhenAllWithCancellation(startFuncs);
    }
}