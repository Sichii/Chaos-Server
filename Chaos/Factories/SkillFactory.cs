using System;
using System.Collections.Generic;
using System.Linq;
using Chaos.Caches.Interfaces;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class SkillFactory : ISkillFactory
{
    private readonly ILogger Logger;
    private readonly ISkillScriptFactory SkillScriptFactory;
    private readonly ISimpleCache<string, SkillTemplate> SkillTemplateCache;

    public SkillFactory(
        ISimpleCache<string, SkillTemplate> skillTemplateCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillFactory> logger
    )
    {
        SkillTemplateCache = skillTemplateCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;
    }

    public Skill CreateSkill(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SkillTemplateCache.GetObject(templateKey);
        var skill = new Skill(template);

        var scriptKeys = template.ScriptKeys
            .Concat(extraScriptKeys)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        skill.Script = SkillScriptFactory.CreateScript(scriptKeys, skill);
        skill.UniqueId = ServerId.NextId;

        Logger.LogDebug("Created skill \"{SkillName}\" with unique id \"{UniqueId}\"", skill.Template.Name, skill.UniqueId);

        return skill;
    }
}