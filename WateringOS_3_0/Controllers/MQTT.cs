using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WateringOS_3_0.Models;
using WateringOS_3_0.Controllers;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace WateringOS_3_0
{
    public class MQTTController
    {
        private readonly string client_id = "aaa6b1f384e94ce0bc4049bd09a27443"; // unique client ID (WateringSystem)
        private readonly string sensor_id = "c770306170b1470f83571dad74b09016"; // Sensors

        public readonly static string  Flow1 = "fbf47f612a7948109f3ef3e584b6d942"; // Sensors : 
        public readonly static string  Flow2 = "10cafe34ecd644a39eb9c1c28e009529"; // Sensors : 
        public readonly static string  Flow3 = "a73382ef81d745009a394c9aa2a66cd7"; // Sensors : 
        public readonly static string Flow4 = "a04390c364de48a281fb17615192fa9b"; // Sensors :
        public readonly static string Flow5 = "03d998f1dabb43aea5ba1fb10e47d771"; // Sensors :
        public readonly static string Pump = "03b4da51ef6343ccb30351caa2d70d78"; // Sensors :
        public readonly static string Valve1 = "339d1fb8c4d0411a9f4190ff2bd4d622"; // Sensors :
        public readonly static string Valve2 = "0e1a2309339743fb85e6407da5952dad"; // Sensors :
        public readonly static string Valve3 = "3719faa8b7a94191af83aed2cc436ce3"; // Sensors :
        public readonly static string Valve4 = "35e368dd4c854072be54add1f6ecdbca"; // Sensors :
        public readonly static string Valve5 = "3904e3a2de544bc98d25aa184da280db"; // Sensors :
        public readonly static string Tank = "5c310ecd8ec449e0aeeab2035ba34001"; // Sensors :
        public readonly static string TankForce = "f0db0ce446be4abe8862d290e30a84fe"; // Sensors :
        public readonly static string Rain = "24cf4973b1cb4b28a635bbee375adfbc"; // Sensors :
        public readonly static string Ground = "fc6ff457c99b4328b336b49f6096f0b8"; // Sensors :
        public readonly static string Pressure = "53277e8cb74240518b731bc6f76fff84"; // Sensors :
        public readonly static string LevelRaw = "e7ce45985b56454ea957e26a18eecde4"; // Sensors :
        public readonly static string TempCPU = "3638a525e9214aa9b23cf71a5b7727fe"; // Sensors :
        public readonly static string TempAmb = "6ca451085c5a4208b63cf384d08accbb"; // Sensors :
        public readonly static string TempExp = "b2cfe6ad7b2c47eca69b1e981516ee7d"; // Sensors :
        public readonly static string PowerGood_5V = "e782657a1cdd48859f92f2850462d127"; // Sensors :
        public readonly static string PowerGood_12V = "55950578cfbc44a2a0d94a11e0718a78"; // Sensors :
        public readonly static string PowerGood_24V = "fc400cee0ef0456fbbc6422944d972d1"; // Sensors :
        public readonly static string PowerFail_5V = "61f93cda5c5947098ea060b181aee068"; // Sensors :
        public readonly static string PowerFail_12V = "a401eda7c4e941029b4dc28bd7b41850"; // Sensors :
        public readonly static string PowerFail_24V = "424a865e110a4c8a9e04fe6f581ca6ec"; // Sensors :
        public readonly static string WatchDog = "633ad0a4c7864b4c9b34710ad0e3824d"; // Sensors :
        public readonly static string TempDiff = "445564684d6b4c03a00ed00fce8abdfc"; // Sensors :

        private Dictionary<string, object> MQTTvalues = new Dictionary<string, object>()
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

        public void Initialize()
        {
            MQTTLog(LogType.Information, "Start initialisation", "The intialization of the MQTT communication class has started.");
        }

        public string CreateArguments(Dictionary<string, object> vList)
        {
            var vStr = string.Format("{0} {1} ", client_id, sensor_id);

            foreach (KeyValuePair<string, object> kvp in vList)
            {
                vStr = vStr + string.Format("{0} {1} ", kvp.Value.ToString(), kvp.Key);
            }

            return vStr;
        }

        public void Update(string Metric, object Value)
        {
            MQTTvalues[Metric] = Value;
        }

        public void Publish()
        {
            if (Globals.MQTT_IsBusy)
            {
                MQTTLog(LogType.Warning, "MQTT in busy state - publish skipped", "MQTT_IsBusy found true. The start of process mqtt_pub.Start() was supressed");
            }
            else
            {
                using (Process mqtt_pub = new Process())
                {
                    mqtt_pub.StartInfo.FileName = "/app/mqtt.py";
                    mqtt_pub.StartInfo.Arguments = CreateArguments(MQTTvalues);
                    mqtt_pub.StartInfo.UseShellExecute = false;
                    mqtt_pub.StartInfo.CreateNoWindow = true;
                    mqtt_pub.StartInfo.RedirectStandardError = true;
                    mqtt_pub.StartInfo.RedirectStandardOutput = true;
                    if (mqtt_pub.Start())
                    {
                        //Globals.MQTT_IsBusy = true;
                        //mqtt_pub.WaitForExit();
                        while (!mqtt_pub.HasExited)
                        {
                            Globals.MQTT_IsBusy = true;
                        }
                        Globals.MQTT_IsBusy = false;
                    }
                    else
                    {
                        MQTTLog(LogType.Error, "MQTT Publish - Failed to start process", "MQTT Publish process was throwing an exception. The process could not be started.");
                        Globals.MQTT_IsBusy = false;
                    }
                }
            }

        }

        private void MQTTLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_MQTTController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_MQTTController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_MQTTController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_MQTTController.LogError(vMessage);
                    break;
                case LogType.Fatal:
                    Parents.Logger_MQTTController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_MQTTController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }

            LoggingEventArgs args = new LoggingEventArgs();
            args.TimeStamp = DateTime.Now;
            args.Instance = "[MQTT]";
            args.Type = vType;
            args.Message = vMessage;
            args.Detail = vDetail;
            OnMQTTLoggingEvent(args);
        }
        protected void OnMQTTLoggingEvent(LoggingEventArgs e)
        {
            EventHandler<LoggingEventArgs> handler = MQTTLogEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LoggingEventArgs> MQTTLogEvent;
    }
}
