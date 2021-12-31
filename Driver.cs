using System.Collections.Concurrent;
using System.Diagnostics;

public class Driver
{
    private static TimeSpan totalRuntimeLength = TimeSpan.FromMinutes(1);

    public void RunSerial() 
    {
        Random rand = new Random();
        uint taskCounter = 0;

        Stopwatch watch = new Stopwatch();
        watch.Start();

        while (watch.Elapsed < totalRuntimeLength)
        {
            // Simulate doing some task
            TimeSpan taskTimeLength = 
                TimeSpan.FromMilliseconds(rand.Next(100, 1000));
            Thread.Sleep(taskTimeLength);
            ++taskCounter;
        }

        watch.Stop();
        
        Console.WriteLine($"Tasks completed: {taskCounter}");
    }

    public void RunConcurrentBag()
    {
        ulong taskCounter = 0;
        List<Task> tasks = new List<Task>();
        ConcurrentBag<Action> cb = new ConcurrentBag<Action>();

        Stopwatch watch = new Stopwatch();
        watch.Start();

        for (int i = 0; i < Environment.ProcessorCount; ++i) {
            tasks.Add(Task.Run(() =>
            {
                Random rand = new Random();

                while (watch.Elapsed < totalRuntimeLength) {
                    cb.Add(() => 
                    {
                        // Simulate doing some task
                        TimeSpan taskTimeLength = 
                            TimeSpan.FromMilliseconds(rand.Next(100, 1000));
                        Thread.Sleep(taskTimeLength);
                        Interlocked.Increment(ref taskCounter);
                    });

                    Action work;
                    while (cb.TryTake(out work!)) {
                        work();
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray<Task>());

        watch.Stop();
        
        Console.WriteLine($"Tasks completed: {taskCounter}");
    }

    public void RunConcurrentBagWithWorkBuffer()
    {
        ulong taskCounter = 0;
        List<Task> tasks = new List<Task>();
        ConcurrentBag<Action> cb = new ConcurrentBag<Action>();

        Stopwatch watch = new Stopwatch();
        watch.Start();

        for (int i = 0; i < Environment.ProcessorCount; ++i) {
            tasks.Add(Task.Run(() =>
            {
                Random rand = new Random();
                int workBufferSize = 10;

                while (watch.Elapsed < totalRuntimeLength) {
                    for (int i = 0; i < workBufferSize; ++i)
                    {
                        cb.Add(() => 
                        {
                            // Simulate doing some task
                            TimeSpan taskTimeLength = 
                                TimeSpan.FromMilliseconds(rand.Next(100, 1000));
                            Thread.Sleep(taskTimeLength);
                            Interlocked.Increment(ref taskCounter);
                        });
                    }

                    Action work;
                    while (cb.TryTake(out work!)) {
                        work();
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray<Task>());

        watch.Stop();
        
        Console.WriteLine($"Tasks completed: {taskCounter}");
    }
}
