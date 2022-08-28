using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Core.Synchronization;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.World;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Services.Serialization.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Serialization;

public class UserSaveManager : ISaveManager<Aisling>
{
    private static readonly AutoReleasingSemaphoreSlim Sync = new(1, 1);
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ITypeMapper Mapper;
    private readonly UserSaveManagerOptions Options;

    public UserSaveManager(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger
    )
    {
        Options = options.Value;
        Mapper = mapper;
        Logger = logger;
        //Sync = new AutoReleasingSemaphoreSlim(1, 1);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public async Task<Aisling> LoadAsync(IWorldClient worldClient, string name)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogDebug("Loading aisling {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());
        var path = Path.Combine(directory, $"{name.ToLower()}.json");
        await using var stream = File.OpenRead(path);

        var schema = JsonSerializer.Deserialize<AislingSchema>(stream, JsonSerializerOptions)!;
        var user = Mapper.Map<Aisling>(schema);

        Logger.LogTrace("Loaded aisling {Name}", name);

        return user;
    }

    public async Task SaveAsync(Aisling aisling)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogDebug("Saving aisling {Name}", aisling.Name);

        var schema = Mapper.Map<AislingSchema>(aisling);
        var directory = Path.Combine(Options.Directory, aisling.Name.ToLower());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, $"{aisling.Name.ToLower()}.json");
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(stream, schema, JsonSerializerOptions);

        Logger.LogTrace("Saved aisling {Name}", aisling.Name);
    }
}