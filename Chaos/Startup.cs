using System.Text;
using Chaos.Cryptography.Extensions;
using Chaos.Extensions;
using Chaos.Packets.Extensions;
using Chaos.Pathfinding.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Chaos;

public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddOptions();

        services.AddOptionsFromConfig<ChaosOptions>(ConfigKeys.Options.Key)
                .Validate(o => !string.IsNullOrEmpty(o.StagingDirectory), "RootDirectory is required");
        
        services.AddLogging(
            logging =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(Configuration.GetSection(ConfigKeys.Logging.Key));

                logging.AddNLog(
                    Configuration,
                    new NLogProviderOptions
                    {
                        LoggingConfigurationSectionName = ConfigKeys.Logging.NLog.Key
                    });
            });

        services.AddServerAuthentication();
        services.AddCryptography();
        services.AddPacketSerializersFromAssembly();
        services.AddAislingSerialization();

        services.AddPathfinding();
        services.AddSimpleCaches();
        services.AddScripting();
        services.AddWorldFactories();
        services.AddTypeMappersFromAssembly();

        services.AddLobbyServer();
        services.AddLoginserver();
        services.AddWorldServer();

        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class ConfigKeys
    {
        public static class Options
        {
            public static string Key => "Options";
        }

        public static class Logging
        {
            public static string Key => "Logging";

            public static class NLog
            {
                public static string Key => $"{Logging.Key}:NLog";
            }
        }
    }
}