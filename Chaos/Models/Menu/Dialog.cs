using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Models.Menu;

public sealed record Dialog : IScripted<IDialogScript>
{
    private readonly IDialogFactory DialogFactory;
    public object? Context { get; set; }
    public IDialogSourceEntity DialogSource { get; set; }
    public List<ItemDetails> Items { get; set; }
    public ArgumentCollection MenuArgs { get; set; }
    public string? NextDialogKey { get; set; }
    public List<DialogOption> Options { get; set; }
    public string? PrevDialogKey { get; set; }
    public List<Skill> Skills { get; set; }
    public List<byte>? Slots { get; set; }
    public List<Spell> Spells { get; set; }
    public DialogTemplate Template { get; set; }
    public string Text { get; private set; }
    public ushort? TextBoxLength { get; set; }
    public ChaosDialogType Type { get; set; }
    public bool Contextual { get; }
    /// <inheritdoc />
    public IDialogScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    private Dialog(DialogTemplate template, IDialogSourceEntity dialogSource)
    {
        DialogFactory = null!;
        Template = template;
        DialogSource = dialogSource;
        ScriptKeys = null!;
        Script = null!;
        Items = new List<ItemDetails>();
        NextDialogKey = template.NextDialogKey;
        Options = template.Options.ToList();
        PrevDialogKey = template.PrevDialogKey;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Text = template.Text;
        TextBoxLength = template.TextBoxLength;
        Type = template.Type;
        MenuArgs = new ArgumentCollection();
        Contextual = template.Contextual;
    }

    public Dialog(
        DialogTemplate template,
        IDialogSourceEntity dialogSource,
        IScriptProvider scriptProvider,
        IDialogFactory dialogFactory,
        ICollection<string>? extraScriptKeys = null
    )
        : this(template, dialogSource)
    {
        extraScriptKeys ??= Array.Empty<string>();

        DialogFactory = dialogFactory;
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IDialogScript, Dialog>(ScriptKeys, this);
    }

    public Dialog(
        IDialogSourceEntity dialogSource,
        IDialogFactory dialogFactory,
        ChaosDialogType type,
        string text
    )
    {
        Text = text;
        Type = type;
        DialogSource = dialogSource;
        Template = null!;
        DialogFactory = dialogFactory;
        TextBoxLength = null;
        NextDialogKey = null;
        PrevDialogKey = null;
        Script = new CompositeDialogScript();
        Options = new List<DialogOption>();
        ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Items = new List<ItemDetails>();
        MenuArgs = new ArgumentCollection();
    }

    public void Close(Aisling source)
    {
        Type = ChaosDialogType.CloseDialog;
        Options.Clear();
        NextDialogKey = null;
        source.Client.SendDialog(this);
        source.ActiveDialog.TryRemove(this);
        source.DialogHistory.Clear();
    }

    public void Display(Aisling source)
    {
        source.ActiveDialog.Set(this);

        Script.OnDisplaying(source);

        //if a different dialog was displayed while this one was being displayed
        if (source.ActiveDialog.Get() != this)
            return;

        if (!Text.EqualsI("skip"))
            source.Client.SendDialog(this);

        Script.OnDisplayed(source);

        //if a different dialog was displayed while this one was being displayed
        if (source.ActiveDialog.Get() != this)
            return;

        if (Text.EqualsI("skip"))
            if (!string.IsNullOrEmpty(NextDialogKey))
                Next(source);
            else
                Close(source);
    }

    public void InjectTextParameters(params object[] parameters) => Text = Text.Inject(parameters);

    public void Next(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is 0)
            optionIndex = null;

        //for some reason some of these types add a +1 to the pursuit id when you respond
        //we're using the pursuit id as an option selector
        //so for any non-menu type, option index should be null (because there are no options)
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (Type)
        {
            case ChaosDialogType.ShowItems:
            case ChaosDialogType.ShowPlayerItems:
            case ChaosDialogType.ShowSpells:
            case ChaosDialogType.ShowSkills:
            case ChaosDialogType.ShowPlayerSpells:
            case ChaosDialogType.ShowPlayerSkills:
            case ChaosDialogType.Normal:
            case ChaosDialogType.DialogTextEntry:
            case ChaosDialogType.Speak:
            case ChaosDialogType.CreatureMenu:
            case ChaosDialogType.Protected:
            case ChaosDialogType.CloseDialog:
                optionIndex = null;

                break;
        }

        source.DialogHistory.Push(this);
        Script.OnNext(source, optionIndex);

        //if a different dialog was displayed, do not continue this dialog
        if (source.ActiveDialog.Get() != this)
            return;

        var nextDialogKey = optionIndex.HasValue ? Options.ElementAtOrDefault(optionIndex.Value - 1)?.DialogKey : NextDialogKey;

        if (!string.IsNullOrEmpty(nextDialogKey))
        {
            if (nextDialogKey.EqualsI("close"))
            {
                Close(source);

                return;
            }

            if (nextDialogKey.EqualsI("top"))
            {
                if (DialogSource is MapEntity mapEntity && !mapEntity.WithinRange(source))
                {
                    Close(source);

                    return;
                }

                DialogSource.Activate(source);

                return;
            }

            var nextDialog = DialogFactory.Create(nextDialogKey, DialogSource);

            if (nextDialog.Contextual)
            {
                nextDialog.MenuArgs = new ArgumentCollection(MenuArgs);
                nextDialog.Context = DeepClone.Create(Context);
            }

            nextDialog.Display(source);
        }
    }

    public void Previous(Aisling source)
    {
        //if no prev dialog key, close
        //if source is a map entity that's out of range, close
        if (string.IsNullOrEmpty(PrevDialogKey) || (DialogSource is MapEntity mapEntity && !mapEntity.WithinRange(source)))
            Close(source);
        else
        {
            var prevDialog = source.DialogHistory.PopUntil(d => d.Template.TemplateKey.EqualsI(PrevDialogKey));
            source.DialogHistory.TryPeek(out var prevPrevDialog);

            //if PreviousDialogKey references something that didn't happen, close the dialog
            if (prevDialog is null)
            {
                Close(source);

                throw new InvalidOperationException(
                    $"Attempted to from dialogKey \"{Template.TemplateKey}\" to dialogKey \"{PrevDialogKey
                    }\" but no dialog with that key was found in the history.");
            }

            var newPrevDialog = DialogFactory.Create(PrevDialogKey, DialogSource);

            //if the dialog is contextual, copy the context and menu args from the previous dialog
            if (newPrevDialog.Contextual)
            {
                newPrevDialog.MenuArgs = new ArgumentCollection(prevPrevDialog?.MenuArgs);
                newPrevDialog.Context = DeepClone.Create(prevPrevDialog?.Context);
            }

            Script.OnPrevious(source);
            prevDialog.Display(source);
        }
    }

    public void Reply(Aisling source, string dialogText, string? nextDialogKey = null)
    {
        var newDialog = new Dialog(
            DialogSource,
            DialogFactory,
            ChaosDialogType.Normal,
            dialogText)
        {
            NextDialogKey = nextDialogKey
        };

        newDialog.Display(source);
    }

    public void ReplyToUnknownInput(Aisling source) => Reply(source, DialogString.UnknownInput.Value);

    #region Dialog Options
    public int? GetOptionIndex(string optionText)
    {
        var index = Options.FindIndex(option => option.OptionText.EqualsI(optionText));

        return index == -1 ? null : index;
    }

    public string? GetOptionText(int optionIndex)
    {
        var option = Options.ElementAtOrDefault(optionIndex);

        return option?.OptionText;
    }

    public bool HasOption(string optionText) => GetOptionIndex(optionText) is not null;

    public DialogOption GetOption(string optionText) => Options.First(option => option.OptionText.EqualsI(optionText));

    public void InsertOption(int index, string optionText, string dialogKey)
    {
        if (index >= Options.Count)
            AddOption(optionText, dialogKey);
        else
            Options.Insert(index, new DialogOption { OptionText = optionText, DialogKey = dialogKey });
    }

    public void AddOption(string optionText, string dialogKey) =>
        Options.Add(new DialogOption { OptionText = optionText, DialogKey = dialogKey });

    public void AddOptions(params (string OptionText, string DialogKey)[] options) => Options.AddRange(
        options.Select(option => new DialogOption { OptionText = option.OptionText, DialogKey = option.DialogKey }));
    #endregion
}