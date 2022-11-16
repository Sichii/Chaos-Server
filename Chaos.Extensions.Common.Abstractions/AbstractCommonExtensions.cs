using Chaos.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class AbstractCommonExtensions
{
    public static OptionsBuilder<T> AddDirectoryBoundOptionsFromConfig<T>(this IServiceCollection services, string? subSection = null)
        where T: class, IDirectoryBound
    {
        var typeName = typeof(T).Name;
        var path = typeName;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{typeName}";

        return services.AddOptions<T>()
                       .Configure<IConfiguration, IStagingDirectory>(
                           (options, config, stagingDir) =>
                           {
                               config.GetRequiredSection(path).Bind(options, binder => binder.ErrorOnUnknownConfiguration = true);
                               options.UseBaseDirectory(stagingDir.StagingDirectory);
                           });
    }
}