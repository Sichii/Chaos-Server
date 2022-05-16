using Chaos.Objects.Panel;

namespace Chaos.Factories.Interfaces;

public interface ISpellFactory
{
    Spell CreateSpell(string templateKey, ICollection<string>? extraScriptKeys = null);
}