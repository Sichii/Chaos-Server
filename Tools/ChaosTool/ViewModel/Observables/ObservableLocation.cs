using System.Text.Json.Serialization;

namespace ChaosTool.ViewModel.Observables;

public class ObservableLocation : ObservablePoint
{
    private string _map = null!;

    [JsonRequired]
    public string Map
    {
        get => _map;
        set => SetField(ref _map, value);
    }
}