using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;

namespace Chaos.Factories.Abstractions;

public interface IPanelObjectFactory<out TPanelObject> where TPanelObject: PanelObjectBase
{
    TPanelObject Create(string templateKey, ICollection<string>? extraScriptKeys = null);
    TPanelObject CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null);
}

public interface IItemFactory : IPanelObjectFactory<Item> { }

public interface ISkillFactory : IPanelObjectFactory<Skill> { }

public interface ISpellFactory : IPanelObjectFactory<Spell> { }