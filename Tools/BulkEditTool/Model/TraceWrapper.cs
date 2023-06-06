namespace BulkEditTool.Model;

public sealed class TraceWrapper<T>
{
    public string Path { get; set; }
    public T Obj { get; }

    public TraceWrapper(string path, T obj)
    {
        Path = path;
        Obj = obj;
    }
}