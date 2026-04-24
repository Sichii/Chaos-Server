#region
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockDialog
{
    /// <summary>
    /// Create a <see cref="Dialog" /> backed by a <see cref="DialogTemplate" /> with the given
    /// <paramref name="templateKey" />. The associated script provider is mocked and the dialog
    /// factory is a bare mock — override via <paramref name="setup" /> if needed.
    /// </summary>
    public static Dialog Create(string templateKey, Action<Dialog>? setup = null)
    {
        var sourceMock = new Mock<IDialogSourceEntity>();

        sourceMock.SetupGet(s => s.Name)
                  .Returns("TestSource");

        sourceMock.SetupGet(s => s.Id)
                  .Returns(1);

        var scriptProviderMock = new Mock<IScriptProvider>();
        var scriptMock = new Mock<IDialogScript>();

        scriptProviderMock.Setup(sp => sp.CreateScript<IDialogScript, Dialog>(It.IsAny<ICollection<string>>(), It.IsAny<Dialog>()))
                          .Returns(scriptMock.Object);

        var factoryMock = new Mock<IDialogFactory>();

        var template = new DialogTemplate
        {
            TemplateKey = templateKey,
            Text = "Hello",
            Type = ChaosDialogType.Normal,
            NextDialogKey = null,
            PrevDialogKey = null,
            Contextual = false,
            Options = [],
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            TextBoxLength = null,
            TextBoxPrompt = null,
            IllustrationIndex = 0
        };

        var dialog = new Dialog(
            template,
            sourceMock.Object,
            scriptProviderMock.Object,
            factoryMock.Object);

        setup?.Invoke(dialog);

        return dialog;
    }
}
