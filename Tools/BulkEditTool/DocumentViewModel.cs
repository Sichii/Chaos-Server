using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using RoslynPad.Roslyn;

namespace BulkEditTool;

internal class DocumentViewModel : INotifyPropertyChanged
{
    private readonly RoslynHost _host;
    private DocumentId? _id;
    private bool _isReadOnly;
    private string? _result;

    public bool HasError { get; private set; }

    public DocumentId Id
    {
        get => _id ?? throw new InvalidOperationException("Document not initialized");
        private set => _id = value;
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        private set => SetProperty(ref _isReadOnly, value);
    }

    public string? Result
    {
        get => _result;
        private set => SetProperty(ref _result, value);
    }

    public Script<object>? Script { get; private set; }

    public string? Text { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public DocumentViewModel? LastGoodPrevious
    {
        get
        {
            var previous = Previous;

            while (previous is { HasError: true })
                previous = previous.Previous;

            return previous;
        }
    }

    public DocumentViewModel? Previous { get; }

    private static MethodInfo HasSubmissionResult { get; } =
        typeof(Compilation).GetMethod(nameof(HasSubmissionResult), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingMemberException(nameof(HasSubmissionResult));

    private static PrintOptions PrintOptions { get; } =
        new()
            { MemberDisplayFormat = MemberDisplayFormat.SeparateLines };

    public DocumentViewModel(RoslynHost host, DocumentViewModel? previous)
    {
        _host = host;
        Previous = previous;
    }

    private async Task ExecuteAsync(bool hasResult)
    {
        var script = Script;

        if (script == null)
            return;

        try
        {
            var result = await script.RunAsync().ConfigureAwait(true);

            if (result.Exception != null)
            {
                HasError = true;
                Result = FormatException(result.Exception);
            } else
                Result = hasResult ? FormatObject(result.ReturnValue) : null;
        } catch (Exception ex)
        {
            HasError = true;
            Result = FormatException(ex);
        }
    }

    private static string FormatException(Exception ex) => CSharpObjectFormatter.Instance.FormatException(ex);

    private static string FormatObject(object o) => CSharpObjectFormatter.Instance.FormatObject(o, PrintOptions);

    internal void Initialize(DocumentId id) => Id = id;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        return false;
    }

    public async Task<bool> TrySubmitAsync()
    {
        Result = null;

        Script = LastGoodPrevious?.Script?.ContinueWith(Text)
                 ?? CSharpScript.Create(
                     Text,
                     ScriptOptions.Default
                                  .WithReferences(_host.DefaultReferences)
                                  .WithImports(_host.DefaultImports));

        var compilation = Script.GetCompilation();
        var hasResult = HasSubmissionResult.Invoke(compilation, null) as bool? == true;
        var diagnostics = Script.Compile();

        if (diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error))
        {
            Result = string.Join(Environment.NewLine, diagnostics.Select(FormatObject));

            return false;
        }

        IsReadOnly = true;

        await ExecuteAsync(hasResult).ConfigureAwait(true);

        return true;
    }
}