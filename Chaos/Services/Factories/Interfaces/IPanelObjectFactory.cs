using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;

namespace Chaos.Services.Factories.Interfaces;

public interface IPanelObjectFactory<out TPanelObject> where TPanelObject : PanelObjectBase
{
    TPanelObject Create(string templateKey, ICollection<string>? extraScriptKeys = null);
}

public interface IItemFactory : IPanelObjectFactory<Item> { }

public interface ISkillFactory : IPanelObjectFactory<Skill> { }

public interface ISpellFactory : IPanelObjectFactory<Spell> { }