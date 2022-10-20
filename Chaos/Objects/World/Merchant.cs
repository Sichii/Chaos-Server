using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public sealed class Merchant : Creature, IScriptedMerchant
{
    /// <inheritdoc />
    public override int AssailIntervalMs => 500;
    public override bool IsAlive => true;
    public override StatSheet StatSheet { get; }

    public override CreatureType Type { get; }
    protected override ILogger<Merchant> Logger { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }
    /// <inheritdoc />
    public IMerchantScript Script { get; }
    public MerchantTemplate Template { get; }

    public Merchant(
        MerchantTemplate template,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Merchant> logger,
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

        Template = template;
        Logger = logger;
        StatSheet = StatSheet.Maxed;
        Type = CreatureType.Merchant;
        Direction = template.Direction;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, this);
    }

    public override void ApplyDamage(
        Creature source,
        int amount,
        byte? hitSound = 1
    ) => Script.OnAttacked(source, ref amount);

    public override void OnClicked(Aisling source) => Script.OnClicked(source);

    public override void OnGoldDroppedOn(Aisling source, int amount) => Script.OnGoldDroppedOn(source, amount);

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count) => Script.OnItemDroppedOn(source, slot, count);

    /// <inheritdoc />
    public override void OnApproached(Creature creature) => Script.OnApproached(creature);

    /// <inheritdoc />
    public override void OnDeparture(Creature creature) => Script.OnDeparture(creature);
}