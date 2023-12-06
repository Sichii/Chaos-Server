namespace ChaosTool.Model;

public sealed class TraceWrapper<T>(string path, T @object)
{
    public string Path { get; set; } = path;
    public T Object { get; } = @object;
}