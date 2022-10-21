using Chaos.Objects.Dialog;

namespace Chaos.Data;

public class DialogContext
{
    public required Dialog Dialog { get; set; }
    public required object SourceEntity { get; set; }
}