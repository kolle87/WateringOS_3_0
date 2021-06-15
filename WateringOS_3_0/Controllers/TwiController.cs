using System;
using System.Threading.Tasks;
using System.Device.I2c;
using WateringOS_3_0;
using WateringOS_3_0.Models;
using Microsoft.Extensions.Logging;

/* DISCLAIMER

Watering OS - (C) Michael Kollmeyer 2020  
  
This file is part of WateringOS.

    WateringOS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WateringOS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WateringOS.  If not, see<https://www.gnu.org/licenses/>.

*/

namespace WateringOS_3_0
{
    public class TWIController
    {
        private I2cDevice TWI_TempCPU;      // 0x48
        private I2cDevice TWI_TempAmbient;  // 0x4F
        private I2cDevice TWI_TempExposed;  // 0x4B
        private I2cDevice TWI_Ground;   // 0x28
        private I2cConnectionSettings settings1;
        private I2cConnectionSettings settings2;
        private I2cConnectionSettings settings3;
        private I2cConnectionSettings settings4;
        private bool IsBusy = false;
        public short AmbientTemp { get; private set; }
        public short ExposedTemp { get; private set; }
        public short CPUTemp { get; private set; }
        public int Ground { get; private set; }

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
                    this.settings3 = new I2cConnectionSettings(1, 0x4B);   // Exposed Temperature
                    this.TWI_TempExposed = I2cDevice.Create(settings3);
                    await Task.Delay(100);
                }
                catch (Exception e)
                {
                    TwiLog(LogType.Error, "0x4B: Error initializing exposed temperature sensor", e.Message);
                }

                // Initialize Ground Sensor (Arduino Nano)
                try
                {
                    TwiLog(LogType.Information, "0x56: Loading slave", "Loading slave settings for 0x56 (ground sensor) and starting interface");
                    this.settings4 = new I2cConnectionSettings(1, 0x56);   // Ground sensor
                    this.TWI_Ground = I2cDevice.Create(settings4);
                    await Task.Delay(100);
                }
                catch (Exception e)
                {
                    TwiLog(LogType.Error, "0x56: Error initializing ground sensor", e.Message);
                }
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "TWI Class Error Initializing", e.Message);
            }
        }
        public void TWI_Read()
        {
            if (!this.IsBusy) { AmbientTemp = ReadAmbientTemp(); }
            
            using (Task t_wait = Task.Run(async delegate { await Task.Delay(100); }))
            {
                t_wait.Wait();
            }
            if (!this.IsBusy) { ExposedTemp = ReadExposedTemp(); }            
            using (Task t_wait = Task.Run(async delegate { await Task.Delay(100); }))
            {
                t_wait.Wait();
            }
            /*
            if (!this.IsBusy) { Ground = ReadGround(); }            
            using (Task t_wait = Task.Run(async delegate { await Task.Delay(100); }))
            {
                t_wait.Wait();
            }*/
            if (!this.IsBusy) { CPUTemp = ReadCPUTemp(); }
        }

        private short ReadCPUTemp()
        {
            try
            {
                this.IsBusy = true;
                /*
                    var vASr = new byte[] { 0xAA };    // 0xAA = DS1621 read temp
                    var vASa = new byte[2];
                    this.TWI_TempCPU.Write(vASr);
                    this.TWI_TempCPU.Read(vASa);
                    //var vNeg = 1;
                    //if ((vASa[0] & 128) == 1) { vNeg = -1; }
                    //int tTcpu = (vASa[0] & 127) * vNeg;
                    sbyte tTcpu = (sbyte)vASa[0];
                    return tTcpu;
                */
                this.IsBusy = false;
                return 50;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading CPU temperature", e.Message);
                this.IsBusy = false;
                return -50;
            }
        }
        private short ReadAmbientTemp()
        {
            try
            {
                this.IsBusy = true;
                var vASr = new byte[] { 0x00 };    // 0x00 = LM75 Read Temp
                var vASa = new byte[2];
                this.TWI_TempAmbient.Write(vASr);
                this.TWI_TempAmbient.Read(vASa);
                // var vNeg = 1;
                // if ((vASa[0] & 128) == 1) { vNeg = -1; }
                // int tTamb = (vASa[0] & 127) * vNeg;
                sbyte tTair = (sbyte)vASa[0];
                this.IsBusy = false;
                return tTair;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading ambient temperature", e.Message);
                this.IsBusy = false;
                return -50;
            }
        }
        private short ReadExposedTemp()
        {
            try
            {
                this.IsBusy = true;
                var vASr = new byte[] { 0x00 };    // 0x00 = LM75 Read Temp
                var vASa = new byte[2];
                this.TWI_TempExposed.Write(vASr);
                this.TWI_TempExposed.Read(vASa);
                // var vNeg = 1;
                // if ((vASa[0] & 128) == 1) { vNeg = -1; }
                // return ((vASa[0] & 127) * vNeg);
                sbyte tTexp = (sbyte)(vASa[0]);
                this.IsBusy = false;
                return tTexp;
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading exposed temperature", e.Message);
                this.IsBusy = false;
                return -50;
            }
        }

        public int ReadGround()
        {
            try
            {
                if (!this.IsBusy)
                {
                    this.IsBusy = true;
                    //var vASr = new byte[] { };    
                    var vASa = new byte[2];
                    //this.TWI_TankWeight.Write(vASr);
                    this.TWI_Ground.Read(vASa);
                    TwiLog(LogType.Information, ("ReadGround() [0] "+ vASa[0] +"  [1] " + vASa[1]), "ReadGround() - TWI busy");
                    int tGround = vASa[0] * 256;
                    this.IsBusy = false;
                    return tGround;
                }
                else
                {
                    TwiLog(LogType.Warning, "ReadGround() - TWI busy", "ReadGround() - TWI busy");
                    this.IsBusy = false;
                    return 10;
                }
            }
            catch (Exception e)
            {
                TwiLog(LogType.Error, "Error reading ground sensor", e.Message);
                this.IsBusy = false;
                return 10;
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
