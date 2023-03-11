namespace BulkEditTool.Model;

public sealed class TraceWrapper<T>
{
    public T Obj { get; }
    public string Path { get; }

    public TraceWrapper(string path, T obj)
    {
        Path = path;
        Obj = obj;
    }
}