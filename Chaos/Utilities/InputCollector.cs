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