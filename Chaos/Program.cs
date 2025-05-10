#region
using System.Diagnostics;
using System.Runtime;
using System.Text;
using System.Text.Json.Serialization;
using Chaos;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.DependencyInjection;
using Chaos.Geometry.Abstractions;
using Chaos.Messaging;
using Chaos.Messaging.Options;
using Chaos.Models.Board;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Site.Utilities;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using AppContext = Chaos.AppContext;
#endregion

var encodingProvider = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(encodingProvider);

GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

Process.GetCurrentProcess()
       .PriorityClass = ProcessPriorityClass.High;

var currentDirectory = Directory.GetCurrentDirectory();

var builder = WebApplication.CreateEmptyBuilder(
    new WebApplicationOptions
    {
        ApplicationName = "Chaos",
        Args = args,
        ContentRootPath = currentDirectory,
        WebRootPath = currentDirectory,
        #if DEBUG
        EnvironmentName = "Development"
        #else
        EnvironmentName = "Production"
        #endif
    });

AddConfiguration(builder);
ConfigureServices(builder);
ConfigureHost(builder);
RegisterStructuredLoggingTransformations();

//build the app
var app = builder.Build();

ConfigureApp(app);
var logger = app.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine($"Server GC: {GCSettings.IsServerGC}");
Console.WriteLine($"Concurrent GC: {GCSettings.LatencyMode != GCLatencyMode.Batch}");
Console.WriteLine($"GC Latency Mode: {GCSettings.LatencyMode}");

await Task.Delay(2500);

await RunApp(app);

logger.WithTopics(Topics.Actions.Disconnect)
      .LogInformation("Waiting 5 seconds for post shutdown tasks to complete");

//wait for everything to shut down
await Task.Delay(5000);

//flush logger
LogManager.Shutdown();

return;

static void AddConfiguration(WebApplicationBuilder builder)
{
    var currentDirectory = Directory.GetCurrentDirectory();

    builder.Configuration
           .SetBasePath(currentDirectory)
           .AddJsonFile("appsettings.json", false, true)
           .AddJsonFile("appsettings.logging.json", false, true)
           #if DEBUG
           .AddJsonFile("appsettings.local.json", false, true);
    #else
           .AddJsonFile("appsettings.prod.json", false, true);
    #endif

    var useSeq = builder.Configuration.GetValue<bool>(ConfigKeys.Logging.UseSeq);

    if (useSeq)
        builder.Configuration.AddJsonFile("appsettings.seq.json", false, true);
}

static async Task RunApp(WebApplication app)
{
    AppContext.Provider = app.Services;

    var serverCtx = app.Services.GetRequiredService<CancellationTokenSource>();

    await app.RunAsync(serverCtx.Token);
    await serverCtx.Token.WaitTillCanceled();
}

static bool IsSiteEnabled(IConfiguration config) => config.GetValue<bool>(ConfigKeys.Options.SiteOptions.EnableSite);

static void ConfigureApp(WebApplication app)
{
    var serverCtx = app.Services.GetRequiredService<CancellationTokenSource>();

    //app configuration
    //initialize objects with a lot of cross-cutting concerns
    //this object is needed in a lot of places, some of which it doesnt make a lot of sense to have a service injected into
    _ = app.Services.GetRequiredService<IOptions<WorldOptions>>()
           .Value;
    _ = app.Services.GetRequiredService<IScriptRegistry>();

    if (IsSiteEnabled(app.Configuration))
    {
        app.UseDeveloperExceptionPage();
        app.UseStaticFiles(new StaticFileOptions());
        app.UseRouting();
        app.MapRazorPages();
    }

    //intercept ctrl+c to gracefully shutdown
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        serverCtx.Cancel();
    };
}

static void ConfigureHost(WebApplicationBuilder builder) => builder.WebHost.UseKestrel(opts => opts.ListenAnyIP(5000));

static void ConfigureServices(WebApplicationBuilder builder)
{
    var serverCtx = new CancellationTokenSource();

    builder.Services.AddSingleton(serverCtx);
    builder.Services.AddChaosOptions();

    builder.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    });

    builder.Services.AddJsonSerializerOptions();
    builder.Services.AddCommandInterceptor<Aisling, AislingCommandInterceptorOptions>(ConfigKeys.Options.Key);
    builder.Services.AddChannelService(ConfigKeys.Options.Key);
    builder.Services.AddServerAuthentication();
    builder.Services.AddCryptography();
    builder.Services.AddPacketSerializer();
    builder.Services.AddPathfinding();
    builder.Services.AddStorage();
    builder.Services.AddScripting();
    builder.Services.AddFunctionalScriptRegistry();
    builder.Services.AddWorldFactories();
    builder.Services.AddTypeMapper();
    builder.Services.AddSingleton<IStockService, IHostedService, StockService>();

    builder.Services.AddSingleton<IShardGenerator, ExpiringMapInstanceCache>(p
        => (ExpiringMapInstanceCache)p.GetRequiredService<ISimpleCache<MapInstance>>());

    builder.Services.AddLobbyServer();
    builder.Services.AddLoginserver();
    builder.Services.AddWorldServer();

    if (IsSiteEnabled(builder.Configuration))
    {
        builder.Services
               .AddRazorPages(opts => opts.RootDirectory = "/Site/Pages")
               .AddRazorRuntimeCompilation()
               .AddJsonOptions(opts =>
               {
                   var jsonSerializerOptions = opts.JsonSerializerOptions;

                   jsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                   jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                   jsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
               });

        builder.Environment.WebRootPath = "Site/wwwroot";

        builder.ConfigureSite();
    }
}

static void RegisterStructuredLoggingTransformations()
    => LogManager.Setup()
                 .SetupSerialization(builder =>
                 {
                     builder.RegisterObjectTransformation<ISocketClient>(client => new
                     {
                         IpAddress = client.RemoteIp
                     });

                     builder.RegisterObjectTransformation<IChaosLobbyClient>(client => new
                     {
                         IpAddress = client.RemoteIp,
                         Type = "LobbyClient"
                     });

                     builder.RegisterObjectTransformation<IChaosLoginClient>(client => new
                     {
                         IpAddress = client.RemoteIp,
                         Type = "LoginClient"
                     });

                     builder.RegisterObjectTransformation<IChaosWorldClient>(client => new
                     {
                         IpAddress = client.RemoteIp,
                         LoginId1 = client.LoginId1,
                         LoginId2 = client.LoginId2,

                         // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                         Id = client.Aisling?.Id,
                         Name = client.Aisling?.Name,
                         Location = client.Aisling is not null ? ILocation.ToString(client.Aisling) : null,
                         Type = "WorldClient"
                     });

                     builder.RegisterObjectTransformation<WorldEntity>(obj => new
                     {
                         Type = obj.GetType()
                                   .Name,
                         Id = obj.Id,
                         Creation = obj.Creation
                     });

                     builder.RegisterObjectTransformation<MapEntity>(obj => new
                     {
                         Type = obj.GetType()
                                   .Name,
                         Id = obj.Id,
                         Creation = obj.Creation,
                         Location = ILocation.ToString(obj)
                     });

                     builder.RegisterObjectTransformation<VisibleEntity>(obj => new
                     {
                         Type = obj.GetType()
                                   .Name,
                         Id = obj.Id,
                         Creation = obj.Creation,
                         Location = ILocation.ToString(obj),
                         Sprite = obj.Sprite
                     });

                     builder.RegisterObjectTransformation<NamedEntity>(obj => new
                     {
                         Type = obj.GetType()
                                   .Name,
                         Id = obj.Id,
                         Creation = obj.Creation,
                         Location = ILocation.ToString(obj),
                         Name = obj.Name
                     });

                     builder.RegisterObjectTransformation<Creature>(obj => new
                     {
                         Type = obj.GetType()
                                   .Name,
                         Id = obj.Id,
                         Location = ILocation.ToString(obj),
                         Name = obj.Name
                     });

                     builder.RegisterObjectTransformation<Aisling>(obj => new
                     {
                         // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                         IpAddress = obj.Client?.RemoteIp,
                         Id = (uint?)obj.Id,
                         Name = obj.Name,
                         Location = ILocation.ToString(obj),
                         LoginId1 = obj.Client?.LoginId1,
                         LoginId2 = obj.Client?.LoginId2
                     });

                     builder.RegisterObjectTransformation<Monster>(obj => new
                     {
                         Type = nameof(Monster),
                         Id = obj.Id,
                         Name = obj.Name,
                         TemplateKey = obj.Template.TemplateKey,
                         Location = ILocation.ToString(obj)
                     });

                     builder.RegisterObjectTransformation<Merchant>(obj => new
                     {
                         Type = nameof(Merchant),
                         Id = obj.Id,
                         Name = obj.Name,
                         TemplateKey = obj.Template.TemplateKey,
                         Location = ILocation.ToString(obj)
                     });

                     builder.RegisterObjectTransformation<Money>(obj => new
                     {
                         Type = nameof(Money),
                         Id = obj.Id,
                         Amount = obj.Amount,
                         Creation = obj.Creation,
                         Location = ILocation.ToString(obj)
                     });

                     builder.RegisterObjectTransformation<Item>(obj => new
                     {
                         Uid = obj.UniqueId,
                         Name = obj.DisplayName,
                         TemplateKey = obj.Template.TemplateKey,
                         Count = obj.Count
                     });

                     builder.RegisterObjectTransformation<Spell>(obj => new
                     {
                         Uid = obj.UniqueId,
                         Name = obj.Template.Name,
                         TemplateKey = obj.Template.TemplateKey
                     });

                     builder.RegisterObjectTransformation<Skill>(obj => new
                     {
                         Uid = obj.UniqueId,
                         Name = obj.Template.Name,
                         TemplateKey = obj.Template.TemplateKey
                     });

                     builder.RegisterObjectTransformation<MapInstance>(obj => new
                     {
                         InstanceId = obj.InstanceId,
                         BaseInstanceId = obj.BaseInstanceId,
                         TemplateKey = obj.Template.TemplateKey,
                         Name = obj.Name
                     });

                     builder.RegisterObjectTransformation<CommandDescriptor>(obj => new
                     {
                         CommandName = obj.Details.CommandName,
                         RequiresAdmin = obj.Details.RequiresAdmin
                     });

                     builder.RegisterObjectTransformation<IEffect>(obj => new
                     {
                         EffectKey = obj.ScriptKey,
                         Name = obj.Name
                     });

                     builder.RegisterObjectTransformation<Redirect>(obj => new
                     {
                         Id = obj.Id,
                         Name = obj.Name,
                         Type = obj.Type,
                         Address = obj.EndPoint
                     });

                     builder.RegisterObjectTransformation<Guild>(obj => obj.Name);

                     builder.RegisterObjectTransformation<GroundItem>(obj => new
                     {
                         Type = nameof(GroundItem),
                         Id = obj.Id,
                         ItemUid = obj.Item.UniqueId,
                         ItemName = obj.Item.DisplayName,
                         ItemTemplateKey = obj.Item.Template.TemplateKey,
                         ItemCount = obj.Item.Count,
                         Creation = obj.Creation,
                         Location = ILocation.ToString(obj),
                         NoTrade = obj.Item.NoTrade,
                         AccountBound = obj.Item.AccountBound,
                         PreventBanking = obj.Item.Color
                     });

                     builder.RegisterObjectTransformation<Group>(obj => new
                     {
                         Id = obj.Id,
                         LeaderName = obj.Leader.Name,
                         MemberCount = obj.Count
                     });

                     builder.RegisterObjectTransformation<Dialog>(obj => new
                     {
                         TemplateKey = obj.Template.TemplateKey,
                         Type = obj.Template.Type,
                         Contextual = obj.Template.Contextual,
                         HasContext = obj.Context is not null,
                         HasMenuArgs = obj.MenuArgs.Count != 0
                     });

                     builder.RegisterObjectTransformation<Exchange>(obj => new
                     {
                         obj.ExchangeId
                     });

                     builder.RegisterObjectTransformation<BoardBase>(obj => new
                     {
                         Key = obj.Key,
                         Name = obj.Name,
                         Posts = obj.Posts.Count
                     });

                     builder.RegisterObjectTransformation<MailBox>(obj => new
                     {
                         Key = obj.Key,
                         Name = obj.Name,
                         Posts = obj.Posts.Count
                     });

                     builder.RegisterObjectTransformation<BulletinBoard>(obj => new
                     {
                         Key = obj.Key,
                         Name = obj.Name,
                         Posts = obj.Posts.Count
                     });

                     builder.RegisterObjectTransformation<Post>(obj => new
                     {
                         PostId = obj.PostId,
                         Author = obj.Author,
                         Subject = obj.Subject,
                         Message = obj.Message,
                         Creation = obj.CreationDate
                     });
                 });