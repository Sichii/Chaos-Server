using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SeqConfigurator.Utility;

public sealed class AsyncComposer<T>
{
    private readonly TaskCompletionSource<T> Promise = new();
    private readonly ConcurrentQueue<Task> Tasks = new();

    public async void Compose(Func<T, Task> asyncBuilderFunc) => Tasks.Enqueue(asyncBuilderFunc(await Promise.Task));

    public void Compose(Action<T> builderAction)
    {
        Tasks.Enqueue(WrapAction(builderAction));

        return;

        async Task WrapAction(Action<T> innerAction)
        {
            var obj = await Promise.Task;
            innerAction(obj);
        }
    }

    public static AsyncComposer<T> Create(T obj)
    {
        var builder = new AsyncComposer<T>();
        builder.Promise.SetResult(obj);

        return builder;
    }

    public static AsyncComposer<T> Create(Task<T> objTask)
    {
        var builder = new AsyncComposer<T>();
        objTask.ContinueWith(async task => builder.Promise.SetResult(await task));

        return builder;
    }

    public TaskAwaiter<T> GetAwaiter()
        => WaitAsync()
            .GetAwaiter();

    public async Task<T> WaitAsync()
    {
        var obj = await Promise.Task;
        await Task.WhenAll(Tasks);

        return obj;
    }
}