
// This example shows a solution for splitting the problem of calculating all of the possible square roots (up to numberToCalculate) into smaller tasks
// Doing so will allow us to use multiple concurrent threads.

// The way this is done is:
//Task 0 : starts at 0, has a step of 10 and stops at numberToCalculate
//Task 1 : starts at 1, has a step of 10 and stops at numberToCalculate
//Task 2 : starts at 2, has a step of 10 and stops at numberToCalculate
//Etc…

//CalculateSqrt for each task then calls the SqrtByAlgorithm method in each task, using step to identify how far to ‘jump’ to the next value to calculate:
//Task 0 : starts at 0, then continues with: 10, 20, 30, 40, 50, etc.and stops at numberToCalculate
//Task 1 : starts at 1, then continues with: 11, 21, 31, 41, 51, etc.and stops at numberToCalculate
//Task 2 : starts at 2, then continues with: 12, 22, 32, 42, 52, etc.and stops at numberToCalculate
//Etc…

// We can easily update the numberToCalculate and calculate more values, or update step and break down the search space more.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


SquareRoots = new Dictionary<int, double>();
Console.WriteLine("Please wait while square roots are calculated...");
int numberToCalculate = 100000000;

for (int i = 0; i < numberToCalculate; i++)
{
    SquareRoots.Add(i, 0);
}

int step = 10;
Task[] tasks = new Task[step];
for (int i = 0; i < step; i++)
{
    // This has the potential to be a 'gotcha'
    // As Task.Run requires a thread to be allocated from the pool, etc. i may be updated by the loop by the time the invoke is called
    // Remove taskStartValue and pass i to CalculateSqrt, then look at the generated SquareRoots in the Dictionary to see what I mean...
    int taskStartValue = i;
    tasks[i] = Task.Run(() => CalculateSqrt(taskStartValue, step, numberToCalculate));
}

Task.WaitAll(tasks);

while (true)
{
    Console.WriteLine("Enter an integer (0-{0}) to find its square root...", numberToCalculate);
    Console.WriteLine(SquareRoots[int.Parse(Console.ReadLine())]);
}


static void CalculateSqrt(int start, int step, int stop)
{
    // Split the search based on a step size
    for (int i = start; i < stop; i += step)
    {
        SquareRoots[i] = SqrtByAlogorithm(i);
    }
}

// From: http://www.softwareandfinance.com/CSharp/Find_SQRT_Algorithm.html
static double SqrtByAlogorithm(double x)
{
    long numeric = (long)x;
    long n = numeric;
    long fraction = (long)((x - numeric) * 1000000); // 6 digits
    long f = fraction;
    int numdigits = 0, fnumdigits = 0, currdigits = 0;
    int tempresult = 0;
    int bOdd = 0, part = 0, tens = 1;
    int fractioncount = 0;
    double result = 0;
    int k, f1, f2, i, num, temp, quotient;

    for (numdigits = 0; n >= 10; numdigits++)
    {
        n = (n / 10);
    }
    numdigits++;
    for (fnumdigits = 0; f >= 10; fnumdigits++)
    {
        f = (f / 10);
    }
    fnumdigits++;
    if ((numdigits % 2) == 1)
    {
        bOdd = 1;
    }
    while (true)
    {
        tens = 1;
        currdigits = (bOdd == 1) ? (numdigits - 1) : (numdigits - 2);
        for (k = 0; k < currdigits; k++)
        {
            tens *= 10;
        }
        part = (int)numeric / tens;
        num = part;
        quotient = tempresult * 2;
        i = 0;
        temp = 0;
        for (i = 1; ; i++)
        {
            if (quotient == 0)
            {
                if (num - i * i < 0)
                {
                    tempresult = (i - 1);
                    break;
                }
            }
            else
            {
                temp = quotient * 10 + i;
                if (num - i * temp < 0)
                {
                    tempresult = quotient / 2 * 10 + i - 1;
                    break;
                }
            }
        }
        f1 = tempresult / 10;
        f2 = tempresult % 10;
        if (f1 == 0)
        {
            numeric = numeric - (tempresult * tempresult * tens);
        }
        else
        {
            numeric = numeric - ((f1 * 2 * 10 + f2) * f2 * tens);
        }
        if (numeric == 0 && fraction == 0)
        {
            if (currdigits > 0)
            {
                // Handle the Zero case
                tens = 1;
                currdigits = currdigits / 2;
                for (k = 0; k < currdigits; k++)
                {
                    tens *= 10;
                }
                tempresult *= tens;
            }
            break;
        }
        if (bOdd == 1)
        {
            numdigits -= 1;
            bOdd = 0;
        }
        else
        {
            numdigits -= 2;
        }
        if (numdigits <= 0)
        {
            if (numeric > 0 || fraction > 0)
            {
                if (fractioncount >= 5)
                {
                    break;
                }
                fractioncount++;
                numeric *= 100;
                if (fraction > 0)
                {
                    // Handle the fraction part for real numbers
                    fnumdigits -= 2;
                    tens = 1;
                    for (k = 0; k < fnumdigits; k++)
                    {
                        tens *= 10;
                    }
                    numeric += fraction / tens;
                    fraction = fraction % tens;
                }
                numdigits += 2;
            }
            else
                break;
        }
    }
    if (fractioncount == 0)
    {
        result = tempresult;
    }
    else
    {
        tens = 1;
        for (k = 0; k < fractioncount; k++)
        {
            tens *= 10;
        }
        result = (double)tempresult / tens;
    }
    return result;
}

public partial class Program
{
    public static Dictionary<int, double> SquareRoots { get; set; }
}