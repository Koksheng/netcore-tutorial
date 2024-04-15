using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线程切换1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < 10000; i++)
            {
                sb.Append("xxxxxxxxxx");
            }
            await File.WriteAllTextAsync(@"d:\temp\a\1.txt", sb.ToString());
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
    }
}
