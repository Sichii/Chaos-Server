using Chaos.Schemas.Content;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class WorldMapNodeViewModel : SchemaViewModelBase<WorldMapNodeSchema>
{
    private ObservableLocation _destination = null!;
    private string _nodeKey = null!;
    private ObservablePoint _screenPosition = null!;
    private string _text = null!;

    public ObservableLocation Destination
    {
        get => _destination;
        set => SetField(ref _destination, value);
    }

    public string NodeKey
    {
        get => _nodeKey;
        set => SetField(ref _nodeKey, value);
    }

    public ObservablePoint ScreenPosition
    {
        get => _screenPosition;
        set => SetField(ref _screenPosition, value);
    }

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }
}