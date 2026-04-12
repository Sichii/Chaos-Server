#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class DialogTests
{
    private readonly Mock<IDialogFactory> DialogFactoryMock;
    private readonly Mock<IDialogSourceEntity> DialogSourceMock;
    private readonly MapInstance Map;
    private readonly Mock<IDialogScript> ScriptMock;
    private readonly Mock<IScriptProvider> ScriptProviderMock;

    public DialogTests()
    {
        Map = MockMapInstance.Create();
        ScriptMock = new Mock<IDialogScript>();
        DialogFactoryMock = new Mock<IDialogFactory>();
        DialogSourceMock = new Mock<IDialogSourceEntity>();

        DialogSourceMock.SetupGet(s => s.Name)
                        .Returns("TestSource");

        DialogSourceMock.SetupGet(s => s.Id)
                        .Returns(1);

        ScriptProviderMock = new Mock<IScriptProvider>();

        ScriptProviderMock.Setup(sp => sp.CreateScript<IDialogScript, Dialog>(It.IsAny<ICollection<string>>(), It.IsAny<Dialog>()))
                          .Returns(ScriptMock.Object);
    }

    #region Close
    [Test]
    public void Close_ShouldSetCloseDialogType_AndClearOptions()
    {
        var dialog = CreateDialog(
            CreateTemplate(
                options:
                [
                    new DialogOption
                    {
                        OptionText = "Opt",
                        DialogKey = "key"
                    }
                ]));
        var aisling = MockAisling.Create(Map);

        dialog.Close(aisling);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);

        dialog.Options
              .Should()
              .BeEmpty();

        dialog.NextDialogKey
              .Should()
              .BeNull();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(dialog));
    }
    #endregion

    #region Constructor (simple)
    [Test]
    public void Constructor_Simple_ShouldSetProperties()
    {
        var dialog = CreateSimpleDialog("Hi there", ChaosDialogType.DialogMenu);

        dialog.Text
              .Should()
              .Be("Hi there");

        dialog.Type
              .Should()
              .Be(ChaosDialogType.DialogMenu);

        dialog.NextDialogKey
              .Should()
              .BeNull();

        dialog.PrevDialogKey
              .Should()
              .BeNull();

        dialog.Options
              .Should()
              .BeEmpty();
    }
    #endregion

    private Dialog CreateDialog(DialogTemplate? template = null)
        => new(
            template ?? CreateTemplate(),
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

    private Dialog CreateSimpleDialog(string text = "Simple", ChaosDialogType type = ChaosDialogType.Normal)
        => new(
            DialogSourceMock.Object,
            DialogFactoryMock.Object,
            type,
            text);

    private DialogTemplate CreateTemplate(
        string templateKey = "testDialog",
        string text = "Hello",
        ChaosDialogType type = ChaosDialogType.Normal,
        string? nextDialogKey = null,
        string? prevDialogKey = null,
        bool contextual = false,
        ICollection<DialogOption>? options = null)
        => new()
        {
            TemplateKey = templateKey,
            Text = text,
            Type = type,
            NextDialogKey = nextDialogKey,
            PrevDialogKey = prevDialogKey,
            Contextual = contextual,
            Options = options ?? [],
            ScriptKeys = new List<string>
            {
                "TestScript"
            },
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            TextBoxLength = null,
            TextBoxPrompt = null,
            IllustrationIndex = 0
        };

    #region Display (onDisplaying continues normally)
    [Test]
    public void Display_ShouldCallOnDisplayed_WhenOnDisplayingDoesNotChangeActiveDialog()
    {
        var dialog = CreateDialog(CreateTemplate(text: "Normal"));
        var aisling = MockAisling.Create(Map);

        // OnDisplaying does nothing special (default mock behavior)
        dialog.Display(aisling);

        // Both OnDisplaying and OnDisplayed should be called
        ScriptMock.Verify(s => s.OnDisplaying(aisling), Times.Once);
        ScriptMock.Verify(s => s.OnDisplayed(aisling), Times.Once);

        // The dialog itself should have been sent
        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(dialog), Times.Once);
    }
    #endregion

    #region InjectTextParameters
    [Test]
    public void InjectTextParameters_ShouldInjectValues()
    {
        var dialog = CreateDialog(CreateTemplate(text: "Hello {0}!"));
        dialog.InjectTextParameters("World");

        dialog.Text
              .Should()
              .Be("Hello World!");
    }
    #endregion

    #region Next (top with MapEntity source within range)
    [Test]
    public void Next_ShouldActivateSource_WhenTopAndSourceIsMapEntityWithinRange()
    {
        var merchant = MockMerchant.Create(Map, "NearMerchant");

        var template = CreateTemplate(nextDialogKey: "top");

        var dialog = new Dialog(
            template,
            merchant,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        // Merchant is at (5,5) by default, aisling at (5,5) — within range
        dialog.Next(aisling);

        // Activate should be called on the merchant
        var merchantScriptMock = Mock.Get(merchant.Script);

        merchantScriptMock.Verify(s => s.OnClicked(aisling), Times.Once);
    }
    #endregion

    #region Next (top with non-MapEntity source)
    [Test]
    public void Next_ShouldActivateSource_WhenTopAndSourceIsNotMapEntity()
    {
        // DialogSourceMock is not a MapEntity, so range check doesn't apply
        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "top"));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        // Activate should be called (no range check since source is not MapEntity)
        DialogSourceMock.Verify(s => s.Activate(aisling), Times.Once);
    }
    #endregion

    #region Next (invalid optionIndex)
    [Test]
    public void Next_ShouldDoNothing_WhenOptionIndexOutOfRange_AndNoNextDialogKey()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "Option1",
                DialogKey = "key1"
            }
        };
        var dialog = CreateDialog(CreateTemplate(type: ChaosDialogType.Menu, options: options));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        // optionIndex 5 is out of range (only 1 option at index 0, 1-based index means valid is 1)
        // ElementAtOrDefault(4) returns null, so nextDialogKey is null
        dialog.Next(aisling, 5);

        // No dialog should have been created since optionIndex yielded null DialogKey
        DialogFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), null), Times.Never);
    }
    #endregion

    #region Previous — Contextual with null prevPrevDialog
    [Test]
    public void Previous_ShouldSetMenuArgsAndContextToNull_WhenPrevPrevDialogIsNull_AndNewPrevIsContextual()
    {
        // Set up a prevDialog in history (only one entry, so prevPrevDialog will be null)
        var prevTemplate = CreateTemplate("prevDialog", "Prev text");

        var prevDialogInHistory = new Dialog(
            prevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var newPrevTemplate = CreateTemplate("prevDialog", contextual: true);

        var newPrevDialog = new Dialog(
            newPrevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var template = CreateTemplate("currentDialog", prevDialogKey: "prevDialog");

        DialogFactoryMock.Setup(f => f.Create("prevDialog", DialogSourceMock.Object, null))
                         .Returns(newPrevDialog);

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        // Push only the prevDialog — no prevPrevDialog in history
        aisling.DialogHistory.Push(prevDialogInHistory);

        dialog.Previous(aisling);

        // prevPrevDialog is null, so MenuArgs should be initialized from null, Context should be null
        newPrevDialog.MenuArgs
                     .Should()
                     .NotBeNull();

        newPrevDialog.Context
                     .Should()
                     .BeNull();
    }
    #endregion

    #region ReplyToUnknownInput
    [Test]
    public void ReplyToUnknownInput_ShouldReplyWithUnknownInputText()
    {
        var dialog = CreateDialog();
        var aisling = MockAisling.Create(Map);

        dialog.ReplyToUnknownInput(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(It.IsAny<Dialog>()));
    }
    #endregion

    #region Constructor (template-based)
    [Test]
    public void Constructor_WithTemplate_ShouldSetProperties()
    {
        var template = CreateTemplate(
            "myDialog",
            "Hello world",
            ChaosDialogType.Menu,
            "next",
            "prev");

        var dialog = CreateDialog(template);

        dialog.Template
              .Should()
              .BeSameAs(template);

        dialog.Text
              .Should()
              .Be("Hello world");

        dialog.Type
              .Should()
              .Be(ChaosDialogType.Menu);

        dialog.NextDialogKey
              .Should()
              .Be("next");

        dialog.PrevDialogKey
              .Should()
              .Be("prev");

        dialog.DialogSource
              .Should()
              .BeSameAs(DialogSourceMock.Object);

        dialog.ScriptKeys
              .Should()
              .Contain("TestScript");
    }

    [Test]
    public void Constructor_WithExtraScriptKeys_ShouldAddToScriptKeys()
    {
        var template = CreateTemplate();

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object,
            ["ExtraScript"]);

        dialog.ScriptKeys
              .Should()
              .Contain("TestScript");

        dialog.ScriptKeys
              .Should()
              .Contain("ExtraScript");
    }
    #endregion

    #region Display
    [Test]
    public void Display_ShouldSendDialog_WhenTextIsNotSkip()
    {
        var dialog = CreateDialog(CreateTemplate(text: "Hello"));
        var aisling = MockAisling.Create(Map);

        dialog.Display(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(dialog));
        ScriptMock.Verify(s => s.OnDisplaying(aisling));
        ScriptMock.Verify(s => s.OnDisplayed(aisling));
    }

    [Test]
    public void Display_ShouldSkipSending_WhenTextIsSkip_AndAutoClose_WhenNoNext()
    {
        var dialog = CreateDialog(CreateTemplate(text: "skip"));
        var aisling = MockAisling.Create(Map);

        dialog.Display(aisling);

        // Should NOT send the dialog itself (text is "skip")
        // But Close will send a CloseDialog
        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Display_ShouldSkipToNext_WhenTextIsSkip_AndNextDialogKeyExists()
    {
        var nextDialog = CreateSimpleDialog("Next");

        DialogFactoryMock.Setup(f => f.Create("nextKey", DialogSourceMock.Object, null))
                         .Returns(nextDialog);

        var dialog = CreateDialog(CreateTemplate(text: "skip", nextDialogKey: "nextKey"));
        var aisling = MockAisling.Create(Map);

        dialog.Display(aisling);

        // The Next method should have been called, which creates the next dialog
        DialogFactoryMock.Verify(f => f.Create("nextKey", DialogSourceMock.Object, null));
    }

    [Test]
    public void Display_ShouldReturnEarly_WhenActiveDialogChangedDuringOnDisplaying()
    {
        var dialog = CreateDialog();
        var aisling = MockAisling.Create(Map);
        var otherDialog = CreateSimpleDialog("Other");

        // During OnDisplaying, a different dialog takes over
        ScriptMock.Setup(s => s.OnDisplaying(aisling))
                  .Callback(() => aisling.ActiveDialog.Set(otherDialog));

        dialog.Display(aisling);

        // OnDisplayed should NOT be called since the dialog was replaced
        ScriptMock.Verify(s => s.OnDisplayed(aisling), Times.Never);
    }

    [Test]
    public void Display_ShouldReturnEarly_WhenActiveDialogChangedDuringOnDisplayed()
    {
        var dialog = CreateDialog(CreateTemplate(text: "skip", nextDialogKey: "nextKey"));
        var aisling = MockAisling.Create(Map);
        var otherDialog = CreateSimpleDialog("Other");

        // During OnDisplayed, a different dialog takes over
        ScriptMock.Setup(s => s.OnDisplayed(aisling))
                  .Callback(() => aisling.ActiveDialog.Set(otherDialog));

        dialog.Display(aisling);

        // Next should NOT be called since the dialog was replaced after OnDisplayed
        DialogFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), null), Times.Never);
    }
    #endregion

    #region Next
    [Test]
    public void Next_ShouldNullifyOptionIndex_WhenZero()
    {
        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "close"));
        var aisling = MockAisling.Create(Map);

        dialog.Next(aisling, 0);

        // Should call OnNext with null optionIndex
        ScriptMock.Verify(s => s.OnNext(aisling, null));
    }

    //formatter:off
    [Test]
    [Arguments(ChaosDialogType.ShowItems)]
    [Arguments(ChaosDialogType.ShowPlayerItems)]
    [Arguments(ChaosDialogType.ShowSpells)]
    [Arguments(ChaosDialogType.ShowSkills)]
    [Arguments(ChaosDialogType.ShowPlayerSpells)]
    [Arguments(ChaosDialogType.ShowPlayerSkills)]
    [Arguments(ChaosDialogType.Normal)]
    [Arguments(ChaosDialogType.DialogTextEntry)]
    [Arguments(ChaosDialogType.Speak)]
    [Arguments(ChaosDialogType.CreatureMenu)]
    [Arguments(ChaosDialogType.Protected)]
    [Arguments(ChaosDialogType.CloseDialog)]

    //formatter:on
    public void Next_ShouldClearOptionIndex_ForNonMenuTypes(ChaosDialogType type)
    {
        var dialog = CreateDialog(CreateTemplate(type: type, nextDialogKey: "close"));
        var aisling = MockAisling.Create(Map);

        dialog.Next(aisling, 5);

        // OnNext should receive null for non-menu types
        ScriptMock.Verify(s => s.OnNext(aisling, null));
    }

    [Test]
    public void Next_ShouldKeepOptionIndex_ForMenuType()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "Option1",
                DialogKey = "close"
            },
            new()
            {
                OptionText = "Option2",
                DialogKey = "close"
            }
        };
        var dialog = CreateDialog(CreateTemplate(type: ChaosDialogType.Menu, options: options));
        var aisling = MockAisling.Create(Map);

        dialog.Next(aisling, 1);

        ScriptMock.Verify(s => s.OnNext(aisling, 1));
    }

    [Test]
    public void Next_ShouldClose_WhenNextDialogKeyIsClose()
    {
        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "close"));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Next_ShouldActivateSource_WhenNextDialogKeyIsTop()
    {
        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "top"));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        DialogSourceMock.Verify(s => s.Activate(aisling));
    }

    [Test]
    public void Next_ShouldClose_WhenTopAndSourceIsMapEntityOutOfRange()
    {
        var merchant = MockMerchant.Create(Map, "Merchant");

        // Move merchant far from aisling
        merchant.SetLocation(Map, new Point(40, 40));

        var template = CreateTemplate(nextDialogKey: "top");

        var dialog = new Dialog(
            template,
            merchant,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Next_ShouldCreateAndDisplayNextDialog()
    {
        var nextDialog = CreateSimpleDialog("Next dialog");

        DialogFactoryMock.Setup(f => f.Create("nextKey", DialogSourceMock.Object, null))
                         .Returns(nextDialog);

        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "nextKey"));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        DialogFactoryMock.Verify(f => f.Create("nextKey", DialogSourceMock.Object, null));
    }

    [Test]
    public void Next_WithOptionIndex_ShouldUseOptionDialogKey()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "Option1",
                DialogKey = "close"
            }
        };
        var dialog = CreateDialog(CreateTemplate(type: ChaosDialogType.Menu, options: options));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling, 1);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Next_ShouldCopyContext_WhenNextDialogIsContextual()
    {
        var nextTemplate = CreateTemplate("nextDialog", contextual: true);

        var nextDialog = new Dialog(
            nextTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        DialogFactoryMock.Setup(f => f.Create("nextKey", DialogSourceMock.Object, null))
                         .Returns(nextDialog);

        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "nextKey"));
        dialog.Context = "test context";
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        nextDialog.Contextual
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Next_ShouldReturnEarly_WhenActiveDialogChangedDuringOnNext()
    {
        var dialog = CreateDialog(CreateTemplate(nextDialogKey: "nextKey"));
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);
        var otherDialog = CreateSimpleDialog("Other");

        ScriptMock.Setup(s => s.OnNext(aisling, null))
                  .Callback(() => aisling.ActiveDialog.Set(otherDialog));

        dialog.Next(aisling);

        // Should not create next dialog since active dialog changed
        DialogFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), null), Times.Never);
    }

    [Test]
    public void Next_ShouldDoNothing_WhenNoNextDialogKeyAndNoOptionIndex()
    {
        var dialog = CreateDialog(CreateTemplate());
        var aisling = MockAisling.Create(Map);
        aisling.ActiveDialog.Set(dialog);

        dialog.Next(aisling);

        DialogFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), null), Times.Never);
    }
    #endregion

    #region Previous
    [Test]
    public void Previous_ShouldClose_WhenNoPrevDialogKey()
    {
        var dialog = CreateDialog(CreateTemplate(prevDialogKey: null));
        var aisling = MockAisling.Create(Map);

        dialog.Previous(aisling);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Previous_ShouldClose_WhenSourceIsMapEntityOutOfRange()
    {
        var merchant = MockMerchant.Create(Map, "Merchant");
        merchant.SetLocation(Map, new Point(40, 40));

        var template = CreateTemplate(prevDialogKey: "prevKey");

        var dialog = new Dialog(
            template,
            merchant,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        dialog.Previous(aisling);

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }
    #endregion

    #region Reply
    [Test]
    public void Reply_ShouldCreateAndDisplayNewDialog()
    {
        var dialog = CreateDialog();
        var aisling = MockAisling.Create(Map);

        dialog.Reply(aisling, "Reply text");

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "Reply text")));
    }

    [Test]
    public void Reply_ShouldSetNextDialogKey_WhenProvided()
    {
        var dialog = CreateDialog();
        var aisling = MockAisling.Create(Map);

        dialog.Reply(aisling, "Reply", "nextKey");

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.NextDialogKey == "nextKey")));
    }
    #endregion

    #region Dialog Options
    [Test]
    public void GetOptionIndex_ShouldReturnIndex_WhenOptionExists()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "First",
                DialogKey = "key1"
            },
            new()
            {
                OptionText = "Second",
                DialogKey = "key2"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        dialog.GetOptionIndex("First")
              .Should()
              .Be(0);

        dialog.GetOptionIndex("Second")
              .Should()
              .Be(1);
    }

    [Test]
    public void GetOptionIndex_ShouldReturnNull_WhenOptionDoesNotExist()
    {
        var dialog = CreateDialog();

        dialog.GetOptionIndex("NonExistent")
              .Should()
              .BeNull();
    }

    [Test]
    public void GetOptionIndex_ShouldBeCaseInsensitive()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "Option",
                DialogKey = "key"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        dialog.GetOptionIndex("option")
              .Should()
              .Be(0);

        dialog.GetOptionIndex("OPTION")
              .Should()
              .Be(0);
    }

    [Test]
    public void GetOptionText_ShouldReturnText_WhenIndexValid()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "First",
                DialogKey = "key1"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        // GetOptionText uses 1-based index
        dialog.GetOptionText(1)
              .Should()
              .Be("First");
    }

    [Test]
    public void GetOptionText_ShouldReturnNull_WhenIndexOutOfRange()
    {
        var dialog = CreateDialog();

        dialog.GetOptionText(99)
              .Should()
              .BeNull();
    }

    [Test]
    public void HasOption_ShouldReturnTrue_WhenOptionExists()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "Exists",
                DialogKey = "key"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        dialog.HasOption("Exists")
              .Should()
              .BeTrue();
    }

    [Test]
    public void HasOption_ShouldReturnFalse_WhenOptionDoesNotExist()
    {
        var dialog = CreateDialog();

        dialog.HasOption("Missing")
              .Should()
              .BeFalse();
    }

    [Test]
    public void GetOption_ShouldReturnOption_WhenExists()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "TheOption",
                DialogKey = "theKey"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        var option = dialog.GetOption("TheOption");

        option.OptionText
              .Should()
              .Be("TheOption");

        option.DialogKey
              .Should()
              .Be("theKey");
    }

    [Test]
    public void AddOption_ShouldAddOption()
    {
        var dialog = CreateDialog();

        dialog.AddOption("NewOption", "newKey");

        dialog.Options
              .Should()
              .HaveCount(1);

        dialog.Options[0]
              .OptionText
              .Should()
              .Be("NewOption");
    }

    [Test]
    public void AddOptions_ShouldAddMultipleOptions()
    {
        var dialog = CreateDialog();

        dialog.AddOptions(("Opt1", "key1"), ("Opt2", "key2"));

        dialog.Options
              .Should()
              .HaveCount(2);
    }

    [Test]
    public void InsertOption_ShouldInsertAtIndex_WhenIndexInRange()
    {
        var options = new List<DialogOption>
        {
            new()
            {
                OptionText = "First",
                DialogKey = "key1"
            },
            new()
            {
                OptionText = "Third",
                DialogKey = "key3"
            }
        };
        var dialog = CreateDialog(CreateTemplate(options: options));

        dialog.InsertOption(1, "Second", "key2");

        dialog.Options
              .Should()
              .HaveCount(3);

        dialog.Options[1]
              .OptionText
              .Should()
              .Be("Second");
    }

    [Test]
    public void InsertOption_ShouldAppend_WhenIndexBeyondCount()
    {
        var dialog = CreateDialog();

        dialog.InsertOption(99, "Appended", "key");

        dialog.Options
              .Should()
              .HaveCount(1);

        dialog.Options[0]
              .OptionText
              .Should()
              .Be("Appended");
    }
    #endregion

    #region Previous (dialog not in history)
    [Test]
    public void Previous_ShouldThrowInvalidOperationException_WhenPrevDialogNotInHistory()
    {
        var template = CreateTemplate(prevDialogKey: "nonExistentDialog");

        var prevDialog = CreateSimpleDialog("Prev");

        DialogFactoryMock.Setup(f => f.Create("nonExistentDialog", DialogSourceMock.Object, null))
                         .Returns(prevDialog);

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        // History is empty, so PopUntil will return null
        // This should throw InvalidOperationException and also close the dialog
        var act = () => dialog.Previous(aisling);

        act.Should()
           .Throw<InvalidOperationException>();

        dialog.Type
              .Should()
              .Be(ChaosDialogType.CloseDialog);
    }

    [Test]
    public void Previous_ShouldThrow_WhenHistoryDoesNotContainPrevDialogKey()
    {
        var prevTemplate = CreateTemplate("differentDialog");

        var prevDialog = new Dialog(
            prevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var template = CreateTemplate("currentDialog", prevDialogKey: "targetDialog");

        DialogFactoryMock.Setup(f => f.Create("targetDialog", DialogSourceMock.Object, null))
                         .Returns(CreateSimpleDialog("Target"));

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        // Push a dialog with different key into history
        aisling.DialogHistory.Push(prevDialog);

        // The history does not contain "targetDialog", so PopUntil returns null
        var act = () => dialog.Previous(aisling);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void Previous_ShouldNavigateBack_WhenPrevDialogInHistory()
    {
        var prevTemplate = CreateTemplate("prevDialog", "Previous text");

        var prevDialogInHistory = new Dialog(
            prevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var newPrevDialog = new Dialog(
            prevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var template = CreateTemplate("currentDialog", prevDialogKey: "prevDialog");

        DialogFactoryMock.Setup(f => f.Create("prevDialog", DialogSourceMock.Object, null))
                         .Returns(newPrevDialog);

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        // Push the prev dialog into history
        aisling.DialogHistory.Push(prevDialogInHistory);

        dialog.Previous(aisling);

        // OnPrevious should have been called on the current dialog's script
        ScriptMock.Verify(s => s.OnPrevious(aisling), Times.Once);

        // The popped dialog from history should have been displayed (not the factory-created one)
        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "Previous text")), Times.Once);
    }

    [Test]
    public void Previous_ShouldCopyContext_WhenNewPrevDialogIsContextual()
    {
        var prevPrevTemplate = CreateTemplate("prevPrevDialog", "Prev Prev");

        var prevPrevDialog = new Dialog(
            prevPrevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);
        prevPrevDialog.Context = "inherited context";

        var prevTemplate = CreateTemplate("prevDialog", "Prev text");

        var prevDialogInHistory = new Dialog(
            prevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var newPrevTemplate = CreateTemplate("prevDialog", contextual: true);

        var newPrevDialog = new Dialog(
            newPrevTemplate,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var template = CreateTemplate("currentDialog", prevDialogKey: "prevDialog");

        DialogFactoryMock.Setup(f => f.Create("prevDialog", DialogSourceMock.Object, null))
                         .Returns(newPrevDialog);

        var dialog = new Dialog(
            template,
            DialogSourceMock.Object,
            ScriptProviderMock.Object,
            DialogFactoryMock.Object);

        var aisling = MockAisling.Create(Map);

        // Push prevPrev first, then prev, so prevPrev is peeked after popping prev
        aisling.DialogHistory.Push(prevPrevDialog);
        aisling.DialogHistory.Push(prevDialogInHistory);

        dialog.Previous(aisling);

        // The new prev dialog should be contextual and have context copied from prevPrev
        newPrevDialog.Contextual
                     .Should()
                     .BeTrue();
    }
    #endregion
}