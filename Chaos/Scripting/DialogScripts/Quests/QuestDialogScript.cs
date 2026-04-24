#region
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Quests;
using Chaos.Utilities.QuestHelper;
#endregion

namespace Chaos.Scripting.DialogScripts.Quests;

/// <summary>
/// Generic dispatcher that routes dialog lifecycle events to quest handlers registered
/// in the <see cref="IQuestRegistry" />. Attach to a dialog template's <c>scriptKeys</c>
/// as <c>"QuestDialog"</c> (Chaos strips the <c>"Script"</c> suffix) to let declarative
/// <see cref="Quest" /> subclasses react to that dialog.
/// </summary>
public sealed class QuestDialogScript : DialogScriptBase
{
    private readonly IQuestRegistry Registry;
    private readonly IServiceProvider Services;

    public QuestDialogScript(Dialog subject, IQuestRegistry registry, IServiceProvider services)
        : base(subject)
    {
        Registry = registry;
        Services = services;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Dispatch(source);

    /// <inheritdoc />
    public override void OnDisplayed(Aisling source) => Dispatch(source);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null) => Dispatch(source);

    /// <inheritdoc />
    public override void OnPrevious(Aisling source) => Dispatch(source);

    private void Dispatch(Aisling source)
    {
        var handlers = Registry.GetDialogHandlers(Subject.Template.TemplateKey);

        if (handlers.Count == 0)
            return;

        foreach (var handler in handlers)
            ExecuteHandler(handler, source);
    }

    private void ExecuteHandler(DialogQuestHandler handler, Aisling source)
    {
        var context = handler.Quest.CreateContextFor(source, Subject, Services);

        foreach (var op in handler.Operations)
            if (!op(context))
                return; // halt this handler's chain; other handlers still run
    }
}
