using Chaos;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Services;
using Chaos.Services.Abstractions;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
var serverCtx = new CancellationTokenSource();

services.AddSingleton(serverCtx);

startup.ConfigureServices(services);

services.AddSingleton<IGroupService, GroupService>();
services.AddLobbyServer();
services.AddLoginserver();
services.AddWorldServer();

await using var provider = services.BuildServiceProvider();

//initialize objects with a lot of cross-cutting concerns
//this object is needed in a lot of places, some of which it doesnt make a lot of sense to have a service injected into
_ = provider.GetRequiredService<IOptions<WorldOptions>>();
_ = provider.GetRequiredService<IScriptRegistry>();
var logger = provider.GetRequiredService<ILogger<Program>>();

var hostedServices = provider.GetServices<IHostedService>();

var startFuncs = hostedServices
                 .Select<IHostedService, Func<CancellationToken, Task>>(s => s.StartAsync)
                 .ToArray();

await serverCtx.Token.WhenAllWithCancellation(startFuncs);
await serverCtx.Token.WaitTillCanceled();

logger.LogInformation("Waiting 5 seconds for post shutdown tasks to complete");

//wait for everything to shut down
await Task.Delay(5);