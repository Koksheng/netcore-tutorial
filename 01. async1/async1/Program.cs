using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace async1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await DownloadHtmlAsync("https://www.youzack.com",@"d:\temp\a\1.txt");
            Console.WriteLine("ok");
        }

        static async Task DownloadHtmlAsync(string url, string filename)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string html = await httpClient.GetStringAsync(url);
                await File.WriteAllTextAsync(filename, html);
            }
        }
    }
}
