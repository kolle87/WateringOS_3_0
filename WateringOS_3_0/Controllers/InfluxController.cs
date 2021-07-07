using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WateringOS_3_0.Models;

namespace WateringOS_3_0
{
    public static class InfluxController
    {
        private static HttpClient influxDBClient { get; set; }
        private static int TaskCount { get; set; }
        private static readonly int _HighTaskCount = 10;

        public static void OpenConnection()
        {
            influxDBClient = new HttpClient();
            influxDBClient.BaseAddress = new Uri("http://localhost:8086");
            TaskCount = 0;
            Console.WriteLine($"InfluxDB >>> Connection opened");
        }

        public static void CloseConnection()
        {
            influxDBClient.Dispose();
            Console.WriteLine($"InfluxDB >>> Connection closed");
        }
        
        public static void AddInfluxJournal(DateTime vTimeStamp, string vApp, string vType, string vMessage, string vDetails)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(vTimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();
                    
                    string journal = "Journal" +
                                     ",App="+vApp +
                                     ",Type="+vType +
                                     " " +
                                     "Message=\""+vMessage+"\"" +
                                     " " +
                                     vUnixTimestamp.ToString();
                
                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(journal, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                   Console.WriteLine($">>> Error adding Influx-Journal entry ({e.Message})");
                   TaskCount--;
               }
            });
            
        }
        
        public static void WriteInfluxPower(bool vPG5, bool vPG12, bool vPG24, bool vPF5, bool vPF12, bool vPF24, bool vWD)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "Power" +
                                     " " +
                                     "PowerGood_5V=" + vPG5 +
                                     ",PowerGood_12V=" + vPG12 +
                                     ",PowerGood_24V=" + vPG24 +
                                     ",PowerFail_5V=" + vPF5 +
                                     ",PowerFail_12V=" + vPF12 +
                                     ",PowerFail_24V=" + vPF24 +
                                     ",WatchdogPrealarm=" + vWD +
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                    Console.WriteLine($">>> Error adding Influx-PowerMeasurement entry ({e.Message})");
                    TaskCount--;
               }
            });
            
        }
        public static void WriteInfluxRain(byte vRain)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "Rainfall" +
                                     " " +
                                     "Rain=" + vRain +
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                    Console.WriteLine($">>> Error adding Influx-RainMeasurement entry ({e.Message})");
                    TaskCount--;
               }
            });
            
        }
        public static void WriteInfluxTemp(int vTempCPU, int vTempAmb, int vTempExp)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "Temperature" +
                                     " " +
                                     "TempAmb=" + vTempAmb +
                                     ",TempExp=" + vTempExp +
                                     ",TempCPU=" + vTempCPU +
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                    Console.WriteLine($">>> Error adding Influx-TemperatureMeasurement entry ({e.Message})");
                    TaskCount--;
               }
            });
            
        }
        public static void WriteInfluxGround(int vGround)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "Moisture" +
                                     " " +
                                     "Ground=" + vGround +
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                    Console.WriteLine($">>> Error adding Influx-GroundMeasurement entry ({e.Message})");
                    TaskCount--;
               }
            });
            
        }
        public static void WriteInfluxWatering(int vFlow1, int vFlow2, int vFlow3, int vFlow4, int vFlow5, double vPressure, bool vPump, bool vValve1, bool vValve2, bool vValve3, bool vValve4, bool vValve5)
        {
            Task.Run(async delegate
            {
               try
               { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "Activation" +
                                     " " +
                                     "Flow1=" + vFlow1 +
                                     ",Flow2=" + vFlow2 +
                                     ",Flow3=" + vFlow3 +
                                     ",Flow4=" + vFlow4 +
                                     ",Flow5=" + vFlow5 +
                                     ",Pressure=" + vPressure +
                                     ",Pump=" + vPump +
                                     ",Valve1=" + vValve1 +
                                     ",Valve2=" + vValve2 +
                                     ",Valve3=" + vValve3 +
                                     ",Valve4=" + vValve4 +
                                     ",Valve5=" + vValve5 +
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
               }
               catch (Exception e)
               {
                    Console.WriteLine($">>> Error adding Influx-ActiveMeasurement entry ({e.Message})");
                    TaskCount--;
               }
            });
            
        }
        public static void WriteInfluxTank(byte vLevel, byte vRaw)
        {
            Task.Run(async delegate
            {
                try
                { 
                    TaskCount++;
                    if (TaskCount>_HighTaskCount) Console.WriteLine($"InfluxDB >>> High TaskCount: {TaskCount}");
                    
                    DateTime convertedDate = DateTime.SpecifyKind(LogLists.RecentEntries.TimeStamp.AddHours(-2), DateTimeKind.Utc);
                    var vUnixTimestamp = new DateTimeOffset(convertedDate).ToUnixTimeMilliseconds();

                    string measure = "TankLevel" +
                                     " " +
                                     "Tank=" + vLevel +
                                     ",Raw=" + vRaw+
                                     " " +
                                     vUnixTimestamp.ToString();

                    var httpResponse = await influxDBClient.PostAsync("/write?db=watering&precision=ms", new StringContent(measure, Encoding.UTF8, "application/octet-stream"));
                    
                    if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"InfluxDB >>> {httpResponse.StatusCode}");
                        string resultContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resultContent);
                    }

                    TaskCount--;
                }
                catch (Exception e)
                {
                    Console.WriteLine($">>> Error adding Influx-TankMeasurement entry ({e.Message})");
                    TaskCount--;
                }
            });
            
        }
    }
}