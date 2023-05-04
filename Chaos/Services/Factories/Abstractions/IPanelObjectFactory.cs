using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IPanelObjectFactory<out TPanelObject> where TPanelObject: PanelEntityBase
{
    TPanelObject Create(string templateKey, ICollection<string>? extraScriptKeys = null);
    TPanelObject CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null);
}

public interface IItemFactory : IPanelObjectFactory<Item> { }

public interface ISkillFactory : IPanelObjectFactory<Skill> { }

public interface ISpellFactory : IPanelObjectFactory<Spell> { }