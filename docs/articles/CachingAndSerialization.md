# Caching

Chaos starts up with nothing loaded. When something is requested, be it a SkillTemplate, ItemTemplate, or even a MapInstance, it is
fetched and cached for a period of time. If that cached object
is not accessed again within that period of time, it is removed from the cache. This is done to significantly reduce the memory and cpu
usage of the server.

Chaos utilizes the [ISimpleCache](<xref:Chaos.Storage.Abstractions.ISimpleCache`1>) interface. Default implementations of this interface use
microsoft's [IMemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=dotnet-plat-ext-7.0).

Most of the caches that Chaos uses are customizable via `appsettings.json`.
See [ExpiringFileCacheOptions](<xref:Chaos.Storage.ExpiringFileCacheOptions>) for more information.

## Schema Objects

These default caching implementations rely on `Chaos.Schemas` objects as an intermediary for serialization. Think of these schema objects as
DTOs(Data Transfer Objects). They are intended to be bags of properties with no functionality. They exist only to be serialized or
deserialized.

## Object mapping

In many cases, the schema objects are not the same as the actual objects that Chaos uses for interactions. In most scenarios, it doesn't
make a lot of sense to serialize every detail of an object. Many of the basic properties of an item are already saved via a template that
you have created. That template serves as the building block of that object. In cases like this, we really only need to serialize a way to
unique identify that template. This is where object mapping comes in.

Chaos utilizes [IMapperProfiles](<xref:Chaos.TypeMapper.Abstractions.IMapperProfile`2>) to map between the schema objects and the actual
objects. Profiles can utilize other services freely, and if nested mapping is needed they can access eachother through
the [ITypeMapper](<xref:Chaos.TypeMapper.Abstractions.ITypeMapper>) service.

> [!NOTE]
> By default, all profiles in the solution are loaded into the IoC container at startup by the [Mapper](<xref:Chaos.TypeMapper.Mapper>)
> service.

## Serialization

By default, Chaos uses `System.Text.Json` for serialization (See [SerializationContext](<xref:Chaos.Serialization.SerializationContext>)).
If there is a need to use some other medium of storage and serialization, you can can create your
own [ISimpleCache](<xref:Chaos.Storage.Abstractions.ISimpleCache`1>) implementations that do not serialize from json.