using System.Collections.Concurrent;
using System.Diagnostics;

public class Driver
{
    private static TimeSpan totalRuntimeLength = TimeSpan.FromMinutes(1);
    private const int SEED = 0;

    public void RunSerial() 
    {
        Random rand = new Random(SEED);
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
        
        Console.WriteLine($"Serial - Tasks completed: {taskCounter}");
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
                Random rand = new Random(SEED);

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
        
        Console.WriteLine($"Concurrent - Tasks completed: {taskCounter}");
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
                Random rand = new Random(SEED);
                int workBufferSize = 5;

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
        
        Console.WriteLine($"Concurrent with buffer - Tasks completed: {taskCounter}");
    }
}
