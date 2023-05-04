using Chaos.Models.Abstractions;
using Chaos.Models.Menu;

namespace Chaos.Services.Factories.Abstractions;

public interface IDialogFactory
{
    public Dialog Create(string templateKey, IDialogSourceEntity source, ICollection<string>? extraScriptKeys = null);
}