using System;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Observers;
using Chaos.WorldObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class UserFactory : IUserFactory
{
    private readonly IServiceProvider ServiceProvider;
    private readonly ICacheManager<string, MapInstance> MapInstanceManager;

    public UserFactory(IServiceProvider serviceProvider, ICacheManager<string, MapInstance> mapInstanceManager)
    {
        ServiceProvider = serviceProvider;
        MapInstanceManager = mapInstanceManager;
    }

    public User CreateUser(string name, Gender gender, int hairStyle, DisplayColor hairColor)
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<User>>();
        var user = new User(name, gender, hairStyle, hairColor, logger);
        
        
        return user;
    }

    public User CreateUser(IWorldClient worldClient, string name)
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<User>>();
        var user = new User(worldClient, name, logger);
        
        var weightObserver = new WeightObserver(user);
        var inventoryObserver = new InventoryObserver(user);
        var equipmentObserver = new EquipmentObserver(user);
        var spellBookObserver = new SpellBookObserver(user);
        var skillBookObserver = new SkillBookObserver(user);

        user.Inventory.AddObserver(weightObserver);
        user.Inventory.AddObserver(inventoryObserver);
        user.Equipment.AddObserver(equipmentObserver);
        user.SpellBook.AddObserver(spellBookObserver);
        user.SkillBook.AddObserver(skillBookObserver);

        return user;
    }
}