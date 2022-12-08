using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;

namespace Chaos.Utilities;

public class InputCollectorBuilder
{
    public delegate bool InputHandler(Aisling source, Dialog dialog, int? option = null);

    private readonly List<InputHandler> InputHandlers;
    private readonly List<InputRequestBase> InputRequests;

    public InputCollectorBuilder()
    {
        InputRequests = new List<InputRequestBase>();
        InputHandlers = new List<InputHandler>();
    }

    public virtual InputCollector Build() => new(InputRequests, InputHandlers);

    public virtual InputRequestBase RequestOptionSelection(Lazy<string> dialogText, params Lazy<string>[] options) =>
        new OptionRequest(this, dialogText, options.ToList());

    public virtual InputRequestBase RequestTextInput(Lazy<string> dialogText) => new TextEntryRequest(this, dialogText);

    public abstract class InputRequestBase
    {
        private readonly InputCollectorBuilder Builder;
        protected Lazy<string> DialogText { get; }
        protected MenuOrDialogType Type { get; }

        protected InputRequestBase(InputCollectorBuilder builder, MenuOrDialogType type, Lazy<string> dialogText)
        {
            Builder = builder;
            DialogText = dialogText;
            Type = type;

            builder.InputRequests.Add(this);
        }

        public abstract void Apply(Dialog dialog);

        public virtual InputCollectorBuilder HandleInput(InputHandler handler)
        {
            Builder.InputHandlers.Add(handler);

            return Builder;
        }
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public class OptionRequest : InputRequestBase
    {
        protected List<Lazy<string>> Options { get; }

        public OptionRequest(InputCollectorBuilder builder, Lazy<string> dialogText, List<Lazy<string>> options)
            : base(builder, MenuOrDialogType.Menu, dialogText) =>
            Options = options;

        /// <inheritdoc />
        public override void Apply(Dialog dialog)
        {
            var newType = MenuOrDialogType.Menu;

            if (dialog.MenuArgs.Any())
                newType = MenuOrDialogType.MenuWithArgs;

            dialog.Type = newType;
            dialog.Text = DialogText.Value;
            dialog.PrevDialogKey = dialog.Template.TemplateKey;
            dialog.NextDialogKey = null;

            dialog.Options = Options.Select(
                                        o => new DialogOption
                                        {
                                            DialogKey = dialog.Template.TemplateKey,
                                            OptionText = o.Value
                                        })
                                    .ToList();
        }
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public class TextEntryRequest : InputRequestBase
    {
        public TextEntryRequest(InputCollectorBuilder builder, Lazy<string> dialogText)
            : base(builder, MenuOrDialogType.MenuTextEntry, dialogText) { }

        /// <inheritdoc />
        public override void Apply(Dialog dialog)
        {
            var newType = MenuOrDialogType.MenuTextEntry;

            if (dialog.MenuArgs.Any())
                newType = MenuOrDialogType.MenuTextEntryWithArgs;

            dialog.Type = newType;
            dialog.Text = DialogText.Value;
            dialog.PrevDialogKey = dialog.Template.TemplateKey;
            dialog.NextDialogKey = null;
            dialog.Options = new List<DialogOption>();
        }
    }
}