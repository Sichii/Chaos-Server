using System.Text;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions;
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
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;

namespace Chaos;

public sealed class Startup
{
    public IConfiguration Configuration { get; set; }
    public CancellationTokenSource ServerCtx { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        ServerCtx = new CancellationTokenSource();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(ServerCtx);
        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);

        services.AddChaosOptions(Configuration);

        services.AddLogging(logging => logging.AddNLog());
        RegisterStructuredLoggingTransformations();

        services.AddJsonSerializerOptions();

        services.AddCommandInterceptor<Aisling, AislingCommandInterceptorOptions>(ConfigKeys.Options.Key);

        services.AddChannelService(
            ConfigKeys.Options.Key,
            cs =>
            {
                foreach (var defaultChannel in WorldOptions.Instance.DefaultChannels)
                    cs.RegisterChannel(
                        null,
                        defaultChannel.ChannelName,
                        defaultChannel.MessageColor ?? MessageColor.Gainsboro,
                        (sub, message) =>
                        {
                            var aisling = (Aisling)sub;
                            aisling.SendServerMessage(ServerMessageType.ActiveMessage, message);
                            aisling.Client.SendPublicMessage(uint.MaxValue, PublicMessageType.Shout, message);
                        },
                        true);
            });

        services.AddServerAuthentication();
        services.AddCryptography();
        services.AddPacketSerializer();
        services.AddPathfinding();
        services.AddStorage();
        services.AddScripting();
        services.AddFunctionalScriptRegistry();
        services.AddWorldFactories();
        services.AddTypeMapper();

        services.AddSingleton<IStockService, IHostedService, StockService>();

        services.AddSingleton<IShardGenerator, ExpiringMapInstanceCache>(
            p => (ExpiringMapInstanceCache)p.GetRequiredService<ISimpleCache<MapInstance>>());
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public void RegisterStructuredLoggingTransformations() =>
        LogManager.Setup()
                  .SetupSerialization(
                      builder =>
                      {
                          builder.RegisterObjectTransformation<ISocketClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp
                              });

                          builder.RegisterObjectTransformation<ILobbyClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp,
                                  Type = "LobbyClient"
                              });

                          builder.RegisterObjectTransformation<ILoginClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp,
                                  Type = "LoginClient"
                              });

                          builder.RegisterObjectTransformation<IWorldClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp,
                                  // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                                  Id = client.Aisling?.Id,
                                  Name = client.Aisling?.Name,
                                  Location = client.Aisling is not null ? ILocation.ToString(client.Aisling) : null,
                                  Type = "WorldClient"
                              });

                          builder.RegisterObjectTransformation<WorldEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation
                              });

                          builder.RegisterObjectTransformation<MapEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<VisibleEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Sprite = obj.Sprite
                              });

                          builder.RegisterObjectTransformation<NamedEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<Creature>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<Aisling>(
                              obj => new
                              {
                                  // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                                  IpAddress = obj.Client?.RemoteIp,
                                  Id = (uint?)obj.Id,
                                  Name = obj.Name,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<Monster>(
                              obj => new
                              {
                                  Type = nameof(Monster),
                                  Id = obj.Id,
                                  Name = obj.Name,
                                  TemplateKey = obj.Template.TemplateKey,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<Merchant>(
                              obj => new
                              {
                                  Type = nameof(Merchant),
                                  Id = obj.Id,
                                  Name = obj.Name,
                                  TemplateKey = obj.Template.TemplateKey,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<Money>(
                              obj => new
                              {
                                  Type = nameof(Money),
                                  Id = obj.Id,
                                  Amount = obj.Amount,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<Item>(
                              obj => new
                              {
                                  Uid = obj.UniqueId,
                                  Name = obj.DisplayName,
                                  TemplateKey = obj.Template.TemplateKey,
                                  Count = obj.Count
                              });

                          builder.RegisterObjectTransformation<Spell>(
                              obj => new
                              {
                                  Uid = obj.UniqueId,
                                  Name = obj.Template.Name,
                                  TemplateKey = obj.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<Skill>(
                              obj => new
                              {
                                  Uid = obj.UniqueId,
                                  Name = obj.Template.Name,
                                  TemplateKey = obj.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<MapInstance>(
                              obj => new
                              {
                                  InstanceId = obj.InstanceId,
                                  BaseInstanceId = obj.BaseInstanceId,
                                  TemplateKey = obj.Template.TemplateKey,
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<CommandDescriptor>(
                              obj => new
                              {
                                  CommandName = obj.Details.CommandName,
                                  RequiresAdmin = obj.Details.RequiresAdmin
                              });

                          builder.RegisterObjectTransformation<IEffect>(
                              obj => new
                              {
                                  EffectKey = obj.ScriptKey,
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<Redirect>(
                              obj => new
                              {
                                  Id = obj.Id,
                                  Name = obj.Name,
                                  Type = obj.Type,
                                  Address = obj.EndPoint
                              });

                          builder.RegisterObjectTransformation<Guild>(obj => obj.Name);

                          builder.RegisterObjectTransformation<GroundItem>(
                              obj => new
                              {
                                  Type = nameof(GroundItem),
                                  Id = obj.Id,
                                  ItemUid = obj.Item.UniqueId,
                                  ItemName = obj.Item.DisplayName,
                                  ItemTemplateKey = obj.Item.Template.TemplateKey,
                                  ItemCount = obj.Item.Count,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<Dialog>(
                              obj => new
                              {
                                  TemplateKey = obj.Template.TemplateKey,
                                  Type = obj.Template.Type,
                                  Contextual = obj.Template.Contextual,
                                  HasContext = obj.Context is not null,
                                  HasMenuArgs = obj.MenuArgs.Any()
                              });

                          builder.RegisterObjectTransformation<Exchange>(
                              obj => new
                              {
                                  obj.ExchangeId
                              });

                          builder.RegisterObjectTransformation<BoardBase>(
                              obj => new
                              {
                                  Key = obj.Key,
                                  Name = obj.Name,
                                  Posts = obj.Count()
                              });

                          builder.RegisterObjectTransformation<Post>(
                              obj => new
                              {
                                  PostId = obj.PostId,
                                  Author = obj.Author,
                                  Subject = obj.Subject,
                                  Message = obj.Message,
                                  Creation = obj.CreationDate
                              });
                      });

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class ConfigKeys
    {
        public static class Logging
        {
            public static string Key => nameof(Logging);
            public static string UseSeq => $"{Key}:{nameof(UseSeq)}";

            public static class NLog
            {
                public static string Key => $"{Logging.Key}:{nameof(NLog)}";
            }
        }

        public static class NLog
        {
            public static string Key => nameof(NLog);
        }

        public static class Options
        {
            public static string Key => nameof(Options);
        }
    }
}