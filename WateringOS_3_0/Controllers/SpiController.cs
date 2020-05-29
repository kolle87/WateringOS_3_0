using System;
using System.Diagnostics;
using System.Device.Spi;
using Iot.Device;
using WateringOS_3_0.Models;
using WateringOS_3_0.Controllers;
using Microsoft.Extensions.Logging;

/// SPI Class for RaspberryPi 3
/// to be used with WateringOS >3.0
/// 
/// (C) Michael Kollmeyer, 02-May-2020
/// 

namespace WateringOS_3_0
{
    public class SPIController
    {
        // Private Declarations
        private SpiDevice AtMega32A;
        private SpiConnectionSettings AtMega_Settings;

        // Public Declarations
        public byte   Flow1 { get; private set; }
        public byte   Flow2 { get; private set; }
        public byte   Flow3 { get; private set; }
        public byte   Flow4 { get; private set; }
        public byte   Flow5 { get; private set; }
        public byte   Rain { get; private set; }
        public byte   Level { get; private set; }
        public byte   LevelRaw { get; private set; }
        public double Pressure { get; private set; }
        public byte   Ground { get; private set; }
        public int    MinLevel { get; set; }
        public int    MaxLevel { get; set; }

        public void InitSPI()
        {
            SpiLog(LogType.Information, "Start initialization", "The intialization of the SPI communication class has started.");

            try
            {
                this.AtMega_Settings = new SpiConnectionSettings(0, 0); // (Bus_ID, CS_ID)
                this.AtMega_Settings.ClockFrequency = 10000;  // 10kHz            
                this.AtMega_Settings.Mode = SpiMode.Mode3;    // Mode3: CPOL = 1, CPHA = 1
                this.AtMega32A = SpiDevice.Create(AtMega_Settings);
            }
            catch (Exception e)
            {
                SpiLog(LogType.Error, "Class Erro SPI Initializing", e.Message);
            }
        }
        public bool SPI_Read()
        {
            byte[] ReadBuf = new byte[13];
            byte[] ReqData = new byte[] { 0x55, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0xFF };
            try
            {

                this.AtMega32A.TransferFullDuplex(ReqData, ReadBuf);
                byte vCRC = CRC8.ComputeChecksum(ReadBuf[2], ReadBuf[3], ReadBuf[4], ReadBuf[5], ReadBuf[6], ReadBuf[7], ReadBuf[8], ReadBuf[9], ReadBuf[10]);

                if (ReadBuf[12] == vCRC)
                {
                    this.Flow1 = ReadBuf[2];
                    this.Flow2 = ReadBuf[3];
                    this.Flow3 = ReadBuf[4];
                    this.Flow4 = ReadBuf[5];
                    this.Flow5 = ReadBuf[6];
                    this.Rain = ReadBuf[7];
                    this.LevelRaw = ReadBuf[8];
                    this.Pressure = ReadBuf[9] * 0.049;
                    this.Ground = ReadBuf[10];

                    try   { this.Level = Convert.ToByte(Math.Round(((double)(ReadBuf[8] - this.MinLevel) / (this.MaxLevel - this.MinLevel)) * 100.0)); }
                    catch { this.Level = ReadBuf[8]; }

                    return true;
                }
                else
                {
                    SpiLog(LogType.Error, "CRC failure", "The calculated CRC was not corresponding to the received one.");
                    return false;
                }

            }
            catch (Exception e)
            {
                SpiLog(LogType.Error, "Error reading SPI data", e.Message);
                return false;
            }
        }
        public bool SPI_Reset()
        {
            byte[] SendCmd = new byte[3];
            byte[] AckData = new byte[3];

            try
            {
                SendCmd[0] = 0x11;
                AtMega32A.TransferFullDuplex(SendCmd, AckData);
                if (AckData[2] == 0x06)
                {
                    return true;
                }
                else
                {
                    SpiLog(LogType.Error, "Not received acknowledge", "The command was not acknoledged by the slave.");
                    return false;
                }
            }
            catch (Exception e)
            {
                SpiLog(LogType.Error, "Error sending Atmega reset command", e.Message);
                return false;
            }
        }
        private void SpiLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_SPIController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_SPIController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_SPIController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_SPIController.LogError(vMessage + ": " + vDetail);
                    break;
                case LogType.Fatal:
                    Parents.Logger_SPIController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_SPIController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }

            LoggingEventArgs args = new LoggingEventArgs();
            args.TimeStamp = DateTime.Now;
            args.Instance = "[SPI]";
            args.Type = vType;
            args.Message = vMessage;
            args.Detail = vDetail;
            OnSpiLoggingEvent(args);
        }
        protected void OnSpiLoggingEvent(LoggingEventArgs e)
        {
            EventHandler<LoggingEventArgs> handler = SpiLogEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LoggingEventArgs> SpiLogEvent;

    }
}