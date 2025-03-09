using System;
using System.Threading;

namespace ThreadUnsafe
{
    class ThreadRunner
    {
        private readonly int numberToGenerate = 100;
        private int index = 0;
        private int[] orderedNumbers;
        private Random rng = new Random();
        public void Run()
        {
            orderedNumbers = new int[numberToGenerate];
            Thread[] threads = new Thread[numberToGenerate];

            for (int i = 0; i < numberToGenerate; i++)
            {
                // Create a thread to add a value to the array
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(AddValue);
                Thread thread = new Thread(threadStart);
                // Add the thread to the threads array so we can run them all
                threads[i] = thread;
            }

            for (int i = 0; i < numberToGenerate; i++)
            {
                threads[i].Start(i + 1);
            }

            // Wait for all threads to finish
            // This is a busy wait, which is not ideal as it uses cpu time and blocks the thread, but it works for this example and is simple to understand
            // It is better to use callbacks to know when threads are finished, or use sync techniques like WaitHandles
            bool allThreadsFinished = false;
            while (!allThreadsFinished)
            {
                allThreadsFinished = true;
                foreach (Thread t in threads)
                {
                    if (t.IsAlive)
                    {
                        allThreadsFinished = false;
                        break;
                    }
                }
            }

            foreach (int i in orderedNumbers)
            {
                Console.WriteLine(i);
            }
        }

        private void AddValue(object value)
        {
            orderedNumbers[index] = (int)value;
            // Simulate some work of unknown length
            Thread.Sleep(rng.Next(6));
            index++;
        }
    }
}
