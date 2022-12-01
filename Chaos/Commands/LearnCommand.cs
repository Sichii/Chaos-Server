using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands;

[Command("learn")]
public class LearnCommand : ICommand<Aisling>
{
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    public LearnCommand(ISpellFactory spellFactory, ISkillFactory skillFactory)
    {
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        if (!args.TryGetNext<string>(out var name))
            return default;

        switch (type.ToLower())
        {
            case "spell":
                var spell = SpellFactory.Create(name);

                source.SpellBook.TryAddToNextSlot(spell);

                break;
            case "skill":
                var skill = SkillFactory.Create(name);

                source.SkillBook.TryAddToNextSlot(skill);

                break;
        }

        return default;
    }
}