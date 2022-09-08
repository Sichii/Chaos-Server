using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Clients.Abstractions;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Geometry.JsonConverters;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;
using Chaos.Services.Caches;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Hosted;
using Chaos.Services.Hosted.Abstractions;
using Chaos.Services.Hosted.Options;
using Chaos.Services.Mappers;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Scripting;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Services.Security;
using Chaos.Services.Security.Abstractions;
using Chaos.Services.Security.Options;
using Chaos.Services.Serialization;
using Chaos.Services.Serialization.Abstractions;
using Chaos.Services.Serialization.Options;
using Chaos.Services.Utility;
using Chaos.Services.Utility.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAislingSerialization(this IServiceCollection services)
    {
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

        services.AddOptionsFromConfig<UserSaveManagerOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure<IOptionsSnapshot<ChaosOptions>>(UserSaveManagerOptions.PostConfigure);

        services.AddTransient<ISaveManager<Aisling>, UserSaveManager>();
    }

    public static void AddLobbyServer(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<ILobbyClient>, LobbyClientFactory>();

        services.AddOptionsFromConfig<LobbyOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure(LobbyOptions.PostConfigure)
                .Validate<ILogger<LobbyOptions>>(LobbyOptions.Validate);

        services.AddSingleton<ILobbyServer, IHostedService, LobbyServer>();
    }

    public static void AddLoginserver(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<ILoginClient>, LoginClientFactory>();

        services.AddOptionsFromConfig<LoginOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure<ILogger<LoginOptions>>(LoginOptions.PostConfigure);

        services.AddSingleton<ILoginServer, IHostedService, LoginServer>();
    }

    public static OptionsBuilder<T> AddOptionsFromConfig<T>(this IServiceCollection services, string? subSection = null) where T: class
    {
        var typeName = typeof(T).Name;
        var path = typeName;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{typeName}";

        return services.AddOptions<T>()
                       .Configure<IConfiguration>(
                           (o, c) => c.GetSection(path).Bind(o, options => options.ErrorOnUnknownConfiguration = true));
    }

    public static void AddScripting(this IServiceCollection services)
    {
        services.AddSingleton<IScriptFactory<IItemScript, Item>, ScriptFactory<IItemScript, Item>>();
        services.AddSingleton<IScriptFactory<ISkillScript, Skill>, ScriptFactory<ISkillScript, Skill>>();
        services.AddSingleton<IScriptFactory<ISpellScript, Spell>, ScriptFactory<ISpellScript, Spell>>();
        services.AddSingleton<IScriptFactory<IMonsterScript, Monster>, ScriptFactory<IMonsterScript, Monster>>();
        services.AddSingleton<IScriptFactory<IMapScript, MapInstance>, ScriptFactory<IMapScript, MapInstance>>();
        services.AddTransient<ICloningService<Item>, ItemCloningService>();

        services.AddTransient<IScriptProvider, ScriptProvider>();
    }

    public static void AddServerAuthentication(this IServiceCollection services)
    {
        services.AddOptionsFromConfig<ActiveDirectoryCredentialManagerOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure<IOptionsSnapshot<ChaosOptions>>(ActiveDirectoryCredentialManagerOptions.PostConfigure);

        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
        services.AddSingleton<IRedirectManager, RedirectManager>();
    }

    public static void AddSimpleCaches(this IServiceCollection services)
    {
        services.AddCacheOptions<ItemTemplateCacheOptions>();
        services.AddCacheOptions<SkillTemplateCacheOptions>();
        services.AddCacheOptions<SpellTemplateCacheOptions>();
        services.AddCacheOptions<MapTemplateCacheOptions>();
        services.AddCacheOptions<MapInstanceCacheOptions>();
        services.AddCacheOptions<MetafileCacheOptions>();
        services.AddCacheOptions<MonsterTemplateCacheOptions>();
        services.AddCacheOptions<LootTableCacheOptions>();

        services.AddSingleton<ISimpleCache<ItemTemplate>, ItemTemplateCache>();
        services.AddSingleton<ISimpleCache<SkillTemplate>, SkillTemplateCache>();
        services.AddSingleton<ISimpleCache<SpellTemplate>, SpellTemplateCache>();
        services.AddSingleton<ISimpleCache<MapTemplate>, MapTemplateCache>();
        services.AddSingleton<ISimpleCache<MapInstance>, MapInstanceCache>();
        services.AddSingleton<ISimpleCache<Metafile>, MetafileCache>();
        services.AddSingleton<ISimpleCache<MonsterTemplate>, MonsterTemplateCache>();
        services.AddSingleton<ISimpleCache<LootTable>, LootTableCache>();

        services.AddSingleton<ISimpleCache, SimpleCache>();
        services.AddSingleton<ISimpleCacheProvider, SimpleCache>();
    }

    public static void AddCacheOptions<T>(this IServiceCollection services, string? subSection = null) where T : class, IFileCacheOptions
    {
        subSection ??= Startup.ConfigKeys.Options.Key;

        services.AddOptionsFromConfig<T>(subSection)
                .PostConfigure<IOptionsSnapshot<ChaosOptions>>((o, co) => o.UseRootDirectory(co.Value.StagingDirectory))
                .Validate(o => !string.IsNullOrEmpty(o.Directory), "Directory must be set");
    }

    /// <summary>
    ///     Adds a singleton service that can be retreived via multiple base types
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <typeparam name="TI1">A base type of <typeparamref name="T" /></typeparam>
    /// <typeparam name="TI2">Another base type of <typeparamref name="T" /></typeparam>
    /// <typeparam name="T">An implementation of the previous two types</typeparam>
    public static void AddSingleton<TI1, TI2, T>(this IServiceCollection services) where T: class, TI1, TI2
                                                                                   where TI1: class
                                                                                   where TI2: class
    {
        services.AddSingleton<TI1, T>();
        services.AddSingleton<TI2, T>(p => (T)p.GetRequiredService<TI1>());
    }

    public static void AddTransient<TI1, TI2, T>(this IServiceCollection services) where T: class, TI1, TI2
                                                                                   where TI1: class
                                                                                   where TI2: class
    {
        services.AddTransient<TI1, T>();
        services.AddTransient<TI2, T>();
    }

    public static void AddTypeMappersFromAssembly(this IServiceCollection services)
    {
        var genericInterfaceType = typeof(IMapperProfile<,>);
        var typeMapperImplementations = TypeLoader.LoadImplementationsOfGeneric(genericInterfaceType);

        foreach (var implType in typeMapperImplementations)
            foreach (var iFaceType in implType.ExtractGenericInterfaces(genericInterfaceType))
                services.AddSingleton(iFaceType, implType);

        services.AddSingleton<ITypeMapper, TypeMapper>();
    }

    public static void AddWorldFactories(this IServiceCollection services)
    {
        services.AddTransient<IPanelObjectFactory<Item>, IItemFactory, ItemFactory>();
        services.AddTransient<IPanelObjectFactory<Skill>, ISkillFactory, SkillFactory>();
        services.AddTransient<IPanelObjectFactory<Spell>, ISpellFactory, SpellFactory>();
        services.AddTransient<IMonsterFactory, MonsterFactory>();
        services.AddSingleton<IEffectFactory, EffectFactory>();
        services.AddTransient<IExchangeFactory, ExchangeFactory>();

        services.AddTransient<IPanelObjectFactoryProvider, PanelObjectFactoryProvider>();
    }

    public static void AddWorldServer(this IServiceCollection services)
    {
        services.AddTransient<IClientFactory<IWorldClient>, WorldClientFactory>();

        services.AddOptionsFromConfig<WorldOptions>(Startup.ConfigKeys.Options.Key)
                .PostConfigure(WorldOptions.PostConfigure);

        services.AddSingleton<IWorldServer, IHostedService, WorldServer>();
    }
}