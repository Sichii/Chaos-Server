using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChaosTool.Model;

public class BindableString : INotifyPropertyChanged
{
    private string _string = null!;

    public string String
    {
        get => _string;
        set => SetField(ref _string, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static implicit operator string(BindableString bindableString) => bindableString.String;
    public static implicit operator BindableString(string @string) => new() { String = @string };

    public BindableString() => String = string.Empty;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    /// <inheritdoc />
    public override string ToString() => String;
}