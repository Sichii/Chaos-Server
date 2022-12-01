using Chaos.Objects.Menu;

namespace Chaos.Services.Factories.Abstractions;

public interface IDialogFactory
{
    public Dialog Create(string templateKey, object source, ICollection<string>? extraScriptKeys = null);
}