#region
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
#endregion

namespace Chaos.Services.Factories.Abstractions;

public interface IPanelEntityFactory<out T> where T: PanelEntityBase
{
    T Create(string templateKey, ICollection<string>? extraScriptKeys = null);
    T CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null);
    T CreateScriptProxy(ICollection<string>? extraScriptKeys = null);
}

public interface IItemFactory : IPanelEntityFactory<Item>;
public interface ISkillFactory : IPanelEntityFactory<Skill>;
public interface ISpellFactory : IPanelEntityFactory<Spell>;