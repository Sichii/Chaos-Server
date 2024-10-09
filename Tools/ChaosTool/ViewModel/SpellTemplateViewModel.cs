using Chaos.DarkAges.Definitions;
using Chaos.Schemas.Templates;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class SpellTemplateViewModel : PanelTemplateViewModelBase<SpellTemplateSchema>
{
    private byte _castLines;
    private bool _levelsUp;
    private byte? _maxLevel;
    private string? _prompt;
    private SpellType _spellType;

    public byte CastLines
    {
        get => _castLines;
        set => SetField(ref _castLines, value);
    }

    public bool LevelsUp
    {
        get => _levelsUp;
        set => SetField(ref _levelsUp, value);
    }

    public byte? MaxLevel
    {
        get => _maxLevel;
        set => SetField(ref _maxLevel, value);
    }

    public string? Prompt
    {
        get => _prompt;
        set => SetField(ref _prompt, value);
    }

    public SpellType SpellType
    {
        get => _spellType;
        set => SetField(ref _spellType, value);
    }

    public ObservableLearningRequirements LearningRequirements { get; } = new();

    public SpellTemplateViewModel() => LearningRequirements.PropertyChanged += (_, _) => OnPropertyChanged(nameof(LearningRequirements));
}