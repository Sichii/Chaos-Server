using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chaos;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Effects.Interfaces;
using Chaos.Servers;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    #if DEBUG
                    .AddJsonFile("appsettings.local.json")
                    #else
                    .AddJsonFile("appsettings.prod.json")
                    #endif
                    .Build();


var startup = new Startup(configuration);
startup.ConfigureServices(services);

await using var provider = services.BuildServiceProvider();

//initialize caches
await Task.WhenAll(
    provider.GetRequiredService<ISimpleCache<IEffect>>().LoadCacheAsync(),
    provider.GetRequiredService<ISimpleCache<ItemTemplate>>().LoadCacheAsync(),
    provider.GetRequiredService<ISimpleCache<SkillTemplate>>().LoadCacheAsync(),
    provider.GetRequiredService<ISimpleCache<SpellTemplate>>().LoadCacheAsync(),
    provider.GetRequiredService<ISimpleCache<Metafile>>().LoadCacheAsync(),
    provider.GetRequiredService<ISimpleCache<MapTemplate>>().LoadCacheAsync());

await provider.GetRequiredService<ISimpleCache<MapInstance>>().LoadCacheAsync();

var lobbyServer = provider.GetRequiredService<LobbyServer>();
var loginServer = provider.GetRequiredService<LoginServer>();
var worldServer = provider.GetRequiredService<WorldServer>();

var ctx = new CancellationTokenSource();

await Task.WhenAll(lobbyServer.StartAsync(ctx.Token), 
    loginServer.StartAsync(ctx.Token), 
    worldServer.StartAsync(ctx.Token));

//do nothing
await ctx.Token.WaitTillCanceled();