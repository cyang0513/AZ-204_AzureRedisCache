using AzureRedisCache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureRedisCache.Controllers
{
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;
      private readonly ConnectionMultiplexer m_Redis;

      public HomeController(ILogger<HomeController> logger, IConfiguration config)
      {
         _logger = logger;
         m_Redis = ConnectionMultiplexer.Connect(config.GetSection("chyaredis").Value);
      }

      public async Task<IActionResult> Index()
      {
         var redisDb = m_Redis.GetDatabase();

         var exc =  redisDb.ExecuteAsync("CLIENT", "LIST");

         ViewData["ClientList"] = (await exc).ToString();

         if (redisDb.StringSet("TestKey", "Redis Test Value", new TimeSpan(1, 0, 0)))
         {
            ViewData["TestKey"] = redisDb.StringGet("TestKey");
         }

         return View();
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
}
