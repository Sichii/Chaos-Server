using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.DialogScripts.Abstractions;

public abstract class DialogScriptBase : SubjectiveScriptBase<Dialog>, IDialogScript
{
    /// <inheritdoc />
    protected DialogScriptBase(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null) { }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source) { }

    public virtual bool TryFetchArg<T>(int index, [MaybeNullWhen(false)] out T input) => Subject.MenuArgs.TryGet(index, out input);

    public virtual bool TryFetchArgs<T>([MaybeNullWhen(false)] out T input) => Subject.MenuArgs.TryGet(0, out input);

    public virtual bool TryFetchArgs<T1, T2>([MaybeNullWhen(false)] out T1 input1, [MaybeNullWhen(false)] out T2 input2)
    {
        input2 = default;

        return Subject.MenuArgs.TryGet(0, out input1) && Subject.MenuArgs.TryGet(1, out input2);
    }
}