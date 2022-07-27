using Chaos.Services.Providers.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Providers;

public class SerialTransformProvider : ISerialTransformProvider
{
    private readonly IServiceProvider ServiceProvider;

    public SerialTransformProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public ISerialTransformService<TEntity, TSerialized> GetTransformer<TEntity, TSerialized>() =>
        ServiceProvider.GetRequiredService<ISerialTransformService<TEntity, TSerialized>>();
}