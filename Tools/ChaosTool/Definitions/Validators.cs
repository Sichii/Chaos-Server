#region
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
#endregion

namespace ChaosTool.Definitions;

// ReSharper disable once PartialTypeWithSinglePart
public static partial class Validators
{
    [GeneratedRegex("[^0-9]+", RegexOptions.Compiled)]
    private static partial Regex NotANumberPattern { get; }

    public static void NumberValidationTextBox(object sender, TextCompositionEventArgs e) => e.Handled = NotANumberPattern.IsMatch(e.Text);

    public static void TemplateKeyMatchesFileName(TextBox templateKeyTextBox, TextBox fileNameTextBox)
    {
        if (!templateKeyTextBox.IsEnabled)
            return;

        var templateKey = templateKeyTextBox.Text;
        var extension = Path.GetExtension(fileNameTextBox.Text);
        var currentFileName = Path.GetFileNameWithoutExtension(fileNameTextBox.Text);

        var currentDir = fileNameTextBox.Text[..(fileNameTextBox.Text.Length - currentFileName.Length - extension.Length)];

        fileNameTextBox.Text = currentDir + templateKey + extension;

        if (string.IsNullOrEmpty(extension))
            fileNameTextBox.Text += ".json";
    }
}