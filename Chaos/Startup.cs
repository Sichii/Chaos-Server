using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.DependencyInjection;
using Chaos.Geometry.JsonConverters;
using Chaos.Objects.World;
using Chaos.Serialization;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;

namespace Chaos;

public class Startup
{
    private static readonly SerializationContext JsonContext;
    private static readonly JsonSerializerOptions JsonSerializerOptions;
    private static bool IsInitialized;

    public IConfiguration Configuration { get; set; }

    static Startup()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        JsonSerializerOptions.Converters.Add(new PointConverter());
        JsonSerializerOptions.Converters.Add(new LocationConverter());
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        JsonContext = new SerializationContext(JsonSerializerOptions);
    }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);

        services.AddSingleton(Configuration);
        services.AddOptions();

        services.AddOptionsFromConfig<ChaosOptions>(ConfigKeys.Options.Key)
                .Validate(o => !string.IsNullOrEmpty(o.StagingDirectory), "StagingDirectory is required");

        services.AddSingleton<IStagingDirectory, ChaosOptions>(p => p.GetRequiredService<IOptionsSnapshot<ChaosOptions>>().Value);

        services.AddLogging(
            logging =>
            {
                logging.AddConfiguration(Configuration.GetSection(ConfigKeys.Logging.Key));

                logging.AddNLog(
                    Configuration,
                    new NLogProviderOptions
                    {
                        LoggingConfigurationSectionName = ConfigKeys.Logging.NLog.Key
                    });
            });

        services.AddOptions<JsonSerializerOptions>()
                .Configure<ILogger<WarningJsonTypeInfoResolver>>(
                    (options, logger) =>
                    {
                        if (!IsInitialized)
                        {
                            IsInitialized = true;
                            var defaultResolver = new WarningJsonTypeInfoResolver(logger);
                            var combinedResoler = JsonTypeInfoResolver.Combine(JsonContext, defaultResolver);

                            JsonSerializerOptions.SetTypeResolver(combinedResoler);
                        }

                        ShallowCopy<JsonSerializerOptions>.Merge(JsonSerializerOptions, options);
                    });

        services.AddCommandInterceptorForType<Aisling>("/", a => a.IsAdmin);
        services.AddServerAuthentication();
        services.AddCryptography();
        services.AddPacketSerializer();
        services.AddPathfinding();
        services.AddStorage();
        services.AddScripting();
        services.AddFunctionalScriptRegistry();
        services.AddWorldFactories();
        services.AddTypeMapper();

        services.AddSingleton<IShardGenerator, ExpiringMapInstanceCache>(
            p => (ExpiringMapInstanceCache)p.GetRequiredService<ISimpleCache<MapInstance>>());
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