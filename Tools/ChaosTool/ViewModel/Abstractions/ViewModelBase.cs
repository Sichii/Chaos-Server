namespace ChaosTool.ViewModel.Abstractions;

public abstract class ViewModelBase : RevertibleChangeTrackingBase
{
    private string _originalPath = null!;
    private string _path = null!;

    public string OriginalPath
    {
        get => _originalPath;

        set
        {
            SetField(ref _originalPath, value);
            OnPropertyChanged(nameof(ViewModelIdentifier));
        }
    }

    public string Path
    {
        get => _path;
        set => SetField(ref _path, value);
    }

    public string ViewModelIdentifier => System.IO.Path.GetFileNameWithoutExtension(OriginalPath);
}