using Chaos.Objects.Menu;

namespace Chaos.Factories.Abstractions;

public interface IDialogFactory
{
    public Dialog Create(string templateKey, object source, ICollection<string>? extraScriptKeys = null);
}