using Chaos.Core.Identity;
using Chaos.Scripts.Interfaces;
using Chaos.Templates.Abstractions;
using Chaos.Time.Interfaces;

namespace Chaos.Objects.Panel.Abstractions;

/// <summary>
///     Represents an object that exists within the in-game panels.
/// </summary>
public abstract class PanelObjectBase : IDeltaUpdatable, IScripted
{
    public TimeSpan Cooldown { get; set; }
    public TimeSpan Elapsed { get; set; }
    public ISet<string> ScriptKeys { get; }
    public byte Slot { get; set; }
    public ulong UniqueId { get; }
    public virtual PanelObjectTemplateBase Template { get; }
    
    protected PanelObjectBase(PanelObjectTemplateBase template, ulong? uniqueId = null)
    {
        uniqueId ??= ServerId.NextId;
        Template = template;
        Cooldown = template.BaseCooldown;
        Elapsed = TimeSpan.FromDays(1);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        UniqueId = uniqueId.Value;
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