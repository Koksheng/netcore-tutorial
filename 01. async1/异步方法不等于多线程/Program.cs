using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 异步方法不等于多线程
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("之前," + Thread.CurrentThread.ManagedThreadId);
            //double r = await CalcAsync(500);
            double r = await Calc2Async(500);
            Console.WriteLine($"r={r}");
            Console.WriteLine("之前," + Thread.CurrentThread.ManagedThreadId);
        }

        public static async Task<double> CalcAsync(int n)
        {
            //Console.WriteLine("CalcAsync," + Thread.CurrentThread.ManagedThreadId);
            //double result = 0;
            //Random rand = new Random();
            //for (var i = 0; i < n * n; i++)
            //{
            //    result += rand.NextDouble();
            //}
            //return result;

            return await Task.Run(() =>
            {
                Console.WriteLine("CalcAsync," + Thread.CurrentThread.ManagedThreadId);
                double result = 0;
                Random rand = new Random();
                for (var i = 0; i < n * n; i++)
                {
                    result += rand.NextDouble();
                }
                return result;
            });
        }
        public static Task<double> Calc2Async(int n)
        {
            
            return Task.Run(() =>
            {
                Console.WriteLine("CalcAsync," + Thread.CurrentThread.ManagedThreadId);
                double result = 0;
                Random rand = new Random();
                for (var i = 0; i < n * n; i++)
                {
                    result += rand.NextDouble();
                }
                return Task.FromResult(result); // double ----> Task, then can remove async
            });
        }

    }
}
