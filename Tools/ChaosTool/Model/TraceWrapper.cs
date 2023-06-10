namespace ChaosTool.Model;

public sealed class TraceWrapper<T>
{
    public string Path { get; set; }
    public T Object { get; }

    public TraceWrapper(string path, T @object)
    {
        Path = path;
        Object = @object;
    }
}