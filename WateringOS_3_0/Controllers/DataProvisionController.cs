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
        private bool SIM_MODE = true;

        Random rdm = new Random();
        public string GetLastTimestamp()   { /*if (!SIM_MODE) { return LogLists.RecentEntries.TimeStamp.ToString("dd/MMM/yyyy HH:mm:ss.fff"); } else */ { return DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss.fff"); } }
        public bool   CurrentStatePump()   { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStateValve1() { if (!SIM_MODE) { return false; } else { return false; } }
        public bool   CurrentStateValve2() { if (!SIM_MODE) { return false; } else { return false; } }
        public bool   CurrentStateValve3() { if (!SIM_MODE) { return false; } else { return false; } }
        public bool   CurrentStateValve4() { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStateValve5() { if (!SIM_MODE) { return false; } else { return false; } }
        public bool   CurrentStatePG5()    { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStatePG12()   { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStatePG24()   { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStatePF5()    { if (!SIM_MODE) { return false; } else { return false; } }
        public bool   CurrentStatePF12()   { if (!SIM_MODE) { return true; }  else { return true; } }
        public bool   CurrentStatePF24()   { if (!SIM_MODE) { return true; }  else { return true; } }
        public int    GetCurrentRain()     { if (!SIM_MODE) { return 1; }     else { return rdm.Next(2) + 1; } }
        public int    GetCurrentGround()   { if (!SIM_MODE) { return 2; }     else { return rdm.Next(2) + 1; ; } }
        public int    GetCurrentFlow1()    { if (!SIM_MODE) { return 101; }   else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow2()    { if (!SIM_MODE) { return 120; }   else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow3()    { if (!SIM_MODE) { return 134; }   else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow4()    { if (!SIM_MODE) { return 145; }   else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentFlow5()    { if (!SIM_MODE) { return 159; }   else { return rdm.Next(181)*10 + 200; } }
        public int    GetCurrentLevel()    { if (!SIM_MODE) { return 27; }    else { return rdm.Next(90) + 10; } }
        public double GetCurrentPressure() { if (!SIM_MODE) { return 3.7; }   else { return rdm.NextDouble() * 10; } }
        public int    GetCurrentTempCPU()  { if (!SIM_MODE) { return GetTemperature(); } else { return rdm.Next(20) + 30; } }
        public int    GetCurrentTempAmb()  { if (!SIM_MODE) { return 24; }    else { return rdm.Next(20) + 7; } }
        public int    GetCurrentTempExp()  { if (!SIM_MODE) { return 32; } else { return rdm.Next(20) + 15; ; } }

        public int GetTankAlarmLevel()     { if (!SIM_MODE) { return Settings.System.ALM_TankLevel; } else { return Settings.System.ALM_TankLevel; } }

        public string GetLevelData()     { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimLevelData.json"); }}
        public string GetEnvData()       { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimEnvData.json"); } }
        public string GetPowerData()     { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimPowerData.json"); } }
        public string GetJournalLogs()   { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimJournalLog.json"); } }
        public string GetJournalBuffer() { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimJournalBuffer.json"); } }

        public string GetWatering1() { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimWatering1Buffer.json"); } }
        public string GetWatering2() { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimWatering2Buffer.json"); } }
        public string GetWatering3() { /*if (!SIM_MODE) { return JsonConvert.SerializeObject()} else */{ return System.IO.File.ReadAllText(@"wwwroot/SimWatering3Buffer.json"); } }

        public int GetTemperature()
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
