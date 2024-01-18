namespace Chaos.Site.Models;

public class GetRowsParams
{
    public dynamic? Context { get; set; }

    public int EndRow { get; set; }

    public Dictionary<string, FilterModel>? FilterModel { get; set; }
    public SortModel[]? SortModel { get; set; }
    public int StartRow { get; set; }
}