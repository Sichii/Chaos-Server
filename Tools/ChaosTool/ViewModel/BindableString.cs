using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel;

public sealed class BindableString : NotifyPropertyChangedBase
{
    private string _string = string.Empty;

    public string String
    {
        get => _string;
        set => SetField(ref _string, value);
    }

    public BindableString() => String = string.Empty;

    public static implicit operator string(BindableString bindableString) => bindableString.String;

    public static implicit operator BindableString(string @string)
        => new()
        {
            String = @string
        };

    /// <inheritdoc />
    public override string ToString() => String;
}