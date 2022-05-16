using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Chaos.Extensions;

public static class ServiceCollectionExtensions
{
    public static OptionsBuilder<T> AddOptionsFromConfig<T>(this IServiceCollection services, string? subSection = null) where T: class
    {
        var typeName = typeof(T).Name;
        var path = typeName;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{typeName}";

        return services.AddOptions<T>()
                       .Configure<IConfiguration>(
                           (o, c) => c.GetSection(path).Bind(o, options => options.ErrorOnUnknownConfiguration = true));
    }
}