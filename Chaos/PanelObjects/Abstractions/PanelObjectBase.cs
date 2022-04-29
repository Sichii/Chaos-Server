using System;
using Chaos.Core.Utilities;
using Chaos.DataObjects;
using Chaos.Interfaces;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Templates.Abstractions;
using Chaos.Templates.Interfaces;

namespace Chaos.PanelObjects.Abstractions;

/// <summary>
///     Represents an object that exists within the in-game panels.
/// </summary>
public abstract class PanelObjectBase : IActivatable, IScripted<PanelObjectScriptBase>, ITemplated<string, PanelObjectTemplateBase>
{
    public TimeSpan Cooldown { get; set; }
    public DateTime LastUse { get; set; } = DateTime.MinValue;

    public virtual PanelObjectScriptBase? Script { get; set; }
    public byte Slot { get; set; }
    public ulong UniqueId { get; set; } = ServerId.NextId;
    public TimeSpan Elapsed => DateTime.UtcNow.Subtract(LastUse);
    public virtual PanelObjectTemplateBase Template { get; }

    protected PanelObjectBase(PanelObjectTemplateBase template) => Template = template;

    private TimeSpan CalculateTotalCooldown(double cooldownReduction)
    {
        var cdrMultiplier = Math.Clamp(100.0 - cooldownReduction, 0, 100) / 100;

        return Template.BaseCooldown * cdrMultiplier;
    }

    public virtual bool CanUse(double cooldownReduction) => (LastUse == DateTime.MinValue)
                                                            || (Template.BaseCooldown.TotalMilliseconds == 0)
                                                            || (DateTime.UtcNow.Subtract(LastUse)
                                                                >= CalculateTotalCooldown(cooldownReduction));

    public void OnActivated(ActivationContext activationContext) => Script?.OnUse(activationContext);

    public override string ToString() => $@"SLOT: {Slot} | NAME: {Template.Name}({Template.Sprite})";
}