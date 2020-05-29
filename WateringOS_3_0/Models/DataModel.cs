using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WateringOS_3_0.Models
{
    public class cDataLog
    {
        public DateTime TimeStamp { get; set; }
        public int Flow1 { get; set; }
        public int Flow2 { get; set; }
        public int Flow3 { get; set; }
        public int Flow4 { get; set; }
        public int Flow5 { get; set; }
        public bool Pump { get; set; }
        public bool Valve1 { get; set; }
        public bool Valve2 { get; set; }
        public bool Valve3 { get; set; }
        public bool Valve4 { get; set; }
        public bool Valve5 { get; set; }

        public byte Tank { get; set; }
        public byte Rain { get; set; }
        public byte Ground { get; set; }
        public double Pressure { get; set; }
        public int TempCPU { get; set; }
        public int TempAmb { get; set; }
        public int TempExp { get; set; }

        public bool PowerGood_5V { get; set; }
        public bool PowerGood_12V { get; set; }
        public bool PowerGood_24V { get; set; }
        public bool PowerFail_5V { get; set; }
        public bool PowerFail_12V { get; set; }
        public bool PowerFail_24V { get; set; }
        public bool WatchdogPrealarm { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cEnvLog
    {
        public DateTime TimeStamp { get; set; }
        public byte Rain { get; set; }
        public byte Ground { get; set; }
        public int TempCPU { get; set; }
        public int TempAmb { get; set; }
        public int TempExp { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cPowerLog
    {
        public DateTime TimeStamp { get; set; }
        public byte PowerGood_5V { get; set; }
        public byte PowerGood_12V { get; set; }
        public byte PowerGood_24V { get; set; }
        public byte PowerFail_5V { get; set; }
        public byte PowerFail_12V { get; set; }
        public byte PowerFail_24V { get; set; }
        public byte WatchdogPrealarm { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cLevelLog
    {
        public DateTime TimeStamp { get; set; }
        public byte Tank { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cWaterLog
    {
        public DateTime TimeStamp { get; set; }
        public int Flow1 { get; set; }
        public int Flow2 { get; set; }
        public int Flow3 { get; set; }
        public int Flow4 { get; set; }
        public int Flow5 { get; set; }
        public byte Pump { get; set; }
        public byte Valve1 { get; set; }
        public byte Valve2 { get; set; }
        public byte Valve3 { get; set; }
        public byte Valve4 { get; set; }
        public byte Valve5 { get; set; }
        public double Pressure { get; set; }
        public int TempCPU { get; set; }

        public byte PowerGood_5V { get; set; }
        public byte PowerGood_12V { get; set; }
        public byte PowerGood_24V { get; set; }
        public byte PowerFail_5V { get; set; }
        public byte PowerFail_12V { get; set; }
        public byte PowerFail_24V { get; set; }
        public byte WatchdogPrealarm { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cRecentEntries
    {
        public DateTime TimeStamp { get; set; }
        public int Flow1 { get; set; }
        public int Flow2 { get; set; }
        public int Flow3 { get; set; }
        public int Flow4 { get; set; }
        public int Flow5 { get; set; }
        public bool Pump { get; set; }
        public bool Valve1 { get; set; }
        public bool Valve2 { get; set; }
        public bool Valve3 { get; set; }
        public bool Valve4 { get; set; }
        public bool Valve5 { get; set; }

        public byte Tank { get; set; }
        public byte Rain { get; set; }
        public byte Ground { get; set; }
        public double Pressure { get; set; }
        public byte LevelRaw { get; set; }
        public int TempCPU { get; set; }
        public int TempAmb { get; set; }
        public int TempExp { get; set; }
        public int TempAmbFiltered { get; set; }
        public int TempCPUFiltered { get; set; }
        public int TempExpFiltered { get; set; }

        public bool PowerGood_5V { get; set; }
        public bool PowerGood_12V { get; set; }
        public bool PowerGood_24V { get; set; }
        public bool PowerFail_5V { get; set; }
        public bool PowerFail_12V { get; set; }
        public bool PowerFail_24V { get; set; }
        public bool WatchdogPrealarm { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class cJournal
    {
        public string TimeStamp { get; set; }
        public string Type { get; set; }
        public string App { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }
    }
    public class LogLists
    {
        public static List<cDataLog>  DataLog       = new List<cDataLog>(65535);
        public static List<cEnvLog>   EnvLog        = new List<cEnvLog>(65535);     // LogData for Temperature, Ground and Rain
        public static List<cPowerLog> PowerLog      = new List<cPowerLog>(65535);   // LogData for Power Supply Status and WatchDog
        public static List<cLevelLog> LevelLog      = new List<cLevelLog>(65535);   // LogData for TankLevel
        public static List<cWaterLog> WateringLog1  = new List<cWaterLog>(65535);   // LogData when watering is active
        public static List<cWaterLog> WateringLog2  = new List<cWaterLog>(65535);   // LogData when watering is active
        public static List<cWaterLog> WateringLog3  = new List<cWaterLog>(65535);   // LogData when watering is active
        public static List<cJournal>  JournalLog    = new List<cJournal>(65535);    // Log Messages from Watering System
        public static List<cJournal>  JournalBuffer = new List<cJournal>(255);      // Session Buffer for new Messages

        public static cRecentEntries  RecentEntries = new cRecentEntries();
    }
}
