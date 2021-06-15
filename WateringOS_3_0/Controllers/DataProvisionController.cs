using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Iot.Device.CpuTemperature;
using Newtonsoft.Json;
using WateringOS_3_0.Models;

namespace WateringOS_3_0.Controllers
{
    public class DataProvisionController : Controller
    {
        private bool SIM_MODE = false;

        Random rdm = new Random();
        public string GetServerUptime()    { TimeSpan diffTime = DateTime.Now.Subtract(Globals.ServerStart); return String.Format("{0}d {1:D2}:{2:D2}:{3:D2}", diffTime.Days, diffTime.Hours, diffTime.Minutes, diffTime.Seconds); }
        public string GetSoftwareVersion() { return Globals.Version; }
        public string GetLastTimestamp()   { if (!SIM_MODE) { return LogLists.RecentEntries.TimeStamp.ToString("dd/MMM/yyyy HH:mm:ss.fff"); } else  { return DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss.fff"); } }
        public bool   CurrentStatePump()   { if (!SIM_MODE) { return LogLists.RecentEntries.Pump; }           else { return true; } }
        public bool   CurrentStateValve1() { if (!SIM_MODE) { return LogLists.RecentEntries.Valve1; }         else { return false; } }
        public bool   CurrentStateValve2() { if (!SIM_MODE) { return LogLists.RecentEntries.Valve2; }         else { return false; } }
        public bool   CurrentStateValve3() { if (!SIM_MODE) { return LogLists.RecentEntries.Valve3; }         else { return false; } }
        public bool   CurrentStateValve4() { if (!SIM_MODE) { return LogLists.RecentEntries.Valve4; }         else { return true; } }
        public bool   CurrentStateValve5() { if (!SIM_MODE) { return LogLists.RecentEntries.Valve5; }         else { return false; } }
        public bool   CurrentStatePG5()    { if (!SIM_MODE) { return LogLists.RecentEntries.PowerGood_5V; }   else { return true; } }
        public bool   CurrentStatePG12()   { if (!SIM_MODE) { return LogLists.RecentEntries.PowerGood_12V; }  else { return true; } }
        public bool   CurrentStatePG24()   { if (!SIM_MODE) { return LogLists.RecentEntries.PowerGood_24V; }  else { return true; } }
        public bool   CurrentStatePF5()    { if (!SIM_MODE) { return LogLists.RecentEntries.PowerFail_5V; }   else { return false; } }
        public bool   CurrentStatePF12()   { if (!SIM_MODE) { return LogLists.RecentEntries.PowerFail_12V; }  else { return true; } }
        public bool   CurrentStatePF24()   { if (!SIM_MODE) { return LogLists.RecentEntries.PowerFail_24V; }  else { return true; } }
        public int    GetCurrentRain()     { if (!SIM_MODE) { return LogLists.RecentEntries.Rain; }           else { return rdm.Next(2) + 1; } }
        public int    GetCurrentGround()   { if (!SIM_MODE) { return Globals.tmpGround; }         else { return rdm.Next(2) + 1; ; } }
        public int    GetCurrentFlow1()    { if (!SIM_MODE) { return LogLists.RecentEntries.Flow1; }          else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow2()    { if (!SIM_MODE) { return LogLists.RecentEntries.Flow2; }          else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow3()    { if (!SIM_MODE) { return LogLists.RecentEntries.Flow3; }          else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow4()    { if (!SIM_MODE) { return LogLists.RecentEntries.Flow4; }          else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow5()    { if (!SIM_MODE) { return LogLists.RecentEntries.Flow5; }          else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentLevel()    { if (!SIM_MODE) { return LogLists.RecentEntries.Tank; }           else { return rdm.Next(90) + 10; } }
        public int    GetCurrentTankForce(){ if (!SIM_MODE) { return LogLists.RecentEntries.LevelRaw; }       else { return rdm.Next(14000) + 1000; } }
        public double GetCurrentPressure() { if (!SIM_MODE) { return LogLists.RecentEntries.Pressure; }       else { return rdm.NextDouble() * 10; } }
        public int    GetCurrentTempCPU()  { if (!SIM_MODE) { return LogLists.RecentEntries.TempCPU; }        else { return rdm.Next(20) + 30; } }
        public int    GetCurrentTempAmb()  { if (!SIM_MODE) { return LogLists.RecentEntries.TempAmb; }        else { return rdm.Next(20) + 7; } }
        public int    GetCurrentTempExp()  { if (!SIM_MODE) { return LogLists.RecentEntries.TempExp; }        else { return rdm.Next(20) + 15; ; } }

        public int GetTankAlarmLevel()     { if (!SIM_MODE) { return Settings.System.ALM_TankLevel; } else { return Settings.System.ALM_TankLevel; } }

        public string GetLevelData()     { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.LevelLog.ToList());} else { return System.IO.File.ReadAllText(@"wwwroot/SimLevelData.json"); }}
        public string GetEnvData()       { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.EnvLog.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimEnvData.json"); } }
        public string GetPowerData()     { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.PowerLog.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimPowerData.json"); } }

        public string GetJournalLogs()    { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.JournalLog.ToList()); }    else { return System.IO.File.ReadAllText(@"wwwroot/SimJournalLog.json"); } }
        public string GetJournalBuffer()  { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.JournalBuffer.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimJournalBuffer.json"); } }
        public bool   ClearJournalBuffer(){ LogLists.JournalBuffer.Clear(); return true; }

        public string GetWatering1() { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.WateringLog1.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimWatering1Buffer.json"); } }
        public string GetWatering2() { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.WateringLog2.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimWatering2Buffer.json"); } }
        public string GetWatering3() { if (!SIM_MODE) { return JsonConvert.SerializeObject(LogLists.WateringLog3.ToList()); } else { return System.IO.File.ReadAllText(@"wwwroot/SimWatering3Buffer.json"); } }

        public static int GetTemperature()
        {
            CpuTemperature temperature = new CpuTemperature();
            if (temperature.IsAvailable)
            {
                return Convert.ToInt32(Math.Round(temperature.Temperature.Celsius));
            }
            else
            { return -10; }
        }
    }
}
