using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;

namespace Chaos.Factories.Interfaces;

public interface ISpellFactory
{
    Spell CreateSpell(string templateKey, ICollection<string>? extraScriptKeys = null);
    Spell DeserializeSpell(SerializableSpell serializableSpell);
    Spell CloneSpell(Spell spell);
}