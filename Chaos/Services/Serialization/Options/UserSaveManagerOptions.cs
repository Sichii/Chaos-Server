using System.IO;
using Chaos.Services.Caches.Abstractions;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Serialization.Options;

public record UserSaveManagerOptions
{
    public required string Directory { get; set; }

    /// <inheritdoc />
    public static void PostConfigure(UserSaveManagerOptions options, IOptionsSnapshot<ChaosOptions> chaosOptions) =>
        options.Directory = Path.Combine(chaosOptions.Value.StagingDirectory, options.Directory);
}