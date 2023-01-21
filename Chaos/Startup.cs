using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Chaos.Common.Abstractions;
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
    public IConfiguration Configuration { get; set; }

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
                        var defaultResolver = new WarningJsonTypeInfoResolver(logger);

                        options.WriteIndented = true;
                        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                        options.PropertyNameCaseInsensitive = true;
                        options.IgnoreReadOnlyProperties = true;
                        options.IgnoreReadOnlyFields = true;
                        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.AllowTrailingCommas = true;
                        options.Converters.Add(new PointConverter());
                        options.Converters.Add(new LocationConverter());
                        options.Converters.Add(new JsonStringEnumConverter());

                        options.TypeInfoResolver = JsonTypeInfoResolver.Combine(SerializationContext.Default, defaultResolver);
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