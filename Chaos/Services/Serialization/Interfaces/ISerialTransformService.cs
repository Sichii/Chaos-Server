namespace Chaos.Services.Serialization.Interfaces;

public interface ISerialTransformService<TEntity, TSerialized>
{
    TEntity Transform(TSerialized serialized);
    TSerialized Transform(TEntity entity);
}