using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChaosTool.ViewModel.Abstractions;

public abstract class RevertibleChangeTrackingBase : NotifyPropertyChangedBase, IRevertibleChangeTracking
{
    private bool _isChanged;
    private bool _isDeleted;
    private bool _isInserted;

    /// <inheritdoc />
    public bool IsChanged
    {
        get => _isChanged;
        set => SetChanged(value);
    }

    public bool IsDeleted
    {
        get => _isDeleted;
        set => SetField(ref _isDeleted, value);
    }

    public bool IsInserted
    {
        get => _isInserted;
        set => SetField(ref _isInserted, value);
    }

    /// <inheritdoc />
    public abstract void AcceptChanges();

    /// <inheritdoc />
    public abstract void RejectChanges();

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        SetChanged(true);
    }

    private void SetChanged(bool changed)
    {
        if (_isChanged == changed)
            return;

        _isChanged = changed;
        OnPropertyChanged(nameof(IsChanged));
    }

    protected override bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (field is not IEnumerable && EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }
}