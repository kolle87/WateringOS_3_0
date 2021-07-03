using Microsoft.Extensions.Logging;
using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using WateringOS_3_0.Models;

namespace WateringOS_3_0
{
    public static class InfluxController
    {
        private static InfluxDBClient influxDBClient { get; set; }

        public static void OpenConnection()
        {
            influxDBClient = InfluxDBClientFactory.Create("http://localhost:8086", Globals.InfluxToken);
        }

        public static void CloseConnection()
        {
            influxDBClient.Dispose();
        }
        
        public static void AddInfluxJournal(DateTime vTimeStamp, string vApp, string vType, string vMessage, string vDetails)
        {
            try
            {
                using (var writeApi = influxDBClient.GetWriteApi())
                {
                    var point = PointData.Measurement("Journal")
                        .Tag("Type", vType)
                        .Tag("App", vApp)
                        .Field("Message", vMessage)
                        .Timestamp(vTimeStamp, WritePrecision.Ms);
                
                    writeApi.WritePoint("WateringSystem", "mkollmeyer", point);
                }
            }
            catch
            {
                Console.WriteLine($">>> Error adding Influx-Journal entry");
            }
        }
        
        public static void AddInfluxMeasurement(PointData vMeasurement)
        {
            try
            {
                using (var writeApi = influxDBClient.GetWriteApi())
                {
                    writeApi.WritePoint("WateringSystem", "mkollmeyer", vMeasurement);
                }
            }
            catch
            {
                Console.WriteLine($">>> Error adding Influx-Measurement entry");
            }
        }
    }
}