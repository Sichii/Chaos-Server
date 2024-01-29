using Chaos.Site.Definitions;

namespace Chaos.Site.Models;

public class SortModel
{
    public string ColId { get; set; } = null!;
    public SortType Sort { get; set; }
}