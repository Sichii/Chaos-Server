using Chaos.Collections.Common;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Collections.ObjectModel;
using Chaos.Wpf.Observables;
using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel;

public sealed class ReactorTileTemplateViewModel : SchemaViewModelBase<ReactorTileTemplateSchema>
{
    private bool _shouldBlockPathfinding;
    private string _templateKey = string.Empty;

    public IDictionary<string, DynamicVars> ScriptVars { get; set; }
        = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    public bool ShouldBlockPathfinding
    {
        get => _shouldBlockPathfinding;
        set => SetField(ref _shouldBlockPathfinding, value);
    }

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }

    public ObservingCollection<BindableString> ScriptKeys { get; } = [];

    public ReactorTileTemplateViewModel() => ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
}