using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Chaos.Clients.Interfaces;
using Chaos.Core.JsonConverters;
using Chaos.Core.Utilities;
using Chaos.DataObjects.Serializable;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Options;
using Chaos.WorldObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public class UserSaveManager : ISaveManager<User>
{
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly IMapper Mapper;
    private readonly IUserFactory UserFactory;
    private readonly UserSaveManagerOptions Options;
    private readonly AutoReleasingSemaphoreSlim Sync;

    public UserSaveManager(
        IMapper mapper,
        IUserFactory userFactory,
        IOptionsSnapshot<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger)
    {
        Mapper = mapper;
        UserFactory = userFactory;
        Options = options.Value;
        Logger = logger;
        Sync = new AutoReleasingSemaphoreSlim(1, 1);

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public async Task<User> LoadAsync(IWorldClient worldClient, string name)
    {
        var directory = Path.Combine(Options.Directory, name.ToLower());
        var path = Path.Combine(directory, $"{name.ToLower()}.json");
        await using var stream = File.OpenRead(path);

        var serialized = JsonSerializer.Deserialize<SerializableUser>(stream, JsonSerializerOptions)!;

        var user = UserFactory.CreateUser(worldClient, name);

        return Mapper.Map(serialized, user);
    }

    public async Task SaveAsync(User user)
    {
        await using var sync = await Sync.WaitAsync();

        var serializable = Mapper.Map<SerializableUser>(user);
        var directory = Path.Combine(Options.Directory, user.Name.ToLower());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, $"{user.Name.ToLower()}.json");
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(stream, serializable, JsonSerializerOptions);
        Logger.LogDebug("Saved user {Name}", user.Name);
    }
}