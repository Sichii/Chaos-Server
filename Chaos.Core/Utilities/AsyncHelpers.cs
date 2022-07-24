// ReSharper disable AsyncVoidLambda
namespace Chaos.Core.Utilities;

public static class AsyncHelpers
{
    public static void RunSync(Func<Task> task)
    {
        var oldContext = SynchronizationContext.Current;
        var sync = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(sync);

        sync.Post(
            async _ =>
            {
                try
                {
                    await task();
                } catch (Exception e)
                {
                    sync.InnerException = e;

                    throw;
                } finally
                {
                    sync.EndMessageLoop();
                }
            },
            null);

        sync.BeginMessageLoop();

        SynchronizationContext.SetSynchronizationContext(oldContext);
    }

    /// <summary>
    /// Execute's an async Task
    /// method which has a T return type synchronously
    /// </summary>
    /// <typeparam name="T">Return Type</typeparam>
    /// <param name="task">Task
    ///     method to execute
    /// </param>
    /// <returns></returns>
    public static T? RunSync<T>(Func<Task<T>> task)
    {
        var oldContext = SynchronizationContext.Current;
        var synch = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(synch);
        var ret = default(T);

        synch.Post(
            async _ =>
            {
                try
                {
                    ret = await task();
                } catch (Exception e)
                {
                    synch.InnerException = e;

                    throw;
                } finally
                {
                    synch.EndMessageLoop();
                }
            },
            null);

        synch.BeginMessageLoop();
        SynchronizationContext.SetSynchronizationContext(oldContext);

        return ret;
    }

    private class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private bool Done;
        public Exception? InnerException { get; set; }
        private readonly AutoResetEvent WorkItemsWaiting = new(false);
        private readonly Queue<Tuple<SendOrPostCallback, object>?> Items = new();

        public override void Send(SendOrPostCallback d, object? state) => throw new NotSupportedException("We cannot send to our same thread");

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (Items)
                Items.Enqueue(Tuple.Create(d, state)!);

            WorkItemsWaiting.Set();
        }

        public void EndMessageLoop() => Post(_ => Done = true, null);

        public void BeginMessageLoop()
        {
            while (!Done)
            {
                Tuple<SendOrPostCallback, object>? task = null;

                lock (Items)
                    if (Items.Count > 0)
                        task = Items.Dequeue();

                if (task != null)
                {
                    task.Item1(task.Item2);

                    if (InnerException != null) // the method threw an exeption
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                } else
                    WorkItemsWaiting.WaitOne();
            }
        }

        public override SynchronizationContext CreateCopy() => this;
    }
}