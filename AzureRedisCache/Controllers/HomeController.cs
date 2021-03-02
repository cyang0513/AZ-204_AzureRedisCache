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
         var redisDb = m_Redis.GetDatabase();
         var subscriber = m_Redis.GetSubscriber();

         //Transaction
         var redisTrans = redisDb.CreateTransaction();

         redisTrans.StringSetAsync("TransKey1","Transaction value 1");
         redisTrans.StringSetAsync("TransKey2","Transaction value 2");
         redisTrans.StringSetAsync("TransKey3","Transaction value 3");

         redisTrans.Execute();

         ViewData["TransKey1"] = redisDb.StringGet("TransKey1");
         ViewData["TransKey2"] = redisDb.StringGet("TransKey2");
         ViewData["TransKey3"] = redisDb.StringGet("TransKey3");


         return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
