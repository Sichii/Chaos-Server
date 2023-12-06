using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("learn", helpText: "<spell|skill> <templateKey>")]
public class LearnCommand(ISpellFactory spellFactory, ISkillFactory skillFactory) : ICommand<Aisling>
{
    private readonly ISkillFactory SkillFactory = skillFactory;
    private readonly ISpellFactory SpellFactory = spellFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        if (!args.TryGetNext<string>(out var templateKey))
            return default;

        switch (type.ToLower())
        {
            case "spell":
                var spell = SpellFactory.Create(templateKey);

                source.SpellBook.TryAddToNextSlot(spell);

                break;
            case "skill":
                var skill = SkillFactory.Create(templateKey);

                source.SkillBook.TryAddToNextSlot(skill);

                break;
        }

        return default;
    }
}