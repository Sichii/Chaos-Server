using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.DialogScripts.Abstractions;

public interface IDialogScript : IScript
{
    void OnDisplayed(Aisling source);
    void OnDisplaying(Aisling source);
    void OnNext(Aisling source, byte? optionIndex = null);
    void OnPrevious(Aisling source);
}