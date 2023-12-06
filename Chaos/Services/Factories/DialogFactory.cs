using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class DialogFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider) : IDialogFactory
{
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

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