using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Chaos.Clients.Interfaces;
using Chaos.Core.Synchronization;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;
using Chaos.Observers;
using Chaos.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public class UserSaveManager : ISaveManager<User>
{
    private readonly IExchangeFactory ExchangeFactory;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly IMapper Mapper;
    private readonly UserSaveManagerOptions Options;
    private readonly AutoReleasingSemaphoreSlim Sync;

    public UserSaveManager(
        IMapper mapper,
        IExchangeFactory exchangeFactory,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger
    )
    {
        Mapper = mapper;
        ExchangeFactory = exchangeFactory;
        Options = options.Value;
        Logger = logger;
        Sync = new AutoReleasingSemaphoreSlim(1, 1);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public async Task<User> LoadAsync(IWorldClient worldClient, string name)
    {
        Logger.LogDebug("Loading user {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());
        var path = Path.Combine(directory, $"{name.ToLower()}.json");
        await using var stream = File.OpenRead(path);

        var serialized = JsonSerializer.Deserialize<SerializableUser>(stream, JsonSerializerOptions)!;
        var user = new User(worldClient, name, ExchangeFactory);

        var inventoryObserver = new InventoryObserver(user);
        var equipmentObserver = new EquipmentObserver(user);
        var spellBookObserver = new SpellBookObserver(user);
        var skillBookObserver = new SkillBookObserver(user);

        user.Inventory.AddObserver(inventoryObserver);
        user.Equipment.AddObserver(equipmentObserver);
        user.SpellBook.AddObserver(spellBookObserver);
        user.SkillBook.AddObserver(skillBookObserver);

        user = Mapper.Map(serialized, user);
        user.Loading = false;

        Logger.LogTrace("Loaded user {Name}", name);

        return user;
    }

    public async Task SaveAsync(User user)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogDebug("Saving user {Name}", user.Name);

        var serializable = Mapper.Map<SerializableUser>(user);
        var directory = Path.Combine(Options.Directory, user.Name.ToLower());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, $"{user.Name.ToLower()}.json");
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(stream, serializable, JsonSerializerOptions);

        Logger.LogTrace("Saved user {Name}", user.Name);
    }
}