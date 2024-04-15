namespace 取消1;

class Program
{
    static async Task Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        //await DownloadAsync("https://www.youzack.com", 100);
        CancellationTokenSource cancellationtokensource = new CancellationTokenSource();
        //cancellationtokensource.CancelAfter(3000);
        CancellationToken cancellationtoken = cancellationtokensource.Token;
        Download3Async("https://www.youzack.com", 100, cancellationtoken);

        while(Console.ReadLine() != "q")
        {

        }
        cancellationtokensource.Cancel();
        Console.ReadLine();
    }

    static async Task DownloadAsync(string url, int n)
    {
        using(HttpClient client = new HttpClient())
        {
            for(int i = 0; i < n; i++)
            {
                string html = await client.GetStringAsync(url);
                Console.WriteLine($"{DateTime.Now} : {html}");
            }
        }
    }

    static async Task Download2Async(string url, int n, CancellationToken cancellationToken)
    {
        using (HttpClient client = new HttpClient())
        {
            for (int i = 0; i < n; i++)
            {
                string html = await client.GetStringAsync(url);
                Console.WriteLine($"{DateTime.Now} : {html}");
                //if (cancellationToken.IsCancellationRequested)
                //{
                //    Console.WriteLine("请求被取消");
                //    break;
                //}
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    static async Task Download3Async(string url, int n, CancellationToken cancellationToken)
    {
        using (HttpClient client = new HttpClient())
        {
            for (int i = 0; i < n; i++)
            {
                var resp = await client.GetAsync(url, cancellationToken);
                string html = await resp.Content.ReadAsStringAsync();
                File.WriteAllTextAsync("/Users/guekoksheng/Projects/net6 yangzhongke/netcore-tutorial/a/1.txt", html, cancellationToken);
                Console.WriteLine($"{DateTime.Now} : {html}");
            }
        }
    }
}

