using Chaos.DarkAges.Definitions;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Collections.ObjectModel;
using Chaos.Wpf.Observables;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class DialogTemplateViewModel : SchemaViewModelBase<DialogTemplateSchema>
{
    private bool _contextual;
    private string? _nextDialogKey;
    private string? _prevDialogKey;
    private string _templateKey = null!;
    private string _text = null!;
    private ushort? _textBoxLength;
    private string? _textBoxPrompt;
    private ChaosDialogType _type;

    public bool Contextual
    {
        get => _contextual;
        set => SetField(ref _contextual, value);
    }

    public string? NextDialogKey
    {
        get => _nextDialogKey;
        set => SetField(ref _nextDialogKey, value);
    }

    public string? PrevDialogKey
    {
        get => _prevDialogKey;
        set => SetField(ref _prevDialogKey, value);
    }

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public ushort? TextBoxLength
    {
        get => _textBoxLength;
        set => SetField(ref _textBoxLength, value);
    }

    public string? TextBoxPrompt
    {
        get => _textBoxPrompt;
        set => SetField(ref _textBoxPrompt, value);
    }

    public ChaosDialogType Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }

    public ObservingCollection<ObservableDialogOption> Options { get; } = [];

    public ObservingCollection<BindableString> ScriptKeys { get; } = [];

    public DialogTemplateViewModel()
    {
        Options.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Options));
        ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
    }
}