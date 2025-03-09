
// In this example we are going to create 100 threads and 100 Task.Run threads from the threadpool.
// The idea is to show how much more efficient using the thread pool is - especially for short-running tasks.

// We'll be using the stopwatch to identify how long each background task took, and averaging that out over 100 runs.

// There are some possible gotcha's here. The Task.Run().ContinueWith actually runs the ContinueWith code on another thread from the pool - so this code may cause thread race condition bugs.
// The same is true for the Thread.Start() code as the callback is managed using a multicast delegate. These callbacks all run on the created thread.
// That is why we are using the lock. There are other ways to manage this e.g., using WaitAll and looping through all of the finished Tasks on the foreground thread (for example).


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


double taskSum = 0, threadSum = 0;
int numToAverage = 100, numTasksFinished = 0, numThreadsFinished = 0;
object lockObject = new object();

// Make the tasks
Console.WriteLine($"Making {numToAverage} Tasks using Task.Run()...");
for (int i = 0; i < numToAverage; i++)
{
    ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadFunction);
    Stopwatch sw = new Stopwatch();
    sw.Start();
    _ = Task.Run(() => ts.Invoke(sw))
        .ContinueWith(result =>
        {
            // Task.Run Callback
            taskSum += sw.Elapsed.Ticks;
            lock (lockObject)
            {
                numTasksFinished++;
            }
        });
}

// Make the threads
Console.WriteLine($"Making {numToAverage} Threads using Thread.Start()...");
for (int i = 0; i < numToAverage; i++)
{
    ParameterizedThreadStart ts = new ParameterizedThreadStart(ThreadFunction);
    Stopwatch sw = new Stopwatch();
    ts += (param) =>
    {
        // Thread Callback
        Stopwatch sw = (Stopwatch)param;
        threadSum += sw.Elapsed.Ticks;
        lock (lockObject)
        {
            numThreadsFinished++;
        }

    };
    Thread thread = new Thread(ts);
    sw.Start();
    thread.Start(sw);
}

// Wait until all of the tasks and threads have finished
// This is quite inefficient as it is difficult to assess the completion of a thread
// If we were only using tasks we could Task.WaitAll - which would be much more efficient.
SpinWait.SpinUntil(() => numTasksFinished + numThreadsFinished == numToAverage * 2);

Console.WriteLine($"All {numToAverage} Threads and {numToAverage} Tasks have finished.");

Console.WriteLine("Average Task.Run time in milliseconds: {0}", (taskSum / numToAverage)/10000);
Console.WriteLine("Average Thread time in milliseconds: {0}", (threadSum / numToAverage)/10000);

// This is the function our Tasks/Threads call.
// It just stops the stopwatch immediately.
static void ThreadFunction(object param)
{
    Stopwatch sw = (Stopwatch)param;
    // Stop the stopwatch once the thread is running
    sw.Stop();
}