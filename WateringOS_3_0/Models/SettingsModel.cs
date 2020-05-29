namespace WateringOS_3_0.Models
{
    public class cSystemSettings
    {
        // System Settings
        public int Task_cycle { get; set; } = 1000;         // [ ms ] normal task cycle (reset watchdog / check watering trigger time)
        public int Save_cycle { get; set; } = 20;           // [ min] Interval to write logged data in a file
        public int FastLog { get; set; } = 200;             // [ ms ] task cycle during watering
        public int Log_Enviroment { get; set; } = 20;       // [ min] Logging interval of enviromental data
        public int Log_Power { get; set; } = 1000;          // [ ms ] Logging interval of power supply state  >>> also to be logged at change <<<
        public int Log_Level { get; set; } = 20;            // [ min] Logging interval of tank level          >>> also to be logged at change <<<

        public int DLY_Auto_LogOff { get; set; } = 1;       // [ min] time for automatic log off >>> Requires Server Restart <<<

        public int Tank_Min { get; set; } = 123;            // [ int] calibration tank 0%
        public int Tank_Max { get; set; } = 198;            // [ int] calibration tank 100%

        public int Wat_min_tank { get; set; } = 3;          // [  % ] minimum tank level for activating watering
        public int Wat_min_vol { get; set; } = 50;          // [ ml ] minimum volume to water
        public int Wat_min_time { get; set; } = 15;         // [ sec] minimum time watering is active
        public int Wat_max_vol { get; set; } = 2550;        // [ ml ] maximum watering time
        public int Wat_max_time { get; set; } = 100;        // [ sec] maximum watering volume

        public int ALM_TankLevel { get; set; } = 5;         // [  % ] Low Level alarm for water tank
        public int ALM_MinPress { get; set; } = 1200;       // [mbar] minimum pressure during watering
        public int ALM_MaxPress { get; set; } = 6000;       // [mbar] maximum pressure during watering
        public int ALM_NoPumpDelay { get; set; } = 1000;    // [ ms ] Alarm delay between command and feedback
        public int ALM_NoValveDelay { get; set; } = 1000;   // [ ms ] Alarm delay between command and feedback
        public int ALM_PressOffDelay { get; set; } = 3000;  // [ ms ] Time pressure should drop after pump stopped
        public int ALM_PressOffValue { get; set; } = 300;   // [mbar] Pressure alarm threshold after pump stopped
        public int ALM_WarnTempCPU { get; set; } = 58;      // [°C] Temperature to raise a warning (60° CPU throttling)
        public int ALM_MaxTempCPU { get; set; } = 79;       // [°C] Temperature to raise a critical alarm (80° max operating temp)

        public int DLY_ValveOpen { get; set; } = 2000;      // [ ms ] time between valve opening and pump start
        public int DLY_PumpStop { get; set; } = 5000;       // [ ms ] time between pump stop and valve close
    }
    public class cWateringSettings
    { 
        // Watering Settings
        public string Plant_1_Name { get; set; }
        public string Plant_2_Name { get; set; }
        public string Plant_3_Name { get; set; }
        public string Plant_4_Name { get; set; }
        public string Plant_5_Name { get; set; }

        public int TimeMorning { get; set; }
        public int TimeNoon { get; set; }
        public int TimeEvening { get; set; }

        public bool Morning_1 { get; set; } = false;
        public bool Morning_2 { get; set; } = false;
        public bool Morning_3 { get; set; } = false;
        public bool Morning_4 { get; set; } = false;
        public bool Morning_5 { get; set; } = false;
        public bool Noon_1 { get; set; } = false;
        public bool Noon_2 { get; set; } = false;
        public bool Noon_3 { get; set; } = false;
        public bool Noon_4 { get; set; } = false;
        public bool Noon_5 { get; set; } = false;
        public bool Evening_1 { get; set; } = false;
        public bool Evening_2 { get; set; } = false;
        public bool Evening_3 { get; set; } = false;
        public bool Evening_4 { get; set; } = false;
        public bool Evening_5 { get; set; } = false;

        public int Volume_1 { get; set; } = 600;
        public int Volume_2 { get; set; } = 600;
        public int Volume_3 { get; set; } = 600;
        public int Volume_4 { get; set; } = 600;
        public int Volume_5 { get; set; } = 600;

        public int RAF_1 { get; set; } = 80;
        public int RAF_2 { get; set; } = 80;
        public int RAF_3 { get; set; } = 80;
        public int RAF_4 { get; set; } = 80;
        public int RAF_5 { get; set; } = 80;

        public int GAF_1 { get; set; } = 80;
        public int GAF_2 { get; set; } = 80;
        public int GAF_3 { get; set; } = 80;
        public int GAF_4 { get; set; } = 80;
        public int GAF_5 { get; set; } = 80;

        public bool Plant_1_Mon { get; set; } = false;
        public bool Plant_1_Tue { get; set; } = false;
        public bool Plant_1_Wed { get; set; } = false;
        public bool Plant_1_Thu { get; set; } = false;
        public bool Plant_1_Fri { get; set; } = false;
        public bool Plant_1_Sat { get; set; } = false;
        public bool Plant_1_Sun { get; set; } = false;

        public bool Plant_2_Mon { get; set; } = false;
        public bool Plant_2_Tue { get; set; } = false;
        public bool Plant_2_Wed { get; set; } = false;
        public bool Plant_2_Thu { get; set; } = false;
        public bool Plant_2_Fri { get; set; } = false;
        public bool Plant_2_Sat { get; set; } = false;
        public bool Plant_2_Sun { get; set; } = false;

        public bool Plant_3_Mon { get; set; } = false;
        public bool Plant_3_Tue { get; set; } = false;
        public bool Plant_3_Wed { get; set; } = false;
        public bool Plant_3_Thu { get; set; } = false;
        public bool Plant_3_Fri { get; set; } = false;
        public bool Plant_3_Sat { get; set; } = false;
        public bool Plant_3_Sun { get; set; } = false;

        public bool Plant_4_Mon { get; set; } = false;
        public bool Plant_4_Tue { get; set; } = false;
        public bool Plant_4_Wed { get; set; } = false;
        public bool Plant_4_Thu { get; set; } = false;
        public bool Plant_4_Fri { get; set; } = false;
        public bool Plant_4_Sat { get; set; } = false;
        public bool Plant_4_Sun { get; set; } = false;

        public bool Plant_5_Mon { get; set; } = false;
        public bool Plant_5_Tue { get; set; } = false;
        public bool Plant_5_Wed { get; set; } = false;
        public bool Plant_5_Thu { get; set; } = false;
        public bool Plant_5_Fri { get; set; } = false;
        public bool Plant_5_Sat { get; set; } = false;
        public bool Plant_5_Sun { get; set; } = false;
    }
    public class Settings
    {
        public static cWateringSettings Watering = new cWateringSettings();
        public static cSystemSettings     System = new cSystemSettings();
    }
}