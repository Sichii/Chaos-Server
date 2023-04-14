using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Chaos.Extensions.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPad.Editor;
using RoslynPad.Roslyn;

namespace BulkEditTool;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
// ReSharper disable once ClassCanBeSealed.Global
public partial class MainWindow : Window
{
    private readonly ObservableCollection<DocumentViewModel> DocViewModel;
    private readonly RoslynHost RoslynHost;
    private RoslynCodeEditor? Editor;
    public string ScriptOutput => DocViewModel.FirstOrDefault()?.Result ?? string.Empty;

    public MainWindow()
    {
        InitializeComponent();

        DocViewModel = new ObservableCollection<DocumentViewModel>();
        Items.ItemsSource = DocViewModel;

        var currentAssembly = Assembly.GetExecutingAssembly();

        var currentAssemblyStack = currentAssembly.GetReferencedAssemblies()
                                                  .Select(Assembly.Load)
                                                  .Where(a => !a.IsDynamic)
                                                  .Prepend(currentAssembly)
                                                  .ToList();

        var namespaces = currentAssemblyStack.Where(asm => asm.FullName!.ContainsI("Chaos."))
                                             .SelectMany(
                                                 asm =>
                                                 {
                                                     try
                                                     {
                                                         return asm.GetTypes();
                                                     } catch
                                                     {
                                                         return Enumerable.Empty<Type>();
                                                     }
                                                 })
                                             .Select(t => t.Namespace)
                                             .Where(nsp => nsp?.StartsWithI("Chaos.") ?? false)
                                             .Distinct()
                                             .ToList();

        namespaces.Add("BulkEditTool");

        RoslynHost = new CustomRoslynHost(
            new[]
            {
                Assembly.Load("RoslynPad.Roslyn.Windows"),
                Assembly.Load("RoslynPad.Editor.Windows"),
                Assembly.Load("BulkEditTool")
                //Assembly.Load("Chaos")
            },
            RoslynHostReferences.NamespaceDefault.With(
                assemblyReferences: new[]
                {
                    typeof(object).Assembly,
                    typeof(Regex).Assembly,
                    typeof(Enumerable).Assembly,
                    Assembly.GetExecutingAssembly()
                }.Concat(currentAssemblyStack),
                imports: namespaces!));

        AddNewDocument();
    }

    private void AddNewDocument(DocumentViewModel? previous = null) => DocViewModel.Add(new DocumentViewModel(RoslynHost, previous));

    private async void OnItemLoaded(object sender, EventArgs e)
    {
        await JsonContext.LoadAsync();

        Editor = (RoslynCodeEditor)sender;

        Editor.TextArea.SelectionCornerRadius = 0;
        Editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Colors.Black), 0);

        Editor.TextArea.SelectionBrush = new SolidColorBrush(
            Color.FromArgb(
                100,
                51,
                153,
                255));

        Editor.TextArea.SelectionForeground = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        Editor.TextArea.Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220));

        Editor.SearchReplacePanel.MarkerBrush = new SolidColorBrush(Color.FromRgb(119, 56, 0));

        Editor.RefreshHighlighting();

        Editor.Loaded -= OnItemLoaded;
        Editor.Focus();

        var viewModel = (DocumentViewModel)Editor.DataContext;
        var workingDirectory = Directory.GetCurrentDirectory();

        var previous = viewModel.LastGoodPrevious;

        if (previous != null)
            Editor.CreatingDocument += (_, args) =>
            {
                args.DocumentId = RoslynHost.AddRelatedDocument(
                    previous.Id,
                    new DocumentCreationArgs(
                        args.TextContainer,
                        workingDirectory,
                        SourceCodeKind.Script,
                        args.ProcessDiagnostics,
                        args.TextContainer.UpdateText));
            };

        var documentId = await Editor.InitializeAsync(
                                         RoslynHost,
                                         new DarkModeColors(),
                                         workingDirectory,
                                         """
// Enter your code here
// You can import namspaces via using statements
// JsonContext is available as a global variable for accessing json data
// You can return a value to see it in the output
// Once you make changes to objects, make sure to save with "await JsonContext.SaveChangesAsync();"
""",
                                         SourceCodeKind.Script)
                                     .ConfigureAwait(true);

        viewModel.Initialize(documentId);
    }

    private async void OnRunClicked(object sender, RoutedEventArgs e)
    {
        if (Editor == null)
            return;

        var viewModel = (DocumentViewModel)Editor.DataContext;

        viewModel.Text = Editor.Text;

        await viewModel.TrySubmitAsync().ConfigureAwait(true);
        Output.Text = viewModel.Result;
    }

    // TODO: workaround for GetSolutionAnalyzerReferences bug (should be added once per Solution)
    private sealed class CustomRoslynHost : RoslynHost
    {
        private bool _addedAnalyzers;

        public CustomRoslynHost(
            IEnumerable<Assembly>? additionalAssemblies = null,
            RoslynHostReferences? references = null,
            ImmutableArray<string>? disabledDiagnostics = null
        )
            : base(additionalAssemblies, references, disabledDiagnostics) { }

        /// <inheritdoc />
        protected override ParseOptions CreateDefaultParseOptions() =>
            new CSharpParseOptions(
                LanguageVersion.Preview,
                DocumentationMode.Parse,
                SourceCodeKind.Script,
                new[] { "TRACE", "DEBUG" });

        protected override IEnumerable<AnalyzerReference> GetSolutionAnalyzerReferences()
        {
            if (!_addedAnalyzers)
            {
                _addedAnalyzers = true;

                return base.GetSolutionAnalyzerReferences();
            }

            return Enumerable.Empty<AnalyzerReference>();
        }
    }
}