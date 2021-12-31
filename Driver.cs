using System.Diagnostics;
using System.Numerics;

public class Driver
{
    private static int numTask = 10;
    private static TimeSpan totalRuntimeLength = TimeSpan.FromMinutes(0.5);

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
}
