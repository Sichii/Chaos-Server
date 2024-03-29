using System.Diagnostics;
using Chaos;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

Environment.SetEnvironmentVariable("DOTNET_ReadyToRun", "0");

Process.GetCurrentProcess()
       .PriorityClass = ProcessPriorityClass.High;

//Environment.SetEnvironmentVariable("DOTNET_GCHeapHardLimit", "0x1F400000");

var services = new ServiceCollection();

// @formatter:off
var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile("appsettings.logging.json", false, true)
                    #if DEBUG
                    .AddJsonFile("appsettings.local.json", false, true)
                    #else
                    .AddJsonFile("appsettings.prod.json", false, true)
                    #endif
                    ;

var initialConfiguration = builder.Build();

if(initialConfiguration.GetValue<bool>(Startup.ConfigKeys.Logging.UseSeq))
    builder.AddJsonFile("appsettings.seq.json", false, true);

var configuration = builder.Build();
// @formatter:on

var startup = new Startup(configuration);

startup.ConfigureServices(services);
var serverCtx = startup.ServerCtx;

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    serverCtx.Cancel();
};

services.AddLobbyServer();
services.AddLoginserver();
services.AddWorldServer();

await using var provider = services.BuildServiceProvider();

//initialize objects with a lot of cross-cutting concerns
//this object is needed in a lot of places, some of which it doesnt make a lot of sense to have a service injected into
_ = provider.GetRequiredService<IOptions<WorldOptions>>()
            .Value;
_ = provider.GetRequiredService<IScriptRegistry>();
var logger = provider.GetRequiredService<ILogger<Program>>();

var hostedServices = provider.GetServices<IHostedService>();

var startFuncs = hostedServices.Select<IHostedService, Func<CancellationToken, Task>>(s => s.StartAsync)
                               .ToArray();

await serverCtx.Token.WhenAllWithCancellation(startFuncs);
await serverCtx.Token.WaitTillCanceled();

logger.WithTopics(Topics.Actions.Disconnect)
      .LogInformation("Waiting 5 seconds for post shutdown tasks to complete");

//wait for everything to shut down
await Task.Delay(5000);