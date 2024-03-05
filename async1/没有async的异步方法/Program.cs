
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 没有async的异步方法
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string s = await ReadAsync(1);
            Console.WriteLine(s);
        }

        //static async Task<string> ReadAsync(int num)
        //{
        //    if(num == 1)
        //    {
        //        string s = await File.ReadAllTextAsync(@"d:\temp\a\1.txt");
        //        return s;
        //    }
        //    else if (num == 2)
        //    {
        //        string s = await File.ReadAllTextAsync(@"d:\temp\a\2.txt");
        //        return s;
        //    }
        //    else
        //    {
        //        throw new ArgumentException();
        //    }
        //}

        static Task<string> ReadAsync(int num)
        {
            if (num == 1)
            {
                return File.ReadAllTextAsync(@"d:\temp\a\1.txt");
            }
            else if (num == 2)
            {
                return File.ReadAllTextAsync(@"d:\temp\a\2.txt");
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
