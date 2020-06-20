using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace WateringOS_3_0.Models
{
    public static class Parents
    {
        public static IHost Host;
        public static ILogger Logger_Program;
        public static ILogger Logger_Startup;
        public static ILogger Logger_BackgroundTaskController;
        public static ILogger Logger_DataProvisionController;
        public static ILogger Logger_ManualController;
        public static ILogger Logger_SettingsController;
        public static ILogger Logger_WateringController;
        public static ILogger Logger_SimpleMembershipAttribute;

        public static ILogger Logger_GPIOController;
        public static ILogger Logger_TWIController;
        public static ILogger Logger_SPIController;
        public static ILogger Logger_SQLController;
    }
    public static class Authorization
    {
        public static bool GrantAccess(string phrase)
        {
            if (phrase == "3507866") { return true; } else { return false; }
        }
    }
    public static class Globals
    {
        public static readonly  bool            AppInDebug       = false;
        public static readonly  string          Version          = "v3.1-beta-0620";
        public static readonly  Timer           FastTask         = new Timer(200);
        public static readonly  Timer           MainTask         = new Timer(1000);
        public static readonly  Timer           SaveTask         = new Timer(1200000);
        public static readonly  Timer           LevelLogger      = new Timer(1200000);
        public static readonly  Timer           EnvLogger        = new Timer(1200000);
        public static readonly  Timer           PowerLogger      = new Timer(60000);
        public static readonly  GPIOController  GpioServer       = new GPIOController();
        public static readonly  TWIController   TwiServer        = new TWIController();
        public static readonly  SQLController   SqlServer        = new SQLController();
        public static readonly  SPIController   SpiServer        = new SPIController();
        public static           bool            IsInitialized    = false;
        public static           DateTime        ServerStart;
        public static           int             vTest            = 0;
        public static           byte            WateringRecord   = 0;
        public static           bool            WateringActive   = false;
        public static           bool        ALM_WarnTempCPUactive= false;
        public static           bool        ALM_MaxTempCPUactive = false;
        public static           bool     ALM_WarnTankLevelactive = false;
        public static           bool            SqlIsBusy        = false;
        public static           string          SqlDat_Prefix    = "INSERT INTO Signals(DateTime, Flow1, Flow2, Flow3, Flow4, Flow5, Rain, Ground, TankLevel, Pressure, TempCPU, TempAmb, TempExp, Pump, Valve1, Valve2, Valve3, Valve4, Valve5, PG5, PG12, PG24, PF5, PF12, PF24, Watchdog, TEST) VALUES ";
        public static           string          SqlDat_Data      = "";
        public static           string          SqlLog_Prefix    = "INSERT INTO Log(TimeStamp, Instance, Type, Name, Details, TEST) VALUES ";
        public static           string          SqlLog_Data      = "";

        public static readonly char[]           IllegalCharacters= { (char)0x27, (char)0x3B };
        public static readonly char[]           ReplaceCharacters= { (char)0x5E, (char)0x5E };
    }
}