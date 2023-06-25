using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChaosTool.Definitions;

public static partial class Validators
{
    private static readonly Regex NotANumberPattern = NotANumberRegex();

    [GeneratedRegex("[^0-9]+", RegexOptions.Compiled)]
    private static partial Regex NotANumberRegex();

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