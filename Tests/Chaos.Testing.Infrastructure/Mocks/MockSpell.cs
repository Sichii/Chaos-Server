#region
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockSpell
{
    public static Spell Create(
        string name = "TestSpell",
        byte castLines = 0,
        Func<SpellTemplate, SpellTemplate>? templateSetup = null,
        Action<Spell>? setup = null)
    {
        var template = new SpellTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            PanelSprite = 1,
            MaxLevel = 100,
            LevelsUp = false,
            CastLines = castLines,
            LearningRequirements = null,
            Prompt = null,
            SpellType = SpellType.None,
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

        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        setup?.Invoke(spell);

        return spell;
    }
}