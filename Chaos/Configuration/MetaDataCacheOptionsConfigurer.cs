using Chaos.Services.Storage.Mutators;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Configuration;

public sealed class MetaDataCacheOptionsConfigurer : IPostConfigureOptions<MetaDataCacheOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, MetaDataCacheOptions options)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        options.PrefixMutators.Add(new EnchantmentMetaNodeMutator());
        //add more mutators here
    }
}