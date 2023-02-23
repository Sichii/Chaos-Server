using Chaos.Objects.Menu;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class DialogFactory : IDialogFactory
{
    private readonly ILogger<DialogFactory> Logger;
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

        Logger.LogDebug("Created {@Dialog} for {@Entity}", dialog, source);

        return dialog;
    }
}