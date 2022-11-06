using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;

namespace Chaos.Utilities;

public class InputCollector
{
    private readonly List<InputCollectorBuilder.InputHandler> InputHandlers;
    private readonly List<InputCollectorBuilder.InputRequestBase> InputRequests;
    public virtual bool Completed { get; protected set; }
    protected bool Canceled { get; set; }
    protected bool PendingInput { get; private set; }
    protected int Stage { get; set; }

    public InputCollector(
        ICollection<InputCollectorBuilder.InputRequestBase> inputRequests,
        ICollection<InputCollectorBuilder.InputHandler> inputHandlers
    )
    {
        if (inputRequests.Count != inputHandlers.Count)
            throw new ArgumentException("Input requests and handlers must be the same length");

        InputRequests = inputRequests.ToList();
        InputHandlers = inputHandlers.ToList();
        Stage = 0;
    }

    public virtual void Collect(Aisling source, Dialog dialog, int? option = null)
    {
        if (Canceled)
            return;

        if (Stage >= InputRequests.Count)
        {
            Completed = true;

            return;
        }

        if (PendingInput && !Canceled)
        {
            PendingInput = false;
            var inputHandler = InputHandlers[Stage];

            if (!inputHandler(source, dialog, option))
            {
                Canceled = true;

                return;
            }

            Stage++;
        }

        if (Stage >= InputRequests.Count)
        {
            Completed = true;

            return;
        }

        InputRequests[Stage].Apply(dialog);
        dialog.Display(source);
        PendingInput = true;
    }
}

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

    public InputCollector Build() => new(InputRequests, InputHandlers);

    public InputRequestBase RequestOptionSelection(Lazy<string> dialogText, params Lazy<string>[] options) =>
        new OptionRequest(this, dialogText, options.ToList());

    public InputRequestBase RequestTextInput(Lazy<string> dialogText) => new TextEntryRequest(this, dialogText);

    public abstract class InputRequestBase
    {
        private readonly InputCollectorBuilder Builder;
        protected Lazy<string> DialogText { get; init; }
        protected MenuOrDialogType Type { get; init; }

        protected InputRequestBase(InputCollectorBuilder builder, MenuOrDialogType type, Lazy<string> dialogText)
        {
            Builder = builder;
            DialogText = dialogText;
            Type = type;

            builder.InputRequests.Add(this);
        }

        public abstract void Apply(Dialog dialog);

        public InputCollectorBuilder HandleInput(InputHandler handler)
        {
            Builder.InputHandlers.Add(handler);

            return Builder;
        }
    }

    public class OptionRequest : InputRequestBase
    {
        protected List<Lazy<string>> Options { get; init; }

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