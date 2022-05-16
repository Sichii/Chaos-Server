using System.IO;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Effects.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Servers.Interfaces;
using Chaos.Templates;
using Chaos.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos;

public class Program
{
    public static Task InitializeAsync(IServiceProvider provider)
    {
        //initialize the singleton
        _ = provider.GetRequiredService<ItemUtility>();

        LoadScripts(provider);

        return LoadCachesAsync(provider);
    }

    public static async Task LoadCachesAsync(IServiceProvider provider)
    {
        await Task.WhenAll(
            provider.GetRequiredService<ISimpleCache<string, IEffect>>().LoadCacheAsync(),
            provider.GetRequiredService<ISimpleCache<string, ItemTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ISimpleCache<string, SkillTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ISimpleCache<string, SpellTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ISimpleCache<string, Metafile>>().LoadCacheAsync(),
            provider.GetRequiredService<ISimpleCache<string, MapTemplate>>().LoadCacheAsync());

        await provider.GetRequiredService<ISimpleCache<string, MapInstance>>().LoadCacheAsync();
    }

    public static void LoadScripts(IServiceProvider provider)
    {
        _ = provider.GetRequiredService<IItemScriptFactory>();
        _ = provider.GetRequiredService<ISkillScriptFactory>();
        _ = provider.GetRequiredService<ISpellScriptFactory>();
    }

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            #if DEBUG
                            .AddJsonFile("appsettings.local.json")
                            #else
            .AddJsonFile("appsettings.prod.json")
                            #endif
                            .Build();

        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);

        await using var provider = services.BuildServiceProvider();
        await using var defaultScope = provider.CreateAsyncScope();

        await InitializeAsync(defaultScope.ServiceProvider);
        RunApplication(defaultScope.ServiceProvider);

        //do nothing
        await Task.Delay(-1).ConfigureAwait(false);
    }

    public static void RunApplication(IServiceProvider provider)
    {
        var lobbyServer = provider.GetRequiredService<ILobbyServer>();
        lobbyServer.Start();

        var loginServer = provider.GetRequiredService<ILoginServer>();
        loginServer.Start();

        var worldServer = provider.GetRequiredService<IWorldServer>();
        worldServer.Start();
    }
}