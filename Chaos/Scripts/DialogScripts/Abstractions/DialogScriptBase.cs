using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.DialogScripts.Abstractions;

public abstract class DialogScriptBase : SubjectiveScriptBase<Dialog>, IDialogScript
{
    private readonly ConcurrentDictionary<Delegate, bool> RunOnceCache = new();

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

    protected void RunOnce(Action action)
    {
        if (RunOnceCache.TryAdd(action, true))
            action();
    }

    protected T? RunOnce<T>(Func<T> func) => RunOnceCache.TryAdd(func, true) ? func() : default;
}