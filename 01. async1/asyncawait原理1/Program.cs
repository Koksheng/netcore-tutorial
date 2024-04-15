using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace asyncawait原理1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (HttpClient htttpClient = new HttpClient())
            {
                string html = await HttpClient.GetStringAsync("https://www.taobao.com");
            }
            string txt = "hello yzk";
            string filename = @"e:\temp\a\1.txt";
            await File.WriteAllTextAsync(filename, txt);
            Console.WriteLine("Write in Successfullly");
            string s = await File.ReadAllTextAsync(filename);
            Console.WriteLine("File content" + s);
        }
    }
}
