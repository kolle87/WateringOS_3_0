using System;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WateringOS_3_0.Models;

namespace WateringOS_3_0.Controllers
{
    public class SettingsController : Controller
    {
        // Load and Save Settings
        public bool ReadFromFile()
        {
            try
            {
                Settings.System     = JsonConvert.DeserializeObject<cSystemSettings>(System.IO.File.ReadAllText(@"wwwroot/systemsettings.json"));
                Settings.Watering   = JsonConvert.DeserializeObject<cWateringSettings>(System.IO.File.ReadAllText(@"wwwroot/wateringsettings.json"));
                Console.WriteLine($"SettingsController: ReadFromFile() OK");
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Information, "ReadFromFile() OK", "Settings have been loaded from the *.json file");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SettingsController: ReadFromFile() " + e.Message);
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Error, "ReadFromFile() Fail", e.Message);
                return false;
            }
            
        }
        public bool WriteToFile()
        {
            try
            {
                System.IO.File.WriteAllText(@"wwwroot/systemsettings.json",   JsonConvert.SerializeObject(Settings.System));
                System.IO.File.WriteAllText(@"wwwroot/wateringsettings.json", JsonConvert.SerializeObject(Settings.Watering));
                Console.WriteLine($"SettingsController: WriteToFile() OK");
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Information, "WriteToFile() OK", "Settings have been written to the *.json file");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SettingsController: WriteToFile() " + e.Message);
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Error, "WriteToFile() Fail", e.Message);
                return false;
            }

        }
        public static bool ReadFromFileStatic()
        {
            try
            {
                Settings.System = JsonConvert.DeserializeObject<cSystemSettings>(System.IO.File.ReadAllText(@"wwwroot/systemsettings.json"));
                Settings.Watering = JsonConvert.DeserializeObject<cWateringSettings>(System.IO.File.ReadAllText(@"wwwroot/wateringsettings.json"));
                Console.WriteLine($"SettingsController: ReadFromFileStatic() OK");
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Information, "ReadFromFileStatic() OK", "Settings have been loaded by static read of the *.json file");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SettingsController: ReadFromFileStatic() " + e.Message);
                BackgroundTaskController.AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[APP] SettingsController", LogType.Error, "ReadFromFileStatic() Fail", e.Message);
                return false;
            }

        }
        // provide settings
        public int Task_cycle()         { return Settings.System.Task_cycle; }
        public int FastLog()            { return Settings.System.FastLog; }
        public int AutoSaveLog()        { return Settings.System.Save_cycle; }
        public int Log_Enviroment()     { return Settings.System.Log_Enviroment; }
        public int Log_Power()          { return Settings.System.Log_Power; }
        public int Log_Level()          { return Settings.System.Log_Level; }

        public int DLY_Auto_LogOff()    { return Settings.System.DLY_Auto_LogOff; }

        public int Tank_Min()           { return Settings.System.Tank_Min; }
        public int Tank_Max()           { return Settings.System.Tank_Max; }

        public int Wat_min_tank()       { return Settings.System.Wat_min_tank; }
        public int Wat_min_vol()        { return Settings.System.Wat_min_vol; }
        public int Wat_min_time()       { return Settings.System.Wat_min_time; }
        public int Wat_max_vol()        { return Settings.System.Wat_max_vol; }
        public int Wat_max_time()       { return Settings.System.Wat_max_time; }

        public int ALM_TankLevel()      { return Settings.System.ALM_TankLevel; }
        public int ALM_MinPress()       { return Settings.System.ALM_MinPress; }
        public int ALM_MaxPress()       { return Settings.System.ALM_MaxPress; }
        public int ALM_NoPumpDelay()    { return Settings.System.ALM_NoPumpDelay; }
        public int ALM_NoValveDelay()   { return Settings.System.ALM_NoValveDelay; }
        public int ALM_PressOffDelay()  { return Settings.System.ALM_PressOffDelay; }
        public int ALM_PressOffValue()  { return Settings.System.ALM_PressOffValue; }
        public int ALM_WarnTempCPU()    { return Settings.System.ALM_WarnTempCPU; }
        public int ALM_MaxTempCPU()     { return Settings.System.ALM_MaxTempCPU; }

        public int DLY_ValveOpen()      { return Settings.System.DLY_ValveOpen; }
        public int DLY_PumpStop()       { return Settings.System.DLY_PumpStop; }

        // Watering Settings
        public string Plant_1_Name() { return Settings.Watering.Plant_1_Name; }
        public string Plant_2_Name() { return Settings.Watering.Plant_2_Name; }
        public string Plant_3_Name() { return Settings.Watering.Plant_3_Name; }
        public string Plant_4_Name() { return Settings.Watering.Plant_4_Name; }
        public string Plant_5_Name() { return Settings.Watering.Plant_5_Name; }

        public int TimeMorning()  { return Settings.Watering.TimeMorning; }
        public int TimeNoon()     { return Settings.Watering.TimeNoon; }
        public int TimeEvening()  { return Settings.Watering.TimeEvening; }

        public bool Morning_1()   { return Settings.Watering.Morning_1; }
        public bool Morning_2()   { return Settings.Watering.Morning_2; }
        public bool Morning_3()   { return Settings.Watering.Morning_3; }
        public bool Morning_4()   { return Settings.Watering.Morning_4; }
        public bool Morning_5()   { return Settings.Watering.Morning_5; }
        public bool Noon_1()      { return Settings.Watering.Noon_1; }
        public bool Noon_2()      { return Settings.Watering.Noon_2; }
        public bool Noon_3()      { return Settings.Watering.Noon_3; }
        public bool Noon_4()      { return Settings.Watering.Noon_4; }
        public bool Noon_5()      { return Settings.Watering.Noon_5; }
        public bool Evening_1()   { return Settings.Watering.Evening_1; }
        public bool Evening_2()   { return Settings.Watering.Evening_2; }
        public bool Evening_3()   { return Settings.Watering.Evening_3; }
        public bool Evening_4()   { return Settings.Watering.Evening_4; }
        public bool Evening_5()   { return Settings.Watering.Evening_5; }

        public int Volume_1()     { return Settings.Watering.Volume_1; }
        public int Volume_2()     { return Settings.Watering.Volume_2; }
        public int Volume_3()     { return Settings.Watering.Volume_3; }
        public int Volume_4()     { return Settings.Watering.Volume_4; }
        public int Volume_5()     { return Settings.Watering.Volume_5; }

        public int RAF_1()        { return Settings.Watering.RAF_1; }
        public int RAF_2()        { return Settings.Watering.RAF_2; }
        public int RAF_3()        { return Settings.Watering.RAF_3; }
        public int RAF_4()        { return Settings.Watering.RAF_4; }
        public int RAF_5()        { return Settings.Watering.RAF_5; }

        public int GAF_1()        { return Settings.Watering.GAF_1; }
        public int GAF_2()        { return Settings.Watering.GAF_2; }
        public int GAF_3()        { return Settings.Watering.GAF_3; }
        public int GAF_4()        { return Settings.Watering.GAF_4; }
        public int GAF_5()        { return Settings.Watering.GAF_5; }

        public bool Plant_1_Mon() { return Settings.Watering.Plant_1_Mon; }
        public bool Plant_1_Tue() { return Settings.Watering.Plant_1_Tue; }
        public bool Plant_1_Wed() { return Settings.Watering.Plant_1_Wed; }
        public bool Plant_1_Thu() { return Settings.Watering.Plant_1_Thu; }
        public bool Plant_1_Fri() { return Settings.Watering.Plant_1_Fri; }
        public bool Plant_1_Sat() { return Settings.Watering.Plant_1_Sat; }
        public bool Plant_1_Sun() { return Settings.Watering.Plant_1_Sun; }

        public bool Plant_2_Mon() { return Settings.Watering.Plant_2_Mon; }
        public bool Plant_2_Tue() { return Settings.Watering.Plant_2_Tue; }
        public bool Plant_2_Wed() { return Settings.Watering.Plant_2_Wed; }
        public bool Plant_2_Thu() { return Settings.Watering.Plant_2_Thu; }
        public bool Plant_2_Fri() { return Settings.Watering.Plant_2_Fri; }
        public bool Plant_2_Sat() { return Settings.Watering.Plant_2_Sat; }
        public bool Plant_2_Sun() { return Settings.Watering.Plant_2_Sun; }

        public bool Plant_3_Mon() { return Settings.Watering.Plant_3_Mon; }
        public bool Plant_3_Tue() { return Settings.Watering.Plant_3_Tue; }
        public bool Plant_3_Wed() { return Settings.Watering.Plant_3_Wed; }
        public bool Plant_3_Thu() { return Settings.Watering.Plant_3_Thu; }
        public bool Plant_3_Fri() { return Settings.Watering.Plant_3_Fri; }
        public bool Plant_3_Sat() { return Settings.Watering.Plant_3_Sat; }
        public bool Plant_3_Sun() { return Settings.Watering.Plant_3_Sun; }

        public bool Plant_4_Mon() { return Settings.Watering.Plant_4_Mon; }
        public bool Plant_4_Tue() { return Settings.Watering.Plant_4_Tue; }
        public bool Plant_4_Wed() { return Settings.Watering.Plant_4_Wed; }
        public bool Plant_4_Thu() { return Settings.Watering.Plant_4_Thu; }
        public bool Plant_4_Fri() { return Settings.Watering.Plant_4_Fri; }
        public bool Plant_4_Sat() { return Settings.Watering.Plant_4_Sat; }
        public bool Plant_4_Sun() { return Settings.Watering.Plant_4_Sun; }

        public bool Plant_5_Mon() { return Settings.Watering.Plant_5_Mon; }
        public bool Plant_5_Tue() { return Settings.Watering.Plant_5_Tue; }
        public bool Plant_5_Wed() { return Settings.Watering.Plant_5_Wed; }
        public bool Plant_5_Thu() { return Settings.Watering.Plant_5_Thu; }
        public bool Plant_5_Fri() { return Settings.Watering.Plant_5_Fri; }
        public bool Plant_5_Sat() { return Settings.Watering.Plant_5_Sat; }
        public bool Plant_5_Sun() { return Settings.Watering.Plant_5_Sun; }
//---------------------------------------------------------------------------
// receive settings
        public void Get_Task_cycle(int data) { Settings.System.Task_cycle = data; }
        public void Get_AutoSaveLog(int data) { Settings.System.Save_cycle = data; }
        public void Get_FastLog(int data) { Settings.System.FastLog = data; }
        public void Get_Log_Enviroment(int data) { Settings.System.Log_Enviroment = data; }
        public void Get_Log_Power(int data) { Settings.System.Log_Power = data; }
        public void Get_Log_Level(int data) { Settings.System.Log_Level = data; }

        public void Get_DLY_Auto_LogOff(int data) { Settings.System.DLY_Auto_LogOff = data; }

        public void Get_Tank_Min(int data) { Settings.System.Tank_Min = data; }
        public void Get_Tank_Max(int data) { Settings.System.Tank_Max = data; }

        public void Get_Wat_min_tank(int data) { Settings.System.Wat_min_tank = data; }
        public void Get_Wat_min_vol(int data) { Settings.System.Wat_min_vol = data; }
        public void Get_Wat_min_time(int data) { Settings.System.Wat_min_time = data; }
        public void Get_Wat_max_vol(int data) { Settings.System.Wat_max_vol = data; }
        public void Get_Wat_max_time(int data) { Settings.System.Wat_max_time = data; }

        public void Get_ALM_TankLevel(int data) { Settings.System.ALM_TankLevel = data; }
        public void Get_ALM_MinPress(int data) { Settings.System.ALM_MinPress = data; }
        public void Get_ALM_MaxPress(int data) { Settings.System.ALM_MaxPress = data; }
        public void Get_ALM_NoPumpDelay(int data) { Settings.System.ALM_NoPumpDelay = data; }
        public void Get_ALM_NoValveDelay(int data) { Settings.System.ALM_NoValveDelay = data; }
        public void Get_ALM_PressOffDelay(int data) { Settings.System.ALM_PressOffDelay = data; }
        public void Get_ALM_PressOffValue(int data) { Settings.System.ALM_PressOffValue = data; }
        public void Get_ALM_WarnTempCPU(int data) { Settings.System.ALM_WarnTempCPU = data; }
        public void Get_ALM_MaxTempCPU(int data) { Settings.System.ALM_MaxTempCPU = data; }

        public void Get_DLY_ValveOpen(int data) { Settings.System.DLY_ValveOpen = data; }
        public void Get_DLY_PumpStop(int data) { Settings.System.DLY_PumpStop = data; }

        // Watering Settings
        public void Get_Plant_1_Name(string data) { Settings.Watering.Plant_1_Name = data; }
        public void Get_Plant_2_Name(string data) { Settings.Watering.Plant_2_Name = data; }
        public void Get_Plant_3_Name(string data) { Settings.Watering.Plant_3_Name = data; }
        public void Get_Plant_4_Name(string data) { Settings.Watering.Plant_4_Name = data; }
        public void Get_Plant_5_Name(string data) { Settings.Watering.Plant_5_Name = data; }

        public void Get_TimeMorning(int data) { Settings.Watering.TimeMorning = data; }
        public void Get_TimeNoon(int data) { Settings.Watering.TimeNoon = data; }
        public void Get_TimeEvening(int data) { Settings.Watering.TimeEvening = data; }

        public void Get_Morning_1(bool data) { Settings.Watering.Morning_1 = data; }
        public void Get_Morning_2(bool data) { Settings.Watering.Morning_2 = data; }
        public void Get_Morning_3(bool data) { Settings.Watering.Morning_3 = data; }
        public void Get_Morning_4(bool data) { Settings.Watering.Morning_4 = data; }
        public void Get_Morning_5(bool data) { Settings.Watering.Morning_5 = data; }
        public void Get_Noon_1(bool data) { Settings.Watering.Noon_1 = data; }
        public void Get_Noon_2(bool data) { Settings.Watering.Noon_2 = data; }
        public void Get_Noon_3(bool data) { Settings.Watering.Noon_3 = data; }
        public void Get_Noon_4(bool data) { Settings.Watering.Noon_4 = data; }
        public void Get_Noon_5(bool data) { Settings.Watering.Noon_5 = data; }
        public void Get_Evening_1(bool data) { Settings.Watering.Evening_1 = data; }
        public void Get_Evening_2(bool data) { Settings.Watering.Evening_2 = data; }
        public void Get_Evening_3(bool data) { Settings.Watering.Evening_3 = data; }
        public void Get_Evening_4(bool data) { Settings.Watering.Evening_4 = data; }
        public void Get_Evening_5(bool data) { Settings.Watering.Evening_5 = data; }

        public void Get_Volume_1(int data) { Settings.Watering.Volume_1 = data; }
        public void Get_Volume_2(int data) { Settings.Watering.Volume_2 = data; }
        public void Get_Volume_3(int data) { Settings.Watering.Volume_3 = data; }
        public void Get_Volume_4(int data) { Settings.Watering.Volume_4 = data; }
        public void Get_Volume_5(int data) { Settings.Watering.Volume_5 = data; }

        public void Get_RAF_1(int data) { Settings.Watering.RAF_1 = data; }
        public void Get_RAF_2(int data) { Settings.Watering.RAF_2 = data; }
        public void Get_RAF_3(int data) { Settings.Watering.RAF_3 = data; }
        public void Get_RAF_4(int data) { Settings.Watering.RAF_4 = data; }
        public void Get_RAF_5(int data) { Settings.Watering.RAF_5 = data; }

        public void Get_GAF_1(int data) { Settings.Watering.GAF_1 = data; }
        public void Get_GAF_2(int data) { Settings.Watering.GAF_2 = data; }
        public void Get_GAF_3(int data) { Settings.Watering.GAF_3 = data; }
        public void Get_GAF_4(int data) { Settings.Watering.GAF_4 = data; }
        public void Get_GAF_5(int data) { Settings.Watering.GAF_5 = data; }

        public void Get_Plant_1_Mon(bool data) { Settings.Watering.Plant_1_Mon = data; }
        public void Get_Plant_1_Tue(bool data) { Settings.Watering.Plant_1_Tue = data; }
        public void Get_Plant_1_Wed(bool data) { Settings.Watering.Plant_1_Wed = data; }
        public void Get_Plant_1_Thu(bool data) { Settings.Watering.Plant_1_Thu = data; }
        public void Get_Plant_1_Fri(bool data) { Settings.Watering.Plant_1_Fri = data; }
        public void Get_Plant_1_Sat(bool data) { Settings.Watering.Plant_1_Sat = data; }
        public void Get_Plant_1_Sun(bool data) { Settings.Watering.Plant_1_Sun = data; }

        public void Get_Plant_2_Mon(bool data) { Settings.Watering.Plant_2_Mon = data; }
        public void Get_Plant_2_Tue(bool data) { Settings.Watering.Plant_2_Tue = data; }
        public void Get_Plant_2_Wed(bool data) { Settings.Watering.Plant_2_Wed = data; }
        public void Get_Plant_2_Thu(bool data) { Settings.Watering.Plant_2_Thu = data; }
        public void Get_Plant_2_Fri(bool data) { Settings.Watering.Plant_2_Fri = data; }
        public void Get_Plant_2_Sat(bool data) { Settings.Watering.Plant_2_Sat = data; }
        public void Get_Plant_2_Sun(bool data) { Settings.Watering.Plant_2_Sun = data; }

        public void Get_Plant_3_Mon(bool data) { Settings.Watering.Plant_3_Mon = data; }
        public void Get_Plant_3_Tue(bool data) { Settings.Watering.Plant_3_Tue = data; }
        public void Get_Plant_3_Wed(bool data) { Settings.Watering.Plant_3_Wed = data; }
        public void Get_Plant_3_Thu(bool data) { Settings.Watering.Plant_3_Thu = data; }
        public void Get_Plant_3_Fri(bool data) { Settings.Watering.Plant_3_Fri = data; }
        public void Get_Plant_3_Sat(bool data) { Settings.Watering.Plant_3_Sat = data; }
        public void Get_Plant_3_Sun(bool data) { Settings.Watering.Plant_3_Sun = data; }

        public void Get_Plant_4_Mon(bool data) { Settings.Watering.Plant_4_Mon = data; }
        public void Get_Plant_4_Tue(bool data) { Settings.Watering.Plant_4_Tue = data; }
        public void Get_Plant_4_Wed(bool data) { Settings.Watering.Plant_4_Wed = data; }
        public void Get_Plant_4_Thu(bool data) { Settings.Watering.Plant_4_Thu = data; }
        public void Get_Plant_4_Fri(bool data) { Settings.Watering.Plant_4_Fri = data; }
        public void Get_Plant_4_Sat(bool data) { Settings.Watering.Plant_4_Sat = data; }
        public void Get_Plant_4_Sun(bool data) { Settings.Watering.Plant_4_Sun = data; }

        public void Get_Plant_5_Mon(bool data) { Settings.Watering.Plant_5_Mon = data; }
        public void Get_Plant_5_Tue(bool data) { Settings.Watering.Plant_5_Tue = data; }
        public void Get_Plant_5_Wed(bool data) { Settings.Watering.Plant_5_Wed = data; }
        public void Get_Plant_5_Thu(bool data) { Settings.Watering.Plant_5_Thu = data; }
        public void Get_Plant_5_Fri(bool data) { Settings.Watering.Plant_5_Fri = data; }
        public void Get_Plant_5_Sat(bool data) { Settings.Watering.Plant_5_Sat = data; }
        public void Get_Plant_5_Sun(bool data) { Settings.Watering.Plant_5_Sun = data; }
    }
}
