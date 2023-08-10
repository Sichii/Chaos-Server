using System.Collections.Concurrent;

namespace SeqConfigurator.Utility;

public sealed class AsyncFluentComposer<T>
{
    private readonly TaskCompletionSource<T> ObjectPromise = new();
    private readonly ConcurrentQueue<Task> Tasks = new();

    public async Task<T> BuildAsync()
    {
        var obj = await ObjectPromise.Task;
        await Task.WhenAll(Tasks);

        return obj;
    }

    public async void Compose(Func<T, Task> asyncBuilderFunc) => Tasks.Enqueue(asyncBuilderFunc(await ObjectPromise.Task));

    public void Compose(Action<T> builderAction)
    {
        Tasks.Enqueue(WrapAction(builderAction));

        return;

        async Task WrapAction(Action<T> innerAction)
        {
            var obj = await ObjectPromise.Task;
            innerAction(obj);
        }
    }

    public static AsyncFluentComposer<T> Create(T obj)
    {
        var builder = new AsyncFluentComposer<T>();
        builder.ObjectPromise.SetResult(obj);

        return builder;
    }

    public static AsyncFluentComposer<T> Create(Task<T> objTask)
    {
        var builder = new AsyncFluentComposer<T>();
        objTask.ContinueWith(async task => builder.ObjectPromise.SetResult(await task));

        return builder;
    }
}