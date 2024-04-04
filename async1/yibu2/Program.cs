namespace yibu2;

class Program
{ 
    static async Task Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        //foreach(var s in Test2())
        //{
        //    Console.WriteLine(s);
        //}
        await foreach (var s in Test3())
        {
            Console.WriteLine(s);
        }
    }

    static IEnumerable<string> Test1()
    {
        List<string> list = new List<string>();
        list.Add("Hello");
        list.Add("yzk");
        list.Add("youzack.com");
        return list;
    }

    //static IEnumerable<string> Test2()
    //{
    //    yield return "Hello";
    //    yield return "yzk";
    //    yield return "youzack.com\"";
    //}

    static async IAsyncEnumerable<string> Test3()
    {
        yield return "Hello";
        yield return "yzk";
        yield return "youzack.com\"";
    }

    interface ITest
    {

        Task<int> GetCharCount(string file);
    }

    class Test : ITest
    {
        public async Task<int> GetCharCount(string file)
        {
            string s = await File.ReadAllTextAsync(file);
            return s.Length;
        }
    }
}

