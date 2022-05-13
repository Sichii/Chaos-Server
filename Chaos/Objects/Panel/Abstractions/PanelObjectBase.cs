using System;
using System.Collections.Generic;
using Chaos.Core.Interfaces;
using Chaos.Scripts.Interfaces;
using Chaos.Templates.Abstractions;

namespace Chaos.Objects.Panel.Abstractions;

/// <summary>
///     Represents an object that exists within the in-game panels.
/// </summary>
public abstract class PanelObjectBase : IDeltaUpdatable, IScripted
{
    public TimeSpan Cooldown { get; set; }
    public TimeSpan Elapsed { get; set; }
    public ICollection<string> ScriptKeys { get; set; } = new List<string>();
    public byte Slot { get; set; }
    public ulong UniqueId { get; set; }
    public virtual PanelObjectTemplateBase Template { get; }

    protected PanelObjectBase(PanelObjectTemplateBase template)
    {
        Template = template;
        Cooldown = template.BaseCooldown;
        Elapsed = TimeSpan.FromDays(1);
    }

    private TimeSpan ActualCooldown(double cooldownReduction)
    {
        var cdrMultiplier = Math.Clamp(100.0 - cooldownReduction, 0, 100) / 100;

        return Cooldown * cdrMultiplier;
    }

    public virtual bool CanUse(double cooldownReduction) =>
        (Cooldown.TotalMilliseconds == 0) || (Elapsed > ActualCooldown(cooldownReduction));

    public override string ToString() => $@"(Id: {UniqueId}, Name: {Template.Name})";

    public void Update(TimeSpan delta) => Elapsed += delta;
}