using ChaosTool.ViewModel.Abstractions;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace ChaosTool.ViewModel.Observables;

public class ObservableStats : NotifyPropertyChangedBase
{
    private int _con;
    private int _dex;
    private int _int;
    private int _str;
    private int _wis;

    public int Con
    {
        get => _con;
        set => SetField(ref _con, value);
    }

    public int Dex
    {
        get => _dex;
        set => SetField(ref _dex, value);
    }

    public int Int
    {
        get => _int;
        set => SetField(ref _int, value);
    }

    public int Str
    {
        get => _str;
        set => SetField(ref _str, value);
    }

    public int Wis
    {
        get => _wis;
        set => SetField(ref _wis, value);
    }

    public bool Equals(ObservableStats? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (_con == other._con) && (_dex == other._dex) && (_int == other._int) && (_str == other._str) && (_wis == other._wis);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((ObservableStats)obj);
    }
}