using Chaos.Schemas.Content;
using Chaos.Wpf.Collections.ObjectModel;
using Chaos.Wpf.Observables;
using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel;

public sealed class WorldMapViewModel : SchemaViewModelBase<WorldMapSchema>
{
    private byte _fieldIndex;
    private string _worldMapKey = null!;

    public byte FieldIndex
    {
        get => _fieldIndex;
        set => SetField(ref _fieldIndex, value);
    }

    public string WorldMapKey
    {
        get => _worldMapKey;
        set => SetField(ref _worldMapKey, value);
    }

    public ObservingCollection<BindableString> NodeKeys { get; } = [];

    public WorldMapViewModel() => NodeKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(NodeKeys));
}