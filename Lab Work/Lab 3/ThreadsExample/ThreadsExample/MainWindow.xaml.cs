using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<int> primeNumbers;
        public MainWindow()
        {
            InitializeComponent();
            primeNumbers = new List<int>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a thread to find prime numbers
            ParameterizedThreadStart ts = new ParameterizedThreadStart(FindPrimeNumbers);
            // Run the thread on a thread pool thread
            var t = Task.Run(() => ts.Invoke(20000));
            // Register a callback to run when the thread finishes
            t.ContinueWith(FindPrimesFinished);
        }

        private void FindPrimesFinished(IAsyncResult iar)
        {
            //outputTextBox.Text = primeNumbers[19999].ToString(); // This doesn't work because it's not on the UI thread
            this.Dispatcher.Invoke(new Action<int>(UpdateTextBox), new object[] { primeNumbers[19999] });
        }

        private void UpdateTextBox(int number)
        {
            outputTextBox.Text = number.ToString();
        }

        private void FindPrimeNumbers(object param)
        {
            int primeCount = 0; int currentPossiblePrime = 1;
            while (primeCount < (int)param)
            {
                currentPossiblePrime++; int possibleFactor = 2; bool isPrime = true;
                while ((possibleFactor <= currentPossiblePrime / 2) && (isPrime == true))
                {
                    int possibleFactor2 = currentPossiblePrime / possibleFactor;
                    if (currentPossiblePrime == possibleFactor2 * possibleFactor)
                    {
                        isPrime = false;
                    }
                    possibleFactor++;
                }
                if (isPrime)
                {
                    primeCount++;
                    primeNumbers.Add(currentPossiblePrime);
                    // Use the Dispatcher to update the UI
                    this.Dispatcher.Invoke(
                        new Action<int>(UpdateTextBox),
                        new object[] { currentPossiblePrime });
                }
            }
        }
    }
}
