using Chaos.Schemas.Templates;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class SkillTemplateViewModel : PanelTemplateViewModelBase<SkillTemplateSchema>
{
    private bool _isAssail;
    private bool _levelsUp;
    private byte? _maxLevel;

    public bool IsAssail
    {
        get => _isAssail;
        set => SetField(ref _isAssail, value);
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

    public ObservableLearningRequirements LearningRequirements { get; } = new();

    public SkillTemplateViewModel() => LearningRequirements.PropertyChanged += (_, _) => OnPropertyChanged(nameof(LearningRequirements));
}