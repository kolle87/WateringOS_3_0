using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WateringOS_3_0.Models;
using WateringOS_3_0.Controllers;

namespace WateringOS_3_0.Controllers
{
    public class MQTT
    {
        private static readonly string client_id = "aaa6b1f384e94ce0bc4049bd09a27443"; // unique client ID (WateringSystem)
        private static readonly string sensor_id = "c770306170b1470f83571dad74b09016"; // Sensors

        public static readonly string  Flow1 = "fbf47f612a7948109f3ef3e584b6d942"; // Sensors : 
        public static readonly string  Flow2 = "10cafe34ecd644a39eb9c1c28e009529"; // Sensors : 
        public static readonly string  Flow3 = "a73382ef81d745009a394c9aa2a66cd7"; // Sensors : 
        public static readonly string Flow4 = "a04390c364de48a281fb17615192fa9b"; // Sensors :
        public static readonly string Flow5 = "03d998f1dabb43aea5ba1fb10e47d771"; // Sensors :
        public static readonly string Pump = "03b4da51ef6343ccb30351caa2d70d78"; // Sensors :
        public static readonly string Valve1 = "339d1fb8c4d0411a9f4190ff2bd4d622"; // Sensors :
        public static readonly string Valve2 = "0e1a2309339743fb85e6407da5952dad"; // Sensors :
        public static readonly string Valve3 = "3719faa8b7a94191af83aed2cc436ce3"; // Sensors :
        public static readonly string Valve4 = "35e368dd4c854072be54add1f6ecdbca"; // Sensors :
        public static readonly string Valve5 = "3904e3a2de544bc98d25aa184da280db"; // Sensors :
        public static readonly string Tank = "5c310ecd8ec449e0aeeab2035ba34001"; // Sensors :
        public static readonly string TankForce = "f0db0ce446be4abe8862d290e30a84fe"; // Sensors :
        public static readonly string Rain = "24cf4973b1cb4b28a635bbee375adfbc"; // Sensors :
        public static readonly string Ground = "fc6ff457c99b4328b336b49f6096f0b8"; // Sensors :
        public static readonly string Pressure = "53277e8cb74240518b731bc6f76fff84"; // Sensors :
        public static readonly string LevelRaw = "e7ce45985b56454ea957e26a18eecde4"; // Sensors :
        public static readonly string TempCPU = "3638a525e9214aa9b23cf71a5b7727fe"; // Sensors :
        public static readonly string TempAmb = "6ca451085c5a4208b63cf384d08accbb"; // Sensors :
        public static readonly string TempExp = "b2cfe6ad7b2c47eca69b1e981516ee7d"; // Sensors :
        public static readonly string PowerGood_5V = "e782657a1cdd48859f92f2850462d127"; // Sensors :
        public static readonly string PowerGood_12V = "55950578cfbc44a2a0d94a11e0718a78"; // Sensors :
        public static readonly string PowerGood_24V = "fc400cee0ef0456fbbc6422944d972d1"; // Sensors :
        public static readonly string PowerFail_5V = "61f93cda5c5947098ea060b181aee068"; // Sensors :
        public static readonly string PowerFail_12V = "a401eda7c4e941029b4dc28bd7b41850"; // Sensors :
        public static readonly string PowerFail_24V = "424a865e110a4c8a9e04fe6f581ca6ec"; // Sensors :
        public static readonly string WatchDog = "633ad0a4c7864b4c9b34710ad0e3824d"; // Sensors :
        public static readonly string TempDiff = "445564684d6b4c03a00ed00fce8abdfc"; // Sensors :

        private static Dictionary<string, object> MQTTvalues = new Dictionary<string, object>()
        {
        {Flow1, "0"},
        {Flow2, "0"},
        {Flow3, "0"},
        {Flow4, "0"},
        {Flow5, "0"},
        {Pump, "0"},
        {Valve1, "0"},
        {Valve2, "0"},
        {Valve3, "0"},
        {Valve4, "0"},
        {Valve5, "0"},
        {Tank, "0"},
        {TankForce, "0"},
        {Rain, "0"},
        {Ground, "0"},
        {Pressure, "0"},
        {LevelRaw, "0"},
        {TempCPU, "0"},
        {TempAmb, "0"},
        {TempExp, "0"},
        {PowerGood_5V, "0"},
        {PowerGood_12V, "0"},
        {PowerGood_24V, "0"},
        {PowerFail_5V, "0"},
        {PowerFail_12V, "0"},
        {PowerFail_24V, "0"},
        {WatchDog, "0"},
        {TempDiff, "0"}
    };

        public static string CreateArguments(Dictionary<string, object> vList)
        {
            var vStr = string.Format("{0} {1} ", client_id, sensor_id);

            foreach (KeyValuePair<string, object> kvp in vList)
            {
                vStr = vStr + string.Format("{0} {1} ", kvp.Value.ToString(), kvp.Key);
            }

            return vStr;
        }

        public static void Update(string Metric, object Value)
        {
            MQTTvalues[Metric] = Value;
        }

        public static string Publish()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/app/mqtt.py";

            start.Arguments = MQTT.CreateArguments(MQTT.MQTTvalues);

            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardError = true;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string vErr = process.StandardError.ReadToEnd();
                    Console.WriteLine(reader.ReadToEnd());
                    return(vErr);
                }
            }
        }
    }
}
