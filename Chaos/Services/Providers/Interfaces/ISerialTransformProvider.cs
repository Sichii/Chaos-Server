using Chaos.Services.Serialization.Interfaces;

namespace Chaos.Services.Providers.Interfaces;

public interface ISerialTransformProvider
{
    ISerialTransformService<TEntity, TSerialized> GetTransformer<TEntity, TSerialized>();
}