using System;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WateringOS_3_0.Controllers;
using WateringOS_3_0.Models;

namespace WateringOS_3_0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            string strHostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] address = ipHostEntry.AddressList;
            foreach (IPAddress element in address) { sb.Append("\tThe Local IP Address: " + element.ToString() + "\n"); }
            Console.WriteLine($">>> Starting MVC Background Service for WateringOS 3.0\n");
            Console.WriteLine(sb.ToString());

            Parents.Host = CreateHostBuilder(args).Build();

            Parents.Logger_Program                   = Parents.Host.Services.GetRequiredService<ILogger<Program>>();
            Parents.Logger_Startup                   = Parents.Host.Services.GetRequiredService<ILogger<Startup>>();
            Parents.Logger_BackgroundTaskController  = Parents.Host.Services.GetRequiredService<ILogger<BackgroundTaskController>>();
            Parents.Logger_DataProvisionController   = Parents.Host.Services.GetRequiredService<ILogger<DataProvisionController>>();
            Parents.Logger_ManualController          = Parents.Host.Services.GetRequiredService<ILogger<ManualController>>();
            Parents.Logger_SettingsController        = Parents.Host.Services.GetRequiredService<ILogger<SettingsController>>();
            Parents.Logger_WateringController        = Parents.Host.Services.GetRequiredService<ILogger<WateringController>>();
            Parents.Logger_SimpleMembershipAttribute = Parents.Host.Services.GetRequiredService<ILogger<SimpleMembershipAttribute>>();
            Parents.Logger_GPIOController            = Parents.Host.Services.GetRequiredService<ILogger<GPIOController>>();
            Parents.Logger_SPIController             = Parents.Host.Services.GetRequiredService<ILogger<SPIController>>();
            Parents.Logger_SQLController             = Parents.Host.Services.GetRequiredService<ILogger<SQLController>>();
            Parents.Logger_TWIController             = Parents.Host.Services.GetRequiredService<ILogger<TWIController>>();

            if (BackgroundTaskController.Initialize())
            {
                Parents.Logger_Program.LogInformation("Controller Initialized");
            }
            else
            {
                Parents.Logger_Program.LogCritical("Controller failed initialization");
                return;
            }
            Parents.Logger_Program.LogDebug("System.DLY_Auto_LogOff: " + Settings.System.DLY_Auto_LogOff);
            Parents.Host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:8001");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
