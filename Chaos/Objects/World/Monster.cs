using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public class Monster : Creature, IScriptedMonster
{
    public List<Item> Items { get; }
    public List<Skill> Skills { get; }
    public List<Spell> Spells { get; }
    public override StatSheet StatSheet { get; }
    public sealed override CreatureType Type { get; }
    protected override ILogger<Monster> Logger { get; }
    public IIntervalTimer WanderTimer { get; }
    public IIntervalTimer MoveTimer { get; }
    public IIntervalTimer AttackTimer { get; }
    public IIntervalTimer CastTimer { get; }
    public MonsterTemplate Template { get; }
    public int Experience { get; set; }
    public Creature? Target { get; set; }
    public int AggroRange { get; set; }
    public ConcurrentDictionary<uint, int> AggroList { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    public Monster(
        MonsterTemplate template,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Monster> logger,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null
    )
        : base(
            template.Name,
            template.Sprite,
            mapInstance,
            point)
    {
        extraScriptKeys ??= Array.Empty<string>();

        Items = new List<Item>();
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Template = template;
        Logger = logger;
        StatSheet = ShallowCopy<StatSheet>.Clone(template.StatSheet);
        Items = new List<Item>();
        Type = template.Type;
        Direction = template.Direction;
        AggroList = new ConcurrentDictionary<uint, int>();
        WanderTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.WanderingIntervalMs), 10);
        MoveTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.MoveIntervalMs), 10);
        AttackTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.AttackIntervalMs), 10);
        CastTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.CastIntervalMs), 50);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, this);
    }

    public override void OnClicked(Aisling source)
    {
        var now = DateTime.UtcNow;

        if (LastClicked.TryGetValue(source.Id, out var lastClicked))
            if (now.Subtract(lastClicked).TotalMilliseconds < 1000)
                return;

        LastClicked[source.Id] = now;
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, Name);
        Script.OnClicked(source);
    }

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if ((uint)Gold + amount > int.MaxValue)
            return;

        source.Gold -= amount;
        Gold += amount;

        source.Client.SendAttributes(StatUpdateType.ExpGold);
        Script.OnGoldDroppedOn(source, amount);
    }

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        if (source.Inventory.RemoveQuantity(slot, count, out var item))
        {
            Logger.LogDebug(
                "{UserName} dropped {Item} on monster {MonsterName}",
                source.Name,
                item,
                Name);

            Items.Add(item);
            Script.OnItemDroppedOn(source, item);
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        foreach(var skill in Skills)
            skill.Update(delta);
        
        foreach(var spell in Spells)
            spell.Update(delta);
        
        WanderTimer.Update(delta);
        MoveTimer.Update(delta);
        AttackTimer.Update(delta);
        CastTimer.Update(delta);
        Script.Update(delta);
    }

    /// <inheritdoc />
    public IMonsterScript Script { get; }
}