#region
using Chaos.Common.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockSkill
{
    public static Skill Create(
        string name = "TestSkill",
        bool isAssail = false,
        Func<SkillTemplate, SkillTemplate>? templateSetup = null,
        Action<Skill>? setup = null)
    {
        var template = new SkillTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            PanelSprite = 1,
            MaxLevel = 100,
            LevelsUp = false,
            IsAssail = isAssail,
            LearningRequirements = null,
            Level = 1,
            AbilityLevel = 0,
            Class = null,
            AdvClass = null,
            Cooldown = null,
            Description = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        if (templateSetup is not null)
            template = templateSetup(template);

        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        setup?.Invoke(skill);

        return skill;
    }
}