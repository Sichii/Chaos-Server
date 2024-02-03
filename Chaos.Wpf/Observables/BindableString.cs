using Chaos.Wpf.Abstractions;

namespace Chaos.Wpf.Observables;

/// <summary>
///     A string replacement that supports 2-way binding within observable collections
/// </summary>
public sealed class BindableString : NotifyPropertyChangedBase
{
    private string _string = string.Empty;

    /// <summary>
    ///     The string value
    /// </summary>
    public string String
    {
        get => _string;
        set => SetField(ref _string, value);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BindableString" /> class
    /// </summary>
    public BindableString() => String = string.Empty;

    /// <summary>
    ///     Converts a <see cref="BindableString" /> to a <see cref="string" />
    /// </summary>
    public static implicit operator string(BindableString bindableString) => bindableString.String;

    /// <summary>
    ///     Converts a <see cref="string" /> to a <see cref="BindableString" />
    /// </summary>
    public static implicit operator BindableString(string @string)
        => new()
        {
            String = @string
        };

    /// <inheritdoc />
    public override string ToString() => String;
}