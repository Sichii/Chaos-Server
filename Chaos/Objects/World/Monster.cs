using System.Runtime.InteropServices;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Templates;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public sealed class Monster : Creature, IScripted<IMonsterScript>
{
    public int AggroRange { get; set; }
    public int Experience { get; set; }
    public Creature? Target { get; set; }
    public ConcurrentDictionary<uint, int> AggroList { get; }
    /// <inheritdoc />
    public override int AssailIntervalMs => Template.AssailIntervalMs;
    public List<Item> Items { get; }
    public IIntervalTimer MoveTimer { get; }

    /// <inheritdoc />
    public IMonsterScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }
    public List<Skill> Skills { get; }
    public IIntervalTimer SkillTimer { get; }
    public List<Spell> Spells { get; }
    public IIntervalTimer SpellTimer { get; }
    public override StatSheet StatSheet { get; }
    public MonsterTemplate Template { get; }
    public override CreatureType Type { get; }
    public IIntervalTimer WanderTimer { get; }
    public LootTable? LootTable { get; set; }
    protected override ILogger<Monster> Logger { get; }

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
        WanderTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.WanderIntervalMs), 10, RandomizationType.Positive);
        MoveTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.MoveIntervalMs), 10, RandomizationType.Positive);
        SkillTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SkillIntervalMs), 50);
        SpellTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SpellIntervalMs), 50);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override void ApplyDamage(Creature source, int amount, byte? hitSound = 1)
    {
        Script.OnAttacked(source, ref amount);
        StatSheet.SubtractHp(amount);
        ShowHealth(hitSound);
    }

    /// <inheritdoc />
    public override void OnApproached(Creature creature) => Script.OnApproached(creature);

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

    /// <inheritdoc />
    public override void OnDeparture(Creature creature) => Script.OnDeparture(creature);

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (source.TryTakeGold(amount))
        {
            Gold += amount;
            source.Client.SendAttributes(StatUpdateType.ExpGold);
            Script.OnGoldDroppedOn(source, amount);

            Logger.LogDebug(
                "{UserName} dropped {Amount} gold on {MonsterName}",
                source.Name,
                amount,
                Name);
        }
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

        foreach (ref var skill in CollectionsMarshal.AsSpan(Skills))
            skill.Update(delta);

        foreach (ref var spell in CollectionsMarshal.AsSpan(Spells))
            spell.Update(delta);

        WanderTimer.Update(delta);
        MoveTimer.Update(delta);
        SkillTimer.Update(delta);
        SpellTimer.Update(delta);
        Script.Update(delta);

        if (!IsAlive && !IsDead)
        {
            IsDead = true;
            Script.OnDeath();
        }
    }
}