using Chaos.Schemas.Templates;
using Chaos.Wpf.Collections.ObjectModel;
using Chaos.Wpf.Observables;
using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel;

public sealed class MapTemplateViewModel : SchemaViewModelBase<MapTemplateSchema>
{
    private byte _height;
    private string _templateKey = string.Empty;
    private byte _width;

    public byte Height
    {
        get => _height;
        set => SetField(ref _height, value);
    }

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }

    public byte Width
    {
        get => _width;
        set => SetField(ref _width, value);
    }

    public ObservingCollection<BindableString> ScriptKeys { get; } = [];

    public MapTemplateViewModel() => ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
}