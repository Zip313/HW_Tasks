using HW_Tasks.Services;
using System.Diagnostics;

namespace HW_Tasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Calculator calculator = new Calculator();

            calculator.Calculate(100000);
            calculator.Calculate(1000000);
            calculator.Calculate(10000000);

            Console.ReadKey();
        }
    }
}