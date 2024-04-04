using System.Linq;
using System.Threading;

namespace WhenAll1;

class Program
{
    static async Task Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        //Task<string> s1 = File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/1.txt");
        //Task<string> s2 = File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/2.txt");
        //Task<string> s3= File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/3.txt");

        //string[] strs = await Task.WhenAll(s1, s2, s3);
        //string ss1 = strs[0];
        //string ss2 = strs[1];
        //string ss3 = strs[2];
        //Console.WriteLine(ss1);
        //Console.WriteLine(ss2);
        //Console.WriteLine(ss3);

        string[] files = Directory.GetFiles("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a");
        Task<int>[] countTask = new Task<int>[files.Length];
        for(int i = 0; i<files.Length; i++)
        {
            string filename = files[i];
            Task<int> t = ReadCharsCount(filename);
            countTask[i] = t;
        }
        int[] count = await Task.WhenAll(countTask);
        int ans = count.Sum();
        Console.WriteLine(ans);

    }

    static async Task<int> ReadCharsCount(string filename)
    {
        string s = await File.ReadAllTextAsync(filename);
        return s.Length;
    }







    //static async Task Main(string[] args)
    //{
    //    //Console.WriteLine("Hello, World!");
    //    //Task<string> s1 = File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/1.txt");
    //    //Task<string> s2 = File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/2.txt");
    //    //Task<string> s3= File.ReadAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/3.txt");

    //    //string[] strs = await Task.WhenAll(s1, s2, s3);
    //    //string ss1 = strs[0];
    //    //string ss2 = strs[1];
    //    //string ss3 = strs[2];
    //    //Console.WriteLine(ss1);
    //    //Console.WriteLine(ss2);
    //    //Console.WriteLine(ss3);

    //    string[] files = Directory.GetFiles("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a");
    //    Task<int>[] countTasks = new Task<int>[files.Length];
    //    for (int i = 0; i < files.Length; i++)
    //    {
    //        string filename = files[i];
    //        Task<int> n = ReadCharsCount(filename);
    //        countTasks[i] = n;
    //    }
    //    int[] counts = await Task.WhenAll(countTasks);
    //    int c = counts.Sum();
    //    Console.WriteLine(c);

    //}

    //static async Task<int> ReadCharsCount(string filename)
    //{
    //    string s = await File.ReadAllTextAsync(filename);
    //    return s.Length;
    //}
}

