
// This example shows how the async and await keywords work like dominoes when awaiting across multiple methods.
// The original caller (the top level code) calls the async method, which calls a subsequent async method, and so on...
// The actual long-running task (just a delay in this code) takes place in the FourthAsync method.
// Because Task.Delay is managed by the runtime in a seperate thread, the long-running task is being done outside of your code.
// Now, the awaits kick into action and pass execution back through from FouthAsync to ThirdAsync, SecondAsync, FirstAsync and then back to the original caller.
// The original caller is not awaiting, so can continue to execute synchronously. It will loop until FirstAsync completes.
// When the long-running task finishes, the runtime passes execution back to FourthAsync, which returns.
// This allows ThirdAsync to continue, return, and so on, until FirstAsync returns and completes.
// Now, execution passes back to the original caller to continue from where it was inturrupted.

Console.WriteLine("Calling FirstAsync from the original caller...");
Task<bool> StartDominoes = FirstAsync();

if (!StartDominoes.IsCompleted)
{
    Console.WriteLine("FirstAsync has not yet completed but has returned execution to the original caller. Original caller is continuing...");
}

// As we have seen before, a busy wait is blocking and uses CPU resources. This is just for demonstration purposes.
while (!StartDominoes.IsCompleted)
{
    Console.WriteLine("FirstAsync has not yet completed. Continuing in the original caller...");
    Thread.Sleep(500);
}

Console.WriteLine($"FirstAsync has now completed. It returned '{StartDominoes.Result}'. Continuing in the original caller...");
Console.WriteLine("Execution finished. The dominoes have fallen.");

public static partial class Program
{
    public static async Task<bool> FirstAsync()
    {
        Console.WriteLine("FirstAsync has been called. Calling SecondAsync...");
        Task<bool> SecondAsyncTask = SecondAsync();
        if (!SecondAsyncTask.IsCompleted)
        {
            Console.WriteLine("SecondAsync has not yet completed but has returned execution to FirstAsync. Awaiting the completion of SecondAsync...");
        }
        bool result = await SecondAsyncTask;
        Console.WriteLine("The FirstAsync method has completed.");
        return result;
    }
    private static async Task<bool> SecondAsync()
    {
        Console.WriteLine("SecondAsync has been called. Calling ThirdAsync...");
        Task<bool> ThirdAsyncTask = ThirdAsync();
        if (!ThirdAsyncTask.IsCompleted)
        {
            Console.WriteLine("ThirdAsync has not yet completed but has returned execution to SecondAsync. Awaiting the completion of ThirdAsync...");
        }
        bool result = await ThirdAsyncTask;
        Console.WriteLine("The SecondAsync method has completed.");
        return result;
    }
    private static async Task<bool> ThirdAsync()
    {
        Console.WriteLine("ThirdAsync has been called. Calling FourthAsync...");
        Task<bool> FourthAsyncTask = FourthAsync();
        if (!FourthAsyncTask.IsCompleted)
        {
            Console.WriteLine("FourthAsync has not yet completed but has returned execution to ThirdAsync. Awaiting the completion of FourthAsync...");
        }
        bool result = await FourthAsyncTask;
        Console.WriteLine("The ThirdAsync method has completed.");
        return result;
    }
    private static async Task<bool> FourthAsync()
    {
        Console.WriteLine("FourthAsync has been called. Performing the long running task...");
        await Task.Delay(5000);
        Console.WriteLine("The FourthAsync method has completed.");
        return true;
    }
}