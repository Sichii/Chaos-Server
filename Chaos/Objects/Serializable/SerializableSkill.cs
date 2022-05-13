using System;
using System.Collections.Generic;

namespace Chaos.Objects.Serializable;

public record SerializableSkill
{
    public int ElapsedMs { get; set; }
    public ICollection<string> ScriptKeys { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public string TemplateKey { get; set; } = null!;
}