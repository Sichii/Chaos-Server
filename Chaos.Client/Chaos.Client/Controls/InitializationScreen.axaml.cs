using Avalonia.Controls;

namespace Chaos.Client.Controls;

public class InitializationScreen : UserControl
{
    public InitializationScreen() { InitializeComponent(); }

    public void Write(string text) => InitializerTextBlock.Text = text;

    public void WriteLine(string text) => Write(text + Environment.NewLine);
}