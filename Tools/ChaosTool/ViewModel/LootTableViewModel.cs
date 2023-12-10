using Chaos.Common.Definitions;
using Chaos.Schemas.Content;
using ChaosTool.Utility;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class LootTableViewModel : SchemaViewModelBase<LootTableSchema>
{
    private string _key = string.Empty;
    private LootTableMode _mode;

    public string Key
    {
        get => _key;
        set => SetField(ref _key, value);
    }

    public LootTableMode Mode
    {
        get => _mode;
        set => SetField(ref _mode, value);
    }

    public ObservingCollection<ObservableLootDrop> LootDrops { get; } = [];

    public LootTableViewModel() => LootDrops.CollectionChanged += (_, _) => OnPropertyChanged(nameof(LootDrops));
}