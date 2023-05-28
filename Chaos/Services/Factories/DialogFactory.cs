using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class DialogFactory : IDialogFactory
{
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public DialogFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider)
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
    }

    /// <inheritdoc />
    public Dialog Create(string templateKey, IDialogSourceEntity source, ICollection<string>? extraScriptKeys = null)
    {
        var template = SimpleCache.Get<DialogTemplate>(templateKey);

        var dialog = new Dialog(
            template,
            source,
            ScriptProvider,
            this,
            extraScriptKeys);

        return dialog;
    }
}