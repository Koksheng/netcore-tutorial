using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnetcorecancel1.Models;

namespace aspnetcorecancel1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        await Download3Async(@"https://www.youzack.com", 100, cancellationToken);
        return View();
    }

    static async Task Download3Async(string url, int n, CancellationToken cancellationToken)
    {
        using (HttpClient client = new HttpClient())
        {
            for (int i = 0; i < n; i++)
            {
                var resp = await client.GetAsync(url, cancellationToken);
                string html = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine($"{DateTime.Now} : {html}");
            }
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

