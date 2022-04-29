using System;
using System.IO;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.DataObjects;
using Chaos.Effects.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Servers.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos;

public class Program
{
    public static async Task LoadCaches(IServiceProvider provider)
    {
        await Task.WhenAll(provider.GetRequiredService<ICacheManager<string, IEffect>>().LoadCacheAsync(),
            provider.GetRequiredService<ICacheManager<string, ItemTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ICacheManager<string, SkillTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ICacheManager<string, SpellTemplate>>().LoadCacheAsync(),
            provider.GetRequiredService<ICacheManager<string, Metafile>>().LoadCacheAsync(),
            provider.GetRequiredService<ICacheManager<short, MapTemplate>>().LoadCacheAsync());

        await provider.GetRequiredService<ICacheManager<string, MapInstance>>().LoadCacheAsync();
    }

    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            #if DEBUG
            .AddJsonFile("appsettings.local.json")
            #endif
            .Build();

        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        await LoadCaches(provider);

        var lobbyServer = provider.GetRequiredService<ILobbyServer>();
        lobbyServer.Start();

        var loginServer = provider.GetRequiredService<ILoginServer>();
        loginServer.Start();

        var worldServer = provider.GetRequiredService<IWorldServer>();
        worldServer.Start();

        //do nothing
        await Task.Delay(-1).ConfigureAwait(false);
    }
}