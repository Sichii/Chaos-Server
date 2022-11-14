using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public sealed class DialogFactory : IDialogFactory
{
    private readonly ILogger Logger;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public DialogFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider, ILogger<DialogFactory> logger)
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    /// <inheritdoc />
    public Dialog Create(string templateKey, object source, ICollection<string>? extraScriptKeys = null)
    {
        var template = SimpleCache.Get<DialogTemplate>(templateKey);

        var dialog = new Dialog(
            template,
            source,
            ScriptProvider,
            this,
            extraScriptKeys);

        Logger.LogTrace("Created dialog {DialogKey} for {Source}", templateKey, source);

        return dialog;
    }
}