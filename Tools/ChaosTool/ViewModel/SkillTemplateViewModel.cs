using Chaos.Schemas.Templates;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class SkillTemplateViewModel : PanelTemplateViewModelBase<SkillTemplateSchema>
{
    private bool _isAssail;

    public bool IsAssail
    {
        get => _isAssail;
        set => SetField(ref _isAssail, value);
    }

    public ObservableLearningRequirements LearningRequirements { get; } = new();

    public SkillTemplateViewModel() => LearningRequirements.PropertyChanged += (_, _) => OnPropertyChanged(nameof(LearningRequirements));
}