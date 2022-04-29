using System;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Templates.Interfaces;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate<string>
{
    public Animation Animation { get; init; } = Animation.None;
    public TimeSpan BaseCooldown { get; init; } = TimeSpan.Zero;
    public int BaseDamage { get; init; } = 0;
    public BodyAnimation BodyAnimation { get; init; } = BodyAnimation.None;
    public string Name { get; init; } = "REPLACE ME";
    public virtual ushort Sprite { get; init; } = 0;
    public TargetsType TargetType { get; init; } = TargetsType.None;
    public abstract string TemplateKey { get; init; }
}