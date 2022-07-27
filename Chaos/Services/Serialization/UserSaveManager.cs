using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Core.Synchronization;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;
using Chaos.Observers;
using Chaos.Services.Providers.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Services.Serialization.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Serialization;

public class UserSaveManager : ISaveManager<Aisling>
{
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ISerialTransformService<Aisling, SerializableAisling> AislingTransformer;
    private readonly UserSaveManagerOptions Options;
    private readonly AutoReleasingSemaphoreSlim Sync;

    public UserSaveManager(
        ISerialTransformService<Aisling, SerializableAisling> aislingTransformer,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger
    )
    {
        Options = options.Value;
        AislingTransformer = aislingTransformer;
        Logger = logger;
        Sync = new AutoReleasingSemaphoreSlim(1, 1);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public async Task<Aisling> LoadAsync(IWorldClient worldClient, string name)
    {
        Logger.LogDebug("Loading user {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());
        var path = Path.Combine(directory, $"{name.ToLower()}.json");
        await using var stream = File.OpenRead(path);

        var serialized = JsonSerializer.Deserialize<SerializableAisling>(stream, JsonSerializerOptions)!;
        var user = AislingTransformer.Transform(serialized);
        
        Logger.LogTrace("Loaded user {Name}", name);

        return user;
    }

    public async Task SaveAsync(Aisling aisling)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogDebug("Saving user {Name}", aisling.Name);

        var serializable = new SerializableAisling(aisling);
        var directory = Path.Combine(Options.Directory, aisling.Name.ToLower());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, $"{aisling.Name.ToLower()}.json");
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(stream, serializable, JsonSerializerOptions);

        Logger.LogTrace("Saved user {Name}", aisling.Name);
    }
}