using System.Diagnostics;

namespace HW_Tasks.Services
{
    internal class Calculator
    {
        List<uint> numbers;

        public void Calculate(int size)
        {
            FillNumbers(size);

            Stopwatch timer = new Stopwatch();
            Console.WriteLine($"Рассчитываем...");

            timer.Start();
            ulong result = CalculateSumSimple();
            timer.Stop();
            Console.WriteLine($"Время выполнения обычным методом: {timer.ElapsedMilliseconds}ms, результат: {result}");

            timer.Restart();
            result = CalculateSumParallelLinq();
            timer.Stop();
            Console.WriteLine($"Время выполнения методом AsParallel LINQ: {timer.ElapsedMilliseconds}ms, результат: {result}");

            timer.Restart();
            result = CalculateSumParallelFor();
            timer.Stop();
            Console.WriteLine($"Время выполнения методом Parallel.ForEach: {timer.ElapsedMilliseconds}ms, результат: {result}");

            timer.Restart();
            result = CalculateSumMultiThread();
            timer.Stop();
            Console.WriteLine($"Время выполнения методом async: {timer.ElapsedMilliseconds}ms, результат: {result}");
            Console.WriteLine();
        }

        private void FillNumbers(int size)
        {
            Console.WriteLine($"Подготовка массива, длина: {size}");
            Random rnd = new Random();
            this.numbers = new List<uint>();
            
            for (var i = 0; i < size; i++)
            {
                uint x = (uint)rnd.Next(0, 1000);
                numbers.Add(x);
            }
        }




        public ulong CalculateSumSimple()
        {
            ulong sum = 0;
            foreach (var i in this.numbers)
            {
                sum += i;
            }
            return sum;
        }

        public ulong CalculateSumParallelLinq()
        {
            long sum = this.numbers.AsParallel().Sum(x=>(uint)x);
            return (ulong)sum;
        }

        public ulong CalculateSumParallelFor()
        {
            object monitor = new object();
            ulong sum = 0;
            Parallel.ForEach(this.numbers, (i) =>
            {
                lock (monitor)
                {
                    sum += i;
                }
            });
            return sum;
        }

        public ulong CalculateSumMultiThread()
        {
            var processorCount = Environment.ProcessorCount; 
            var size = numbers.Count(); 
            var partSize = size / processorCount; 
            ulong sum = 0;
            List<Task> tasks = new List<Task>();
            object monitor = new object();
            for (var i = 0; i < processorCount; Interlocked.Increment(ref i))
            {
                var currentPartSize = i == processorCount - 1 ? size - (partSize * (i - 1)) : partSize;
                    
                var currentNumbers = numbers.Skip(i * partSize).Take(currentPartSize);
                tasks.Add(Task.Run(() =>
                {
                    lock (monitor) 
                    {
                        sum += (ulong)(currentNumbers.Sum(x => (uint)x));
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());  
            
            return sum;
        }


    }
}
