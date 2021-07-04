using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using WateringOS_3_0.Models;

namespace WateringOS_3_0
{
    public static class InfluxController
    {
        private static LineProtocolClient influxDBClient { get; set; }

        public static void OpenConnection()
        {
            influxDBClient = new LineProtocolClient(new Uri("http://localhost:8086"), "watering","mkollmeyer","3507866");
        }

        public static void CloseConnection()
        {
            
        }
        
        public static void AddInfluxJournal(DateTime vTimeStamp, string vApp, string vType, string vMessage, string vDetails)
        {
            try
            {
                Task.Run(async delegate
                {
                    var JournalEntry = new LineProtocolPoint(
                        "Journal",
                        new Dictionary<string, object>
                        {
                            {"Message", vMessage},
                        },
                        new Dictionary<string, string>
                        {
                            {"Type", vType},
                            {"App", vApp}
                        },
                        vTimeStamp);

                    var payload = new LineProtocolPayload();
                    payload.Add(JournalEntry);
                    var influxResult = await influxDBClient.WriteAsync(payload);
                    if (!influxResult.Success)
                        Console.Error.WriteLine(influxResult.ErrorMessage);
                });
            }
            catch
            {
                Console.WriteLine($">>> Error adding Influx-Journal entry");
            }
        }
        
        public static void AddInfluxMeasurement(LineProtocolPoint vMeasurement)
        {
            try
            {
                Task.Run(async delegate
                {
                    var payload = new LineProtocolPayload();
                                    payload.Add(vMeasurement);
                    var influxResult = await influxDBClient.WriteAsync(payload);
                    if (!influxResult.Success)
                        Console.Error.WriteLine(influxResult.ErrorMessage);
                });
            }
            catch
            {
                Console.WriteLine($">>> Error adding Influx-Measurement entry");
            }
        }
    }
}