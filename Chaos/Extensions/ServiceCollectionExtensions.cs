using Chaos.Clients.Abstractions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Extensions.DependencyInjection;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using Chaos.Scripting;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.MapperProfiles;
using Chaos.Services.Servers;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Mutators;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chaos.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFunctionalScriptRegistry(this IServiceCollection services) =>
        services.AddSingleton<IScriptRegistry, FunctionalScriptRegistry>(
            p =>
            {
                var registry = new FunctionalScriptRegistry(p);

                var scriptTypes = typeof(IFunctionalScript).LoadImplementations();

                foreach (var type in scriptTypes)
                    registry.Register(ScriptBase.GetScriptKey(type), type);

                return registry;
            });

    public static void AddLobbyServer(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<ILobbyClient>, LobbyClientFactory>();
        services.AddSingleton<IClientRegistry<ILobbyClient>, ClientRegistry<ILobbyClient>>();

        services.AddOptionsFromConfig<LobbyOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure(LobbyOptions.PostConfigure)
                .Validate<ILogger<LobbyOptions>>(LobbyOptions.Validate);

        services.AddSingleton<ILobbyServer<ILobbyClient>, IHostedService, LobbyServer>();
    }

    public static void AddLoginserver(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<ILoginClient>, LoginClientFactory>();
        services.AddSingleton<IClientRegistry<ILoginClient>, ClientRegistry<ILoginClient>>();

        services.AddOptionsFromConfig<LoginOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure<ILogger<LoginOptions>>(LoginOptions.PostConfigure);

        services.AddSingleton<ILoginServer<ILoginClient>, IHostedService, LoginServer>();
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

        services.AddTransient<IScriptProvider, ScriptProvider>();
        services.AddTransient<ICloningService<Item>, ItemCloningService>();
    }

    public static void AddServerAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IRedirectManager, RedirectManager>();
        services.AddSecurity(Startup.ConfigKeys.Options.Key);
    }

    public static void AddStorage(this IServiceCollection services)
    {
        services.AddDirectoryBoundOptionsFromConfig<UserSaveManagerOptions>(Startup.ConfigKeys.Options.Key);
        services.AddSingleton<ISaveManager<Aisling>, IHostedService, UserSaveManager>();

        services.AddExpiringCache<ItemTemplate, ItemTemplateSchema, ItemTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCache<SkillTemplate, SkillTemplateSchema, SkillTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCache<SpellTemplate, SpellTemplateSchema, SpellTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);

        services.AddExpiringCache<MonsterTemplate, MonsterTemplateSchema, MonsterTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCache<MerchantTemplate, MerchantTemplateSchema, MerchantTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);

        services.AddExpiringCache<LootTable, LootTableSchema, LootTableCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCache<DialogTemplate, DialogTemplateSchema, DialogTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);

        services.AddExpiringCache<WorldMap, WorldMapSchema, WorldMapCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCache<WorldMapNode, WorldMapNodeSchema, WorldMapNodeCacheOptions>(Startup.ConfigKeys.Options.Key);

        services.AddExpiringCache<ReactorTileTemplate, ReactorTileTemplateSchema, ReactorTileTemplateCacheOptions>(
            Startup.ConfigKeys.Options.Key);

        services.AddExpiringCacheImpl<MapTemplate, ExpiringMapTemplateCache, MapTemplateCacheOptions>(Startup.ConfigKeys.Options.Key);
        services.AddExpiringCacheImpl<MapInstance, ExpiringMapInstanceCache, MapInstanceCacheOptions>(Startup.ConfigKeys.Options.Key);

        services.AddDirectoryBoundOptionsFromConfig<MetaDataCacheOptions>(Startup.ConfigKeys.Options.Key)
                .Configure(
                    o =>
                    {
                        o.PrefixMutators.Add(new EnchantmentMetaNodeMutator());
                    });

        services.AddSingleton<IMetaDataCache, MetaDataCache>();

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
        services.AddTransient<IPanelObjectFactory<Item>, IItemFactory, ItemFactory>();
        services.AddTransient<IPanelObjectFactory<Skill>, ISkillFactory, SkillFactory>();
        services.AddTransient<IPanelObjectFactory<Spell>, ISpellFactory, SpellFactory>();
        services.AddTransient<IReactorTileFactory, ReactorTileFactory>();
        services.AddTransient<IMonsterFactory, MonsterFactory>();
        services.AddTransient<IMerchantFactory, MerchantFactory>();
        services.AddTransient<IDialogFactory, DialogFactory>();
        services.AddSingleton<IEffectFactory, EffectFactory>();
        services.AddTransient<IExchangeFactory, ExchangeFactory>();
        services.AddTransient<IClientProvider, ClientProvider>();
    }

    public static void AddWorldServer(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<IWorldClient>, WorldClientFactory>();
        services.AddSingleton<IClientRegistry<IWorldClient>, ClientRegistry<IWorldClient>>();

        services.AddOptionsFromConfig<WorldOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure(WorldOptions.PostConfigure);

        services.AddSingleton<IWorldServer<IWorldClient>, IHostedService, WorldServer>();
    }
}