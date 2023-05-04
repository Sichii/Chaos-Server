using Chaos;
using Chaos.Models.Templates;
using Chaos.Storage.Abstractions;
using DeBroglie;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaveFunctionCollapse;
using ChaosTile = Chaos.Models.Map.Tile;

var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    #if DEBUG
                    .AddJsonFile("appsettings.local.json")
                    #else
                    //.AddJsonFile("appsettings.prod.json")
                    .AddJsonFile("appsettings.local.json")
                    #endif
                    .Build();

var services = new ServiceCollection();
var startup = new Startup(configuration);
startup.ConfigureServices(services);

var serverCtx = new CancellationTokenSource();

services.AddSingleton(serverCtx);

var provider = services.BuildServiceProvider();

var simpleCacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
var mapCache = simpleCacheProvider.GetCache<MapTemplate>();
mapCache.ForceLoad();

var backgroundSample = new List<ChaosTile>
{
    //pravate cave
    //new(3776, 0, 0),
    //new(3754, 0, 0)
    //east woodlands
    //new (28, 0, 0),
    //new (215, 0, 0)
    //shinewood
    //new(14406, 0, 0),
    //new(14960, 0, 0)
    //hwarone
    //new(15648, 0, 0),
    //new (15698,0,0)
    //chaos
    new(4090, 0, 0),
    new(4126, 0, 0)
};

var foregroundSample = new List<ChaosTile>
{
    //pravat cave
    //new(0, 5052, 5053),
    //new(0, 5054, 5055)
    //east woodlands
    //new(0, 4876, 4877),
    //new(0, 4898, 4899)
    //shinewood
    //new(0, 14025, 14026),
    //new(0, 14027, 14028),
    //new(0, 14066, 14067)
    //hwarone
    //new(0, 15494, 15495),
    //new (0,15496,15497)
    //chaos
    new(0, 5546, 5547),
    new(0, 5542, 5543)
};

var mapGenerator = new MapGenerator(
    30,
    30,
    2,
    "output",
    mapCache,
    backgroundSample,
    foregroundSample);

mapGenerator.Model.MultiplyFrequency(new Tile(0), 0.5);

for (var i = 0; i < 10; i++)
{
    mapGenerator.Run();
    await mapGenerator.SaveAsync();
    mapGenerator.SoftReset();
}