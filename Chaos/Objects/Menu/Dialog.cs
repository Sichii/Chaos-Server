using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Menu;

public record Dialog : IScripted<IDialogScript>
{
    private readonly IDialogFactory DialogFactory;
    public List<Item> Items { get; set; }
    public ArgumentCollection MenuArgs { get; set; }
    public string? NextDialogKey { get; set; }
    public List<DialogOption> Options { get; set; }
    public string? PrevDialogKey { get; set; }
    public List<Skill> Skills { get; set; }
    public List<byte>? Slots { get; set; }
    public object SourceEntity { get; set; }
    public List<Spell> Spells { get; set; }
    public DialogTemplate Template { get; set; }
    public string Text { get; set; }
    public ushort? TextBoxLength { get; set; }
    public MenuOrDialogType Type { get; set; }
    /// <inheritdoc />
    public IDialogScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    private Dialog(DialogTemplate template, object sourceEntity)
    {
        DialogFactory = null!;
        Template = template;
        SourceEntity = sourceEntity;
        ScriptKeys = null!;
        Script = null!;
        Items = new List<Item>();
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
    }

    public Dialog(
        DialogTemplate template,
        object sourceEntity,
        IScriptProvider scriptProvider,
        IDialogFactory dialogFactory,
        ICollection<string>? extraScriptKeys = null
    )
        : this(template, sourceEntity)
    {
        extraScriptKeys ??= Array.Empty<string>();

        DialogFactory = dialogFactory;
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IDialogScript, Dialog>(ScriptKeys, this);
    }

    public void Close(Aisling source)
    {
        var closeDialog = new Dialog(DialogTemplate.CloseDialogTemplate, SourceEntity);
        source.Client.SendDialog(closeDialog);
        Script.OnClose(source);
        source.ActiveDialog.TryRemove(this);
    }

    public void Display(Aisling source)
    {
        source.ActiveDialog.Set(this);

        //TODO: REMOVE AFTER TESTING, this should be handled by a script or maybe we can accept some info about what items an npc sells, or what items a dialog can sell
        /*Slots = Type switch
        {
            MenuOrDialogType.ShowPlayerItems  => source.Inventory.Select(i => i.Slot).ToList(),
            MenuOrDialogType.ShowPlayerSkills => source.SkillBook.Select(s => s.Slot).ToList(),
            MenuOrDialogType.ShowPlayerSpells => source.SpellBook.Select(s => s.Slot).ToList(),
            _                                 => null
        };*/

        Script.OnDisplaying(source);
        source.Client.SendDialog(this);
        Script.OnDisplayed(source);
    }

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

    public void Next(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is 0)
            optionIndex = null;

        //for some reasonl some of these menus add a +1 to the pursuit id for no apparent reason
        //we're using the pursuit id as an option selector
        //so for any menu type, option index should be null
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (Type)
        {
            case MenuOrDialogType.ShowItems:
            case MenuOrDialogType.ShowPlayerItems:
            case MenuOrDialogType.ShowSpells:
            case MenuOrDialogType.ShowSkills:
            case MenuOrDialogType.ShowPlayerSpells:
            case MenuOrDialogType.ShowPlayerSkills:
            case MenuOrDialogType.Normal:
            case MenuOrDialogType.DialogTextEntry:
            case MenuOrDialogType.Speak:
            case MenuOrDialogType.CreatureMenu:
            case MenuOrDialogType.Protected:
            case MenuOrDialogType.CloseDialog:
                optionIndex = null;

                break;
        }

        Script.OnNext(source, optionIndex);
        Dialog? nextDialog = null;

        if (!optionIndex.HasValue)
        {
            if (!string.IsNullOrEmpty(NextDialogKey))
                nextDialog = DialogFactory.Create(NextDialogKey, SourceEntity);
        } else
        {
            var option = Options.ElementAtOrDefault(optionIndex.Value - 1);

            if (option != null)
                nextDialog = DialogFactory.Create(option.DialogKey, SourceEntity);
        }

        nextDialog?.Display(source);
    }

    public void Previous(Aisling source)
    {
        Script.OnPrevious(source);

        if (!string.IsNullOrEmpty(PrevDialogKey))
        {
            var prevDialog = DialogFactory.Create(PrevDialogKey, SourceEntity);
            prevDialog.Display(source);
        } else
            Close(source);
    }

    public void Reply(Aisling source, string dialogText)
    {
        Type = MenuOrDialogType.Normal;
        Text = dialogText;
        PrevDialogKey = Template.TemplateKey;
        NextDialogKey = null;
        Options = new List<DialogOption>();

        Display(source);
    }

    public void RequestAmount(Aisling source, string dialogText)
    {
        var newType = MenuOrDialogType.MenuTextEntry;

        if ((MenuArgs != null) && MenuArgs.Any())
            newType = MenuOrDialogType.MenuTextEntryWithArgs;

        Type = newType;
        Text = dialogText;
        PrevDialogKey = Template.TemplateKey;
        NextDialogKey = null;
        Options = new List<DialogOption>();

        Display(source);
    }

    public void RequestConfirmation(
        Aisling source,
        string dialogText,
        string? option1Text = null,
        string? option2Text = null
    )
    {
        var newType = MenuOrDialogType.Menu;

        if (MenuArgs.Any())
            newType = MenuOrDialogType.MenuWithArgs;

        Type = newType;
        Text = dialogText;
        PrevDialogKey = Template.TemplateKey;

        Options = new List<DialogOption>
        {
            new()
            {
                DialogKey = Template.TemplateKey,
                OptionText = option1Text ?? "Yes"
            },
            new()
            {
                DialogKey = Template.TemplateKey,
                OptionText = option2Text ?? "No"
            }
        };

        NextDialogKey = null;

        Display(source);
    }
}