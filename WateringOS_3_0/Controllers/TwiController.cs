using System;
using System.Threading.Tasks;
using System.Device.I2c;
using WateringOS_3_0;
using WateringOS_3_0.Models;
using Microsoft.Extensions.Logging;

/// TWI Class for RaspberryPi 3
/// to be used with WateringOS >3.0
/// 
/// (C) Michael Kollmeyer, 03-May-2020
/// 

namespace WateringOS_3_0
{
    public class TWIController
    {
        private I2cDevice TWI_TempCPU;      // 0x48
        private I2cDevice TWI_TempAmbient;  // 0x4F
        private I2cDevice TWI_TempExposed;  // 0x4B
        private I2cConnectionSettings settings1;
        private I2cConnectionSettings settings2;
        private I2cConnectionSettings settings3;
        public short AmbientTemp { get; private set; }
        public short ExposedTemp { get; private set; }
        public short CPUTemp { get; private set; }

        public async void InitTWIAsync()
        {
            TwiLog(LogType.Information, "Start initialisation", "The intialization of the TWI communication class has started.");
                try
                {

                    // Initialize CPU Temperature Sensor (DS1621)
                    try
                    {
                        TwiLog(LogType.Information, "0x48: Loading slave", "Loading slave settings for 0x48 (CPU temperature sensor) and starting interface");
                        this.settings1 = new I2cConnectionSettings(1, 0x48); // CPU Temperature
                        this.TWI_TempCPU = I2cDevice.Create(this.settings1);
                        await Task.Delay(100);

                        TwiLog(LogType.Information, "0x48: Setting configuration register", "Setting the configuration register to continues measuring");
                        var vACh = new byte[] { 0xAC, 0x00 };    // Access config set 0x00 (continues meas)
                        this.TWI_TempCPU.Write(vACh);
                        await Task.Delay(100);

                        TwiLog(LogType.Information, "0x48: Start temperature conversion", "Sending command to start continues temperature conversion");
                        var vEEh = new byte[] { 0xEE };    // Send 0xEE for start conversion
                        this.TWI_TempCPU.Write(vEEh);
                        await Task.Delay(100);

                    }
                    catch (Exception e)
                    {
                        TwiLog(LogType.Error, "0x48: Error initializing CPU temperature sensor", e.Message);
                    }

                    // Initialize Ambient Temperature Sensor (LM75)
                    try
                    {
                        TwiLog(LogType.Information, "0x4F: Loading slave", "Loading slave settings for 0x4F (ambient temperature sensor) and starting interface");
                        this.settings2 = new I2cConnectionSettings(1, 0x4F);   // Ambient Temperature
                        this.TWI_TempAmbient = I2cDevice.Create(settings2);
                        await Task.Delay(100);
                        
                    }
                    catch (Exception e)
                    {
                        TwiLog(LogType.Error, "0x4F: Error initializing ambient temperature sensor", e.Message);
                    }

                    // Initialize Exposed Temperature Sensor (LM75)
                    try
                    {
                        TwiLog(LogType.Information, "0x4B: Loading slave", "Loading slave settings for 0x4B (exposed temperature sensor) and starting interface");
                        this.settings3 = new I2cConnectionSettings(1, 0x4B);   // Ambient Temperature
                        this.TWI_TempExposed = I2cDevice.Create(settings3);
                        await Task.Delay(100);
                    }
                    catch (Exception e)
                    {
                        TwiLog(LogType.Error, "0x4B: Error initializing exposed temperature sensor", e.Message);
                    }
                }
                catch (Exception e)
                {
                    TwiLog(LogType.Error, "TWI Class Error Initializing", e.Message);
                }
        }
        public void TWI_Read()
        {
            AmbientTemp = ReadAmbientTemp();
            using (Task t_wait = Task.Run(async delegate { await Task.Delay(50); }))
            {
                t_wait.Wait();
            }
            ExposedTemp = ReadExposedTemp();
            using (Task t_wait = Task.Run(async delegate { await Task.Delay(50); }))
            {
                t_wait.Wait();
            }
            CPUTemp     = ReadCPUTemp();
        }
        private short ReadCPUTemp()
        {

            try
            {
                var vASr = new byte[] { 0xAA };    // 0xAA = DS1621 read temp
                var vASa = new byte[2];
                this.TWI_TempCPU.Write(vASr);
                this.TWI_TempCPU.Read(vASa);
                //var vNeg = 1;
                //if ((vASa[0] & 128) == 1) { vNeg = -1; }
                //int tTcpu = (vASa[0] & 127) * vNeg;
                sbyte tTcpu = (sbyte)vASa[0];
                return tTcpu;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading CPU temperature", e.Message);
                return -50;
            }
        }
        private short ReadAmbientTemp()
        {
            try
            {
                var vASr = new byte[] { 0x00 };    // 0x00 = LM75 Read Temp
                var vASa = new byte[2];
                this.TWI_TempAmbient.Write(vASr);
                this.TWI_TempAmbient.Read(vASa);
                // var vNeg = 1;
                // if ((vASa[0] & 128) == 1) { vNeg = -1; }
                // int tTamb = (vASa[0] & 127) * vNeg;
                sbyte tTair= (sbyte)vASa[0];
                return tTair;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading ambient temperature", e.Message);
                return -50;
            }
        }
        private short ReadExposedTemp()
        {
            try
            {
                // var vASr = new byte[] { 0x00 };    // 0x00 = LM75 Read Temp
                // var vASa = new byte[2];
                // this.TWI_TempExposed.Write(vASr);
                // this.TWI_TempExposed.Read(vASa);
                // var vNeg = 1;
                // if ((vASa[0] & 128) == 1) { vNeg = -1; }
                // return ((vASa[0] & 127) * vNeg);
                sbyte tTexp = (sbyte)(-18);
                return tTexp;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading exposed temperature", e.Message);
                return -50;
            }
        }
        private void TwiLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_TWIController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_TWIController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_TWIController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_TWIController.LogError(vMessage);
                    break;
                case LogType.Fatal:
                    Parents.Logger_TWIController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_TWIController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }

            LoggingEventArgs args = new LoggingEventArgs();
            args.TimeStamp = DateTime.Now;
            args.Instance = "[TWI]";
            args.Type = vType;
            args.Message = vMessage;
            args.Detail = vDetail;
            OnTwiLoggingEvent(args);
        }
        protected void OnTwiLoggingEvent(LoggingEventArgs e)
        {
            EventHandler<LoggingEventArgs> handler = TwiLogEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LoggingEventArgs> TwiLogEvent;

    }
}
