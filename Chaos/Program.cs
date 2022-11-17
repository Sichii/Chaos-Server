using Chaos;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Environment.SetEnvironmentVariable("DOTNET_ReadyToRun", "0");
//Environment.SetEnvironmentVariable("DOTNET_GCHeapHardLimit", "0x1F400000");

var services = new ServiceCollection();

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

var startup = new Startup(configuration);
startup.ConfigureServices(services);

services.AddLobbyServer();
services.AddLoginserver();
services.AddWorldServer();

await using var provider = services.BuildServiceProvider();

var ctx = new CancellationTokenSource();
var hostedServices = provider.GetServices<IHostedService>();

var startFuncs = hostedServices
                 .Select<IHostedService, Func<CancellationToken, Task>>(s => s.StartAsync)
                 .ToArray();

await ctx.Token.WhenAllWithCancellation(startFuncs);
await ctx.Token.WaitTillCanceled();