using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Scripts.Interfaces;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates.Interfaces;

namespace Chaos.Templates.Abstractions;

public abstract class PanelObjectTemplateBase : ITemplate, IScripted
{
    public Animation? Animation { get; init; }
    public TimeSpan Cooldown { get; init; } = TimeSpan.Zero;
    public string Name { get; init; } = "REPLACE ME";
    public virtual ushort PanelSprite { get; init; }
    public ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public abstract string TemplateKey { get; init; }

    protected PanelObjectTemplateBase() { }

    protected PanelObjectTemplateBase(PanelObjectTemplateSchema schema, ITypeMapper mapper)
    {
        if (schema.Animation != null)
            Animation = mapper.Map<Animation>(schema.Animation);

        Cooldown = TimeSpan.FromMilliseconds(schema.CooldownMs);
        Name = schema.Name;
        // ReSharper disable once VirtualMemberCallInConstructor
        PanelSprite = schema.PanelSprite;
        ScriptKeys = schema.ScriptKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}