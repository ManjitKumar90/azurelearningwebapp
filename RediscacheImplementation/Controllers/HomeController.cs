using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RediscacheImplementation.Models;
using RediscacheImplementation.Services;

namespace RediscacheImplementation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ICacheService cacheService;
        public HomeController(ILogger<HomeController> logger, ICacheService cacheService)
        {
            _logger = logger;
            this.cacheService = cacheService;
        }

        public IActionResult Index()
        {
            Employee employee =  cacheService.GetData<Employee>("employeeId").GetAwaiter().GetResult();

            if(employee == null)
            {
                return Content("Cache Has Not been Set");
            }

            return Content(JsonConvert.SerializeObject(employee));
            
        }

        public IActionResult SetCache()
        {
            var employee = new Employee()
            {
                Id = "123456",
                Name = "Rohit"
            };
            cacheService.SetData<Employee>("employeeId", employee, TimeSpan.FromMinutes(3));
            return Content("Cache Has been Set");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
