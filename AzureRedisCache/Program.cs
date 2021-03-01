using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureRedisCache
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                 webBuilder.ConfigureAppConfiguration(az => {
                    //var cb = az.Build();
                    az.AddAzureAppConfiguration(x=> {
                       
                       x.Connect(new Uri("https://chyaappconfig.azconfig.io"), new DefaultAzureCredential());

                       //Local:
                       //x.Connect(cb.GetConnectionString("AppConfig"));

                       //Local: Setup service principle to access key vault //appConfigSp
                       //Environment.SetEnvironmentVariable("AZURE_CLIENT_ID ", "7f5e090c-9557-47c8-930f-0a973e854f2c");
                       //Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "Hh1Ji4oOLWuTa-a30RM1gAI7vlVTjD~fvO");
                       //Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "4e6f57dc-a3d9-4a0c-818b-a7c1bb2b79f6");

                       x.ConfigureKeyVault(kv => {
                          kv.SetCredential(new DefaultAzureCredential());
                          });
                    });
                 });
                 webBuilder.UseStartup<Startup>();
              });
   }
}
