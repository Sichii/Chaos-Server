using Chaos.Geometry.Abstractions;
using Chaos.Wpf.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public class ObservablePoint : NotifyPropertyChangedBase, IPoint
{
    private int _x;
    private int _y;

    /// <inheritdoc />
    public int X
    {
        get => _x;
        set => SetField(ref _x, value);
    }

    /// <inheritdoc />
    public int Y
    {
        get => _y;
        set => SetField(ref _y, value);
    }
}