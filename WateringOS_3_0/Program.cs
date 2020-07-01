using System;
using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WateringOS_3_0.Controllers;
using WateringOS_3_0.Models;

/* DISCLAIMER

Watering OS - (C) Michael Kollmeyer 2020  
  
This file is part of WateringOS.

    WateringOS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WateringOS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WateringOS.  If not, see<https://www.gnu.org/licenses/>.

*/

namespace WateringOS_3_0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Globals.ServerStart = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            string strHostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] address = ipHostEntry.AddressList;
            foreach (IPAddress element in address) { sb.Append("\tThe Local IP Address: " + element.ToString() + "\n"); }
            Console.WriteLine($">>> Starting MVC Background Service for WateringOS 3.0\n");
            BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "Program", LogType.Status, "Starting MVC Background Service for WateringOS 3.0", "The service just have started, initiazing MVC Background Service for WateringOS 3.0");
            Console.WriteLine(sb.ToString());
            BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "Program", LogType.Status, "Received IP for service", "Following IPs have been assigned:\n"+sb.ToString());

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
                    webBuilder.UseUrls("http://*:8081");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
