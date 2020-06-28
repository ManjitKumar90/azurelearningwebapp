using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppInsightWebApp.Models;
using System.Net.Http;
using Microsoft.ApplicationInsights;
using System.Threading;

namespace AppInsightWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {

            _logger = logger;
        }

        public IActionResult Index()
        {
            TelemetryClient client = new TelemetryClient();
            client.TrackTrace("HomePage", new Dictionary<string, string>()
            {
                ["RequestId"] = Guid.NewGuid().ToString(),
                ["Message"] = "Home Controller Is Called"

            }); 
            CallGithub().Wait();
            return View();
        }

        private async Task CallGithub()
        {
            HttpClient client = new HttpClient();
            int exp = 2;
            int count = 1;
            do
            {
                Thread.Sleep(count * 1000);
                await client.GetAsync("https://api.github.com/users").ConfigureAwait(false);
                count = count * exp;
            }
            while (count < 16);
            
        }

        public IActionResult Privacy()
        {
            throw new InvalidOperationException("Something Went Wrong");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
