using System.Runtime.InteropServices;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Models.World;

public sealed class Monster : Creature, IScripted<IMonsterScript>, IDialogSourceEntity
{
    public int AggroRange { get; set; }
    public int Experience { get; set; }
    public LootTable? LootTable { get; set; }
    public Creature? Target { get; set; }
    public ConcurrentDictionary<uint, int> AggroList { get; }
    /// <inheritdoc />
    public override int AssailIntervalMs => Template.AssailIntervalMs;
    public ConcurrentDictionary<uint, int> Contribution { get; }
    public List<Item> Items { get; }
    public override ILogger<Monster> Logger { get; }
    public IIntervalTimer MoveTimer { get; }

    /// <inheritdoc />
    public override IMonsterScript Script { get; }
    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }
    public List<Skill> Skills { get; }
    public IIntervalTimer SkillTimer { get; }
    public List<Spell> Spells { get; }
    public IIntervalTimer SpellTimer { get; }
    public override StatSheet StatSheet { get; }
    public MonsterTemplate Template { get; }
    public override CreatureType Type { get; }
    public IIntervalTimer WanderTimer { get; }

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => DisplayColor.Default;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Creature;

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
        StatSheet = ShallowCopy<StatSheet>.Create(template.StatSheet);
        Items = new List<Item>();
        Type = template.Type;
        Direction = template.Direction;
        AggroList = new ConcurrentDictionary<uint, int>();
        Contribution = new ConcurrentDictionary<uint, int>();
        WanderTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.WanderIntervalMs), 10, RandomizationType.Positive);
        MoveTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.MoveIntervalMs), 10, RandomizationType.Positive);
        SkillTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SkillIntervalMs), 50);
        SpellTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SpellIntervalMs), 50);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, this);
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    public override void OnApproached(Creature creature)
    {
        base.OnApproached(creature);

        Script.OnApproached(creature);
    }

    public override void OnClicked(Aisling source)
    {
        var now = DateTime.UtcNow;

        if (LastClicked.TryGetValue(source.Id, out var lastClicked))
            if (now.Subtract(lastClicked).TotalMilliseconds < 1000)
                return;

        LastClicked[source.Id] = now;
        source.SendOrangeBarMessage(Name);
        Script.OnClicked(source);
    }

    /// <inheritdoc />
    public override void OnDeparture(Creature creature)
    {
        base.OnDeparture(creature);

        Script.OnDeparture(creature);
    }

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (source.TryTakeGold(amount))
        {
            Gold += amount;
            source.Client.SendAttributes(StatUpdateType.ExpGold);
            Script.OnGoldDroppedOn(source, amount);

            Logger.WithProperties(source, this)
                  .LogDebug(
                      "Aisling {@AislingName} dropped {Amount} gold on monster {@MonsterName}",
                      source.Name,
                      amount,
                      Name);
        }
    }

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        if (source.Inventory.RemoveQuantity(slot, count, out var items))
            foreach (var item in items)
            {
                Logger.WithProperties(source, item, this)
                      .LogDebug(
                          "Aisling {@AislingName} dropped item {@ItemName} on monster {@MonsterName}",
                          source.Name,
                          item.DisplayName,
                          Name);

                Items.Add(item);
                Script.OnItemDroppedOn(source, item);
            }
    }

    public void ResetAggro()
    {
        Target = null;
        AggroList.Clear();

        foreach (var key in ApproachTime.Keys)
            ApproachTime[key] = DateTime.UtcNow;
    }

    public void ResetAggro(uint id)
    {
        if (Target?.Id == id)
            Target = null;

        AggroList.Remove(id, out _);

        if (ApproachTime.TryGetValue(id, out _))
            ApproachTime[id] = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        foreach (ref var skill in CollectionsMarshal.AsSpan(Skills))
            skill.Update(delta);

        foreach (ref var spell in CollectionsMarshal.AsSpan(Spells))
            spell.Update(delta);

        WanderTimer.Update(delta);
        MoveTimer.Update(delta);
        SkillTimer.Update(delta);
        SpellTimer.Update(delta);

        base.Update(delta);
    }
}