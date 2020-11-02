﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Timers;
using Microsoft.AspNetCore.Mvc;
using WateringOS_3_0.Models;
using WateringOS_3_0.Controllers;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;

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

namespace WateringOS_3_0.Controllers
{
    public class BackgroundTaskController : Controller
    {
        static int LogNumber = 0;
        static int LogCount = 0;

        private static void FastTask_Routine(object sender, ElapsedEventArgs e)
        {
            Globals.SpiServer.SPI_Read();
            Globals.TwiServer.TWI_Read();

            byte vRain = 0;
            byte vGround = 0;

            // Alarm handling --> jitter supression in main task
            int vCurrentTemp = DataProvisionController.GetTemperature();
            double vCurrentPress = Globals.SpiServer.Pressure;
            if ((vCurrentTemp >= Settings.System.ALM_WarnTempCPU) && (vCurrentTemp < Settings.System.ALM_MaxTempCPU) && (!Globals.ALM_WarnTempCPUactive))
            {
                SysLog(LogType.Warning, "High CPU Temperature", "The Temperature of the CPU reached the warning threshold of " + Settings.System.ALM_WarnTempCPU + "°C");
                Globals.ALM_WarnTempCPUactive = true;
            }

            if ((vCurrentTemp >= Settings.System.ALM_MaxTempCPU) && (!Globals.ALM_MaxTempCPUactive))
            {
                SysLog(LogType.Fatal, "Maximum CPU Operation Temperature", "The Temperature of the CPU reached the critical threshold of " + Settings.System.ALM_WarnTempCPU + "°C.\nA stable operation and damages to the hardware can not be excluded.\n\nThe system should be shut down and the failure source investigated.");
                Globals.ALM_MaxTempCPUactive = true;
            }

            if ((LogLists.RecentEntries.Tank <= Settings.System.ALM_TankLevel) && (!Globals.ALM_WarnTankLevelactive))
            {
                SysLog(LogType.Warning, "Tank Level LOW", "The water level of the tank reached the warning threshold of " + Settings.System.ALM_TankLevel + "%");
                Globals.ALM_WarnTankLevelactive = true;
            }

            if ((vCurrentPress >= Settings.System.ALM_MaxPress / 1000) && (!Globals.ALM_MaxPressactive))
            {
                SysLog(LogType.Error, "Pressure HIGH", "The pressure exceeded " + Settings.System.ALM_MaxPress / 1000 + "bar! The pump was stopped for safety reasons.");
                Globals.GpioServer.StopPump();
                Globals.ALM_MaxPressactive = true;
            }

            // enviromental variable translation
            switch (Math.Floor((double)(Globals.SpiServer.Rain / 100)))
            {
                case 1: vRain = 1; break;
                case 2: vRain = 2; break;
                default: vRain = 0; break;
            }

            switch (Math.Floor((double)(Globals.SpiServer.Ground / 100)))
            {
                case 1: vGround = 1; break;
                case 2: vGround = 2; break;
                default: vGround = 0; break;
            }

            // Pump protection
            if ((Globals.GpioServer.PumpActive) && (!Globals.GpioServer.Valve1Open) && (!Globals.GpioServer.Valve2Open) && (!Globals.GpioServer.Valve3Open) && (!Globals.GpioServer.Valve4Open) && (!Globals.GpioServer.Valve5Open))
            {
                Globals.GpioServer.StopPump();
                SysLog(LogType.Error, "Pump Interlock - valve closed", "The pump was stopped since all valves are closed!");
            }

            // Log Environment if different Rain or Ground value than before
            if (((LogLists.RecentEntries.Rain != vRain) || (LogLists.RecentEntries.Ground != vGround)) && (vRain != 0) && (vGround != 0))
            {
                try
                {
                    Globals.EnvLogger.Stop();
                    while (LogLists.EnvLog.Count > 65500) { LogLists.EnvLog.RemoveAt(0); }
                    LogLists.EnvLog.Add(new cEnvLog
                    {
                        TimeStamp = DateTime.Now,
                        Rain = vRain,
                        Ground = vGround,
                        TempAmb = LogLists.RecentEntries.TempAmb,
                        TempCPU = LogLists.RecentEntries.TempCPU,
                        TempExp = LogLists.RecentEntries.TempExp
                    });
                }
                finally { Globals.EnvLogger.Start(); }
            }

            // Log Level if smaller than before or 5% bigger
            //if ((Globals.SpiServer.Level < LogLists.RecentEntries.Tank)||(Globals.SpiServer.Level > LogLists.RecentEntries.Tank+Settings.System.Tank_RefillHyst))
            //{
            //    try
            //    {
            //        Globals.LevelLogger.Stop();
            //        if (LogLists.LevelLog.Count == 65500) { LogLists.LevelLog.RemoveAt(0); }
            //        LogLists.LevelLog.Add(new cLevelLog
            //        {
            //            TimeStamp = DateTime.Now,
            //            Tank = Globals.SpiServer.Level
            //        });
            //    } finally { Globals.LevelLogger.Start(); }
            //}

            // Log Power if change detected
            if ((LogLists.RecentEntries.PowerGood_5V != Globals.GpioServer.PowerGood_5V) ||
                (LogLists.RecentEntries.PowerGood_12V != Globals.GpioServer.PowerGood_12V) ||
                (LogLists.RecentEntries.PowerGood_24V != Globals.GpioServer.PowerGood_24V) ||
                (LogLists.RecentEntries.PowerFail_5V != Globals.GpioServer.PowerFail_5V) ||
                (LogLists.RecentEntries.PowerFail_12V != Globals.GpioServer.PowerFail_12V) ||
                (LogLists.RecentEntries.PowerFail_24V != Globals.GpioServer.PowerFail_24V) ||
                (LogLists.RecentEntries.WatchdogPrealarm != Globals.GpioServer.WatchDog_Prewarn))
            {
                try
                {
                    Globals.PowerLogger.Stop();
                    while (LogLists.PowerLog.Count > 65500) { LogLists.PowerLog.RemoveAt(0); }
                    LogLists.PowerLog.Add(new cPowerLog
                    {
                        TimeStamp = DateTime.Now,
                        PowerGood_5V = (byte)(Globals.GpioServer.PowerGood_5V ? 0 : 2),
                        PowerGood_12V = (byte)(Globals.GpioServer.PowerGood_12V ? 0 : 2),
                        PowerGood_24V = (byte)(Globals.GpioServer.PowerGood_24V ? 0 : 2),
                        PowerFail_5V = (byte)(Globals.GpioServer.PowerFail_5V ? 0 : 3),
                        PowerFail_12V = (byte)(Globals.GpioServer.PowerFail_12V ? 0 : 3),
                        PowerFail_24V = (byte)(Globals.GpioServer.PowerFail_24V ? 0 : 3),
                        WatchdogPrealarm = (byte)(Globals.GpioServer.WatchDog_Prewarn ? 1 : 0)
                    });
                }
                finally { Globals.PowerLogger.Start(); }
            }

            // Log Watering Data with FastLog
            if (Globals.WateringActive)
            {
                LogLists.WateringLog1.Add(new cWaterLog
                {
                    TimeStamp = DateTime.Now,
                    Flow1 = LogLists.RecentEntries.Flow1,
                    Flow2 = LogLists.RecentEntries.Flow2,
                    Flow3 = LogLists.RecentEntries.Flow3,
                    Flow4 = LogLists.RecentEntries.Flow4,
                    Flow5 = LogLists.RecentEntries.Flow5,
                    Pump = (byte)(LogLists.RecentEntries.Pump ? 2 : 0),
                    Valve1 = (byte)(LogLists.RecentEntries.Valve1 ? 1 : 0),
                    Valve2 = (byte)(LogLists.RecentEntries.Valve2 ? 1 : 0),
                    Valve3 = (byte)(LogLists.RecentEntries.Valve3 ? 1 : 0),
                    Valve4 = (byte)(LogLists.RecentEntries.Valve4 ? 1 : 0),
                    Valve5 = (byte)(LogLists.RecentEntries.Valve5 ? 1 : 0),
                    Pressure = LogLists.RecentEntries.Pressure,
                    TempCPU = LogLists.RecentEntries.TempCPU,

                    PowerGood_5V = (byte)(LogLists.RecentEntries.PowerGood_5V ? 2 : 0),
                    PowerGood_12V = (byte)(LogLists.RecentEntries.PowerGood_12V ? 2 : 0),
                    PowerGood_24V = (byte)(LogLists.RecentEntries.PowerGood_24V ? 2 : 0),
                    PowerFail_5V = (byte)(LogLists.RecentEntries.PowerFail_5V ? 3 : 0),
                    PowerFail_12V = (byte)(LogLists.RecentEntries.PowerFail_12V ? 3 : 0),
                    PowerFail_24V = (byte)(LogLists.RecentEntries.PowerFail_24V ? 3 : 0),
                    WatchdogPrealarm = (byte)(LogLists.RecentEntries.WatchdogPrealarm ? 1 : 0)
                });

            }

            // Update recents
            LogLists.RecentEntries.TimeStamp = DateTime.Now;

            //Console.WriteLine($">>> Get TWI data");
            LogLists.RecentEntries.TempAmb = Globals.TwiServer.AmbientTemp;
            LogLists.RecentEntries.TempExp = Globals.TwiServer.ExposedTemp;
            LogLists.RecentEntries.TankForce = Globals.TwiServer.TankWeight;

            LogLists.RecentEntries.TempAmbFiltered = 0; // not used
            LogLists.RecentEntries.TempCPUFiltered = 0; // not used
            LogLists.RecentEntries.TempExpFiltered = 0; // not used

            //Console.WriteLine($">>> Get SPI data");
            LogLists.RecentEntries.Flow1 = Globals.SpiServer.Flow1 * 10;
            LogLists.RecentEntries.Flow2 = Globals.SpiServer.Flow2 * 10;
            LogLists.RecentEntries.Flow3 = Globals.SpiServer.Flow3 * 10;
            LogLists.RecentEntries.Flow4 = Globals.SpiServer.Flow4 * 10;
            LogLists.RecentEntries.Flow5 = Globals.SpiServer.Flow5 * 10;
            //if ((Globals.SpiServer.Level < LogLists.RecentEntries.Tank) || (Globals.SpiServer.Level > LogLists.RecentEntries.Tank + Settings.System.Tank_RefillHyst))
            //    { LogLists.RecentEntries.Tank = Globals.SpiServer.Level; }
            double TankLevel_L = (Globals.TwiServer.TankWeight * 0.0081) - 18;                      // Force to volume in liter
            LogLists.RecentEntries.Tank = (byte)Math.Floor(((TankLevel_L / 75) - 0.063) * 100);     // Liter to Percent
            LogLists.RecentEntries.Rain = vRain;
            LogLists.RecentEntries.Ground = vGround;
            LogLists.RecentEntries.Pressure = Globals.SpiServer.Pressure;
            LogLists.RecentEntries.LevelRaw = Globals.SpiServer.LevelRaw;



            LogLists.RecentEntries.TempCPU = DataProvisionController.GetTemperature();

            //Console.WriteLine($">>> Get GPIO data");
            LogLists.RecentEntries.Pump = Globals.GpioServer.PumpActive;
            LogLists.RecentEntries.Valve1 = Globals.GpioServer.Valve1Open;
            LogLists.RecentEntries.Valve2 = Globals.GpioServer.Valve2Open;
            LogLists.RecentEntries.Valve3 = Globals.GpioServer.Valve3Open;
            LogLists.RecentEntries.Valve4 = Globals.GpioServer.Valve4Open;
            LogLists.RecentEntries.Valve5 = Globals.GpioServer.Valve5Open;
            LogLists.RecentEntries.PowerGood_5V = Globals.GpioServer.PowerGood_5V;
            LogLists.RecentEntries.PowerGood_12V = Globals.GpioServer.PowerGood_12V;
            LogLists.RecentEntries.PowerGood_24V = Globals.GpioServer.PowerGood_24V;
            LogLists.RecentEntries.PowerFail_5V = Globals.GpioServer.PowerFail_5V;
            LogLists.RecentEntries.PowerFail_12V = Globals.GpioServer.PowerFail_12V;
            LogLists.RecentEntries.PowerFail_24V = Globals.GpioServer.PowerFail_24V;
            LogLists.RecentEntries.WatchdogPrealarm = Globals.GpioServer.WatchDog_Prewarn;

        }

        private static void MainTask_Routine(object sender, ElapsedEventArgs e)
        {
            // reset alarm jitter suppression
            int vCurrentTemp = DataProvisionController.GetTemperature();
            if ((Globals.ALM_WarnTempCPUactive) && (vCurrentTemp < Settings.System.ALM_WarnTempCPU - 5))
            {
                SysLog(LogType.Information, "RESET - High CPU Temperature", "The Temperature of the CPU dropped below the warning release threshold of " + Settings.System.ALM_WarnTempCPU + " -5°C");
                Globals.ALM_WarnTempCPUactive = false;
            }
            if ((Globals.ALM_MaxTempCPUactive) && (vCurrentTemp < Settings.System.ALM_MaxTempCPU - 3))
            {
                SysLog(LogType.Information, "RESET - Maximum CPU Operation Temperature", "The Temperature of the CPU dropped below the critical temperature release threshold of " + Settings.System.ALM_WarnTempCPU + " -3°C");
                Globals.ALM_MaxTempCPUactive = false;
            }
            if ((Globals.ALM_WarnTankLevelactive) && (LogLists.RecentEntries.Tank > Settings.System.ALM_TankLevel + 3))
            {
                SysLog(LogType.Information, "RESET - Tank Level LOW", "The water level of the tank reached the warning realease threshold of " + Settings.System.ALM_TankLevel + " +3%");
                Globals.ALM_WarnTankLevelactive = false;
            }
            if ((Globals.ALM_MaxPressactive) && (LogLists.RecentEntries.Pressure < ((Settings.System.ALM_MaxPress - 500) / 1000)))
            {
                SysLog(LogType.Information, "RESET - Pressure HIGH", "The pressure dropped below " + Settings.System.ALM_MaxPress / 1000 + " -0.5 bar");
                Globals.ALM_MaxPressactive = false;
            }

            // NEW: MQTT publish
            MQTT.Update(MQTT.Flow1, LogLists.RecentEntries.Flow1);
            MQTT.Update(MQTT.Flow2, LogLists.RecentEntries.Flow2);
            MQTT.Update(MQTT.Flow3, LogLists.RecentEntries.Flow3);
            MQTT.Update(MQTT.Flow4, LogLists.RecentEntries.Flow4);
            MQTT.Update(MQTT.Flow5, LogLists.RecentEntries.Flow5);
            MQTT.Update(MQTT.Pump, LogLists.RecentEntries.Pump);
            MQTT.Update(MQTT.Valve1, LogLists.RecentEntries.Valve1);
            MQTT.Update(MQTT.Valve2, LogLists.RecentEntries.Valve2);
            MQTT.Update(MQTT.Valve3, LogLists.RecentEntries.Valve3);
            MQTT.Update(MQTT.Valve4, LogLists.RecentEntries.Valve4);
            MQTT.Update(MQTT.Valve5, LogLists.RecentEntries.Valve5);
            MQTT.Update(MQTT.Tank, LogLists.RecentEntries.Tank);
            MQTT.Update(MQTT.TankForce, LogLists.RecentEntries.TankForce);
            MQTT.Update(MQTT.Rain, LogLists.RecentEntries.Rain);
            MQTT.Update(MQTT.Ground, LogLists.RecentEntries.Ground);
            MQTT.Update(MQTT.Pressure, LogLists.RecentEntries.Pressure);
            MQTT.Update(MQTT.LevelRaw, LogLists.RecentEntries.LevelRaw);
            MQTT.Update(MQTT.TempCPU, LogLists.RecentEntries.TempCPU);
            MQTT.Update(MQTT.TempAmb, LogLists.RecentEntries.TempAmb);
            MQTT.Update(MQTT.TempExp, LogLists.RecentEntries.TempExp);
            MQTT.Update(MQTT.PowerGood_5V, LogLists.RecentEntries.PowerGood_5V);
            MQTT.Update(MQTT.PowerGood_12V, LogLists.RecentEntries.PowerGood_12V);
            MQTT.Update(MQTT.PowerGood_24V, LogLists.RecentEntries.PowerGood_24V);
            MQTT.Update(MQTT.PowerFail_5V, LogLists.RecentEntries.PowerFail_5V);
            MQTT.Update(MQTT.PowerFail_12V, LogLists.RecentEntries.PowerFail_12V);
            MQTT.Update(MQTT.PowerFail_24V, LogLists.RecentEntries.PowerFail_24V);
            MQTT.Update(MQTT.WatchDog, LogLists.RecentEntries.WatchdogPrealarm);
            MQTT.Update(MQTT.TempDiff, (LogLists.RecentEntries.TempExp - LogLists.RecentEntries.TempAmb));

            MQTT.Publish();


            // NEW: Tank Level Stack for avarage calculation
            if (!Globals.WateringActive)
            {
                Globals.TankLevel.Enqueue(LogLists.RecentEntries.TankForce);
            }
            else
            {
                if (LogLists.LevelLog.Count == 65500) { LogLists.LevelLog.RemoveAt(0); }
                LogLists.LevelLog.Add(new cLevelLog
                {
                    TimeStamp = DateTime.Now,
                    Tank = LogLists.RecentEntries.Tank
                });
            }

            if (Globals.TankLevel.Count > 1200) { Globals.TankLevel.Dequeue(); } // avoid exceptional growing

            // DEBUG : Log Tank Force
            /*
            if (LogCount < 86400)
            {
                LogCount++; 
                string filename = "usrdata/TankLog" + LogNumber.ToString() + ".csv";
                System.IO.File.AppendAllText(filename, DateTime.Now.ToString() + ";" + LogLists.RecentEntries.TankForce.ToString()+"\n");
            }
            else
            {
                LogCount = 0;
                LogNumber++;
                string filename = "usrdata/TankLog" + LogNumber.ToString() + ".csv";
                System.IO.File.WriteAllText(filename, "");
            }
            */


            // Main task for watering
            if (!Globals.WateringActive)
            {
                #region Morning
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Monday morning", "The routine on Monday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Mon && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Mon && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Mon && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Mon && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Mon && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Tuesday morning", "The routine on Tuesday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Tue && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Tue && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Tue && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Tue && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Tue && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Wednesday morning", "The routine on Wednesday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Wed && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Wed && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Wed && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Wed && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Wed && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Thursday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Thursday morning", "The routine on Thursday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Thu && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Thu && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Thu && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Thu && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Thu && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Friday morning", "The routine on Friday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Fri && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Fri && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Fri && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Fri && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Fri && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Saturday morning", "The routine on Saturday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sat && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Sat && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Sat && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Sat && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Sat && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && (DateTime.Now.Hour == Settings.Watering.TimeMorning) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Sunday morning", "The routine on Sunday morning for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sun && Settings.Watering.Morning_1),
                             (Settings.Watering.Plant_2_Sun && Settings.Watering.Morning_2),
                             (Settings.Watering.Plant_3_Sun && Settings.Watering.Morning_3),
                             (Settings.Watering.Plant_4_Sun && Settings.Watering.Morning_4),
                             (Settings.Watering.Plant_5_Sun && Settings.Watering.Morning_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Morning");
                }
                #endregion Morning
                #region Noon
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Monday noon", "The routine on Monday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Mon && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Mon && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Mon && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Mon && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Mon && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Tuesday noon", "The routine on Tuesday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Tue && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Tue && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Tue && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Tue && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Tue && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Wednesday noon", "The routine on Wednesday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Wed && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Wed && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Wed && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Wed && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Wed && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Thursday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Thursday noon", "The routine on Thursday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Thu && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Thu && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Thu && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Thu && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Thu && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Friday noon", "The routine on Friday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Fri && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Fri && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Fri && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Fri && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Fri && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Saturday noon", "The routine on Saturday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sat && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Sat && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Sat && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Sat && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Sat && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && (DateTime.Now.Hour == Settings.Watering.TimeNoon) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Sunday noon", "The routine on Sunday noon for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sun && Settings.Watering.Noon_1),
                             (Settings.Watering.Plant_2_Sun && Settings.Watering.Noon_2),
                             (Settings.Watering.Plant_3_Sun && Settings.Watering.Noon_3),
                             (Settings.Watering.Plant_4_Sun && Settings.Watering.Noon_4),
                             (Settings.Watering.Plant_5_Sun && Settings.Watering.Noon_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Noon");
                }
                #endregion Noon
                #region Evening
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Monday evening", "The routine on Monday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Mon && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Mon && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Mon && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Mon && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Mon && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Tuesday evening", "The routine on Tuesday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Tue && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Tue && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Tue && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Tue && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Tue && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Wednesday evening", "The routine on Wednesday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Wed && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Wed && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Wed && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Wed && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Wed && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Thursday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Thursday evening", "The routine on Thursday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Thu && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Thu && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Thu && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Thu && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Thu && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Friday evening", "The routine on Friday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Fri && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Fri && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Fri && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Fri && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Fri && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Saturday evening", "The routine on Saturday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sat && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Sat && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Sat && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Sat && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Sat && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && (DateTime.Now.Hour == Settings.Watering.TimeEvening) && (DateTime.Now.Minute == 0) && (DateTime.Now.Second == 0))
                {
                    //WatLog(LogType.Status, "Trigger Sunday evening", "The routine on Sunday evening for watering of plants was triggered.");
                    Watering((Settings.Watering.Plant_1_Sun && Settings.Watering.Evening_1),
                             (Settings.Watering.Plant_2_Sun && Settings.Watering.Evening_2),
                             (Settings.Watering.Plant_3_Sun && Settings.Watering.Evening_3),
                             (Settings.Watering.Plant_4_Sun && Settings.Watering.Evening_4),
                             (Settings.Watering.Plant_5_Sun && Settings.Watering.Evening_5), String.Format("{0}", DateTime.Now.DayOfWeek), "Evening");
                }
                #endregion Evening
            }
        }

        private static void Watering(bool Out1_active, bool Out2_active, bool Out3_active, bool Out4_active, bool Out5_active, string vDay, string vTime)
        {
            WatLog(LogType.Information, String.Format("Check to trigger {0} {1}", vDay, vTime), String.Format("If selected, the routine on {0} {1} for watering of plants will be triggered.", vDay, vTime));
            if (Out1_active || Out2_active || Out3_active || Out4_active || Out5_active)
            {
                WatLog(LogType.Status, String.Format("Trigger {0} {1}", vDay, vTime), String.Format("The routine on {0} {1} for watering of plants was triggered.", vDay, vTime));

                if (LogLists.RecentEntries.Tank < Settings.System.Wat_min_tank)
                {
                    SysLog(LogType.Warning, "Tank Level to low for Watering", "The Tank Level reached the minimum threshold of " + Settings.System.Wat_min_tank + "%. Watering has been suspended!");
                    return;
                }
                // ring buffer for watering log newest first
                LogLists.WateringLog3.Clear();
                LogLists.WateringLog3.AddRange(LogLists.WateringLog2.ToList());
                LogLists.WateringLog2.Clear();
                LogLists.WateringLog2.AddRange(LogLists.WateringLog1.ToList());
                LogLists.WateringLog1.Clear();

                Globals.WateringActive = true;

                // watering procedures
                try
                {
                    #region Variables
                    byte wt1 = 0;
                    byte wt2 = 0;
                    byte wt3 = 0;
                    byte wt4 = 0;
                    byte wt5 = 0;

                    int tmp_GAF1 = Settings.Watering.GAF_1;
                    int tmp_GAF2 = Settings.Watering.GAF_2;
                    int tmp_GAF3 = Settings.Watering.GAF_3;
                    int tmp_GAF4 = Settings.Watering.GAF_4;
                    int tmp_GAF5 = Settings.Watering.GAF_5;

                    int tmp_RAF1 = Settings.Watering.RAF_1;
                    int tmp_RAF2 = Settings.Watering.RAF_2;
                    int tmp_RAF3 = Settings.Watering.RAF_3;
                    int tmp_RAF4 = Settings.Watering.RAF_4;
                    int tmp_RAF5 = Settings.Watering.RAF_5;
                    #endregion Variables
                    #region Factors

                    if ((LogLists.RecentEntries.Rain == 2) && (Out1_active || Out2_active || Out3_active || Out4_active || Out5_active))
                    {   // Rain
                        WatLog(LogType.Information, "Rain recognized - Rain factor applied", "The rain sensor is active. The rain factor will be applied to the watering volume.");
                        tmp_RAF1 = Settings.Watering.RAF_1;
                        tmp_RAF2 = Settings.Watering.RAF_2;
                        tmp_RAF3 = Settings.Watering.RAF_3;
                        tmp_RAF4 = Settings.Watering.RAF_4;
                        tmp_RAF5 = Settings.Watering.RAF_5;
                    }
                    else
                    {
                        tmp_RAF1 = 100;
                        tmp_RAF2 = 100;
                        tmp_RAF3 = 100;
                        tmp_RAF4 = 100;
                        tmp_RAF5 = 100;
                    }

                    if ((LogLists.RecentEntries.Ground == 2) && (Out1_active || Out2_active || Out3_active || Out4_active || Out5_active))
                    {   // Ground damp
                        WatLog(LogType.Information, "Ground damp - Ground factor applied", "The ground sensor is active. The ground factor will be applied to the watering volume.");
                        tmp_GAF1 = Settings.Watering.GAF_1;
                        tmp_GAF2 = Settings.Watering.GAF_2;
                        tmp_GAF3 = Settings.Watering.GAF_3;
                        tmp_GAF4 = Settings.Watering.GAF_4;
                        tmp_GAF5 = Settings.Watering.GAF_5;
                    }
                    else
                    {
                        tmp_GAF1 = 100;
                        tmp_GAF2 = 100;
                        tmp_GAF3 = 100;
                        tmp_GAF4 = 100;
                        tmp_GAF5 = 100;
                    }

                    double tmp_Vol1 = Settings.Watering.Volume_1 * (tmp_RAF1 / 100.0) * (tmp_GAF1 / 100.0);
                    double tmp_Vol2 = Settings.Watering.Volume_2 * (tmp_RAF2 / 100.0) * (tmp_GAF2 / 100.0);
                    double tmp_Vol3 = Settings.Watering.Volume_3 * (tmp_RAF3 / 100.0) * (tmp_GAF3 / 100.0);
                    double tmp_Vol4 = Settings.Watering.Volume_4 * (tmp_RAF4 / 100.0) * (tmp_GAF4 / 100.0);
                    double tmp_Vol5 = Settings.Watering.Volume_5 * (tmp_RAF5 / 100.0) * (tmp_GAF5 / 100.0);

                    if (tmp_Vol1 > Settings.System.Wat_max_vol) { tmp_Vol1 = Settings.System.Wat_max_vol; WatLog(LogType.Warning, "Max volume reached (1) - " + Settings.System.Wat_max_vol + "ml applied", "After application of ground and rain factor, the maximum value for the watering volume of output 1 reached. " + Settings.System.Wat_max_vol + "ml (maximum limit) was set."); }
                    if (tmp_Vol2 > Settings.System.Wat_max_vol) { tmp_Vol2 = Settings.System.Wat_max_vol; WatLog(LogType.Warning, "Max volume reached (2) - " + Settings.System.Wat_max_vol + "ml applied", "After application of ground and rain factor, the maximum value for the watering volume of output 2 reached. " + Settings.System.Wat_max_vol + "ml (maximum limit) was set."); }
                    if (tmp_Vol3 > Settings.System.Wat_max_vol) { tmp_Vol3 = Settings.System.Wat_max_vol; WatLog(LogType.Warning, "Max volume reached (3) - " + Settings.System.Wat_max_vol + "ml applied", "After application of ground and rain factor, the maximum value for the watering volume of output 3 reached. " + Settings.System.Wat_max_vol + "ml (maximum limit) was set."); }
                    if (tmp_Vol4 > Settings.System.Wat_max_vol) { tmp_Vol4 = Settings.System.Wat_max_vol; WatLog(LogType.Warning, "Max volume reached (4) - " + Settings.System.Wat_max_vol + "ml applied", "After application of ground and rain factor, the maximum value for the watering volume of output 4 reached. " + Settings.System.Wat_max_vol + "ml (maximum limit) was set."); }
                    if (tmp_Vol5 > Settings.System.Wat_max_vol) { tmp_Vol5 = Settings.System.Wat_max_vol; WatLog(LogType.Warning, "Max volume reached (5) - " + Settings.System.Wat_max_vol + "ml applied", "After application of ground and rain factor, the maximum value for the watering volume of output 5 reached. " + Settings.System.Wat_max_vol + "ml (maximum limit) was set."); }

                    if (tmp_Vol1 < Settings.System.Wat_min_vol) { tmp_Vol1 = Settings.System.Wat_min_vol; WatLog(LogType.Warning, "Min volume reached (1) - " + Settings.System.Wat_min_vol + "ml applied", "After application of ground and rain factor, the minimum value for the watering volume of output 1 reached. " + Settings.System.Wat_min_vol + "ml (minimum limit) was set."); }
                    if (tmp_Vol2 < Settings.System.Wat_min_vol) { tmp_Vol2 = Settings.System.Wat_min_vol; WatLog(LogType.Warning, "Min volume reached (2) - " + Settings.System.Wat_min_vol + "ml applied", "After application of ground and rain factor, the minimum value for the watering volume of output 2 reached. " + Settings.System.Wat_min_vol + "ml (minimum limit) was set."); }
                    if (tmp_Vol3 < Settings.System.Wat_min_vol) { tmp_Vol3 = Settings.System.Wat_min_vol; WatLog(LogType.Warning, "Min volume reached (3) - " + Settings.System.Wat_min_vol + "ml applied", "After application of ground and rain factor, the minimum value for the watering volume of output 3 reached. " + Settings.System.Wat_min_vol + "ml (minimum limit) was set."); }
                    if (tmp_Vol4 < Settings.System.Wat_min_vol) { tmp_Vol4 = Settings.System.Wat_min_vol; WatLog(LogType.Warning, "Min volume reached (4) - " + Settings.System.Wat_min_vol + "ml applied", "After application of ground and rain factor, the minimum value for the watering volume of output 4 reached. " + Settings.System.Wat_min_vol + "ml (minimum limit) was set."); }
                    if (tmp_Vol5 < Settings.System.Wat_min_vol) { tmp_Vol5 = Settings.System.Wat_min_vol; WatLog(LogType.Warning, "Min volume reached (5) - " + Settings.System.Wat_min_vol + "ml applied", "After application of ground and rain factor, the minimum value for the watering volume of output 5 reached. " + Settings.System.Wat_min_vol + "ml (minimum limit) was set."); }

                    #endregion Factors
                    if (Out1_active)
                    {
                        WatLog(LogType.Status, "Watering Plant 1", "The routine activated watering of plant #1 for " + tmp_Vol1 + " ml");
                        Globals.GpioServer.OpenValve1();                                                       // Open Valve #1
                        var t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_ValveOpen); });
                        t_wait.Wait();     // wait 2s
                        var t_Water1 = Task.Run(async delegate
                        {
                            Globals.GpioServer.StartPump();                                                   // Start Pump
                            while ((Globals.SpiServer.Flow1 * 10) < tmp_Vol1)
                            {
                                await Task.Delay(1000);
                                wt1++;
                                if (wt1 >= Settings.System.Wat_max_time)
                                {
                                    WatLog(LogType.Warning, "Watering 1 reached max time", "The watering procedure on line #1 reached the maximum time for watering and was aborted after " + wt1 + "sec");
                                    break;
                                }
                            }
                            Globals.GpioServer.StopPump();                                                  // Stop Pump
                        });
                        t_Water1.Wait();                                                                        // Check flow every second and wait for full volume watering
                        WatLog(LogType.Information, "Watering Plant 1 finished (" + Globals.SpiServer.Flow1 + "0 ml/" + wt1 + "s)", "The watering procedure of Plant 1 ended after " + wt1 + "sec and " + Globals.SpiServer.Flow1 + "0 ml");
                        t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_PumpStop); });
                        t_wait.Wait();                                                                          // wait 5s to depressurize
                        Globals.GpioServer.CloseValve1();                                                      // Close Valve #1
                    }
                    if (Out2_active)
                    {
                        WatLog(LogType.Status, "Watering Plant 2", "The routine activated watering of plant #2 for " + tmp_Vol2 + " ml");
                        Globals.GpioServer.OpenValve2();                                                       // Open Valve #2
                        var t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_ValveOpen); });
                        t_wait.Wait();     // wait 2s
                        var t_Water2 = Task.Run(async delegate
                        {
                            Globals.GpioServer.StartPump();                                                   // Start Pump
                            while ((Globals.SpiServer.Flow2 * 10) < tmp_Vol2)
                            {
                                await Task.Delay(1000);
                                wt2++;
                                if (wt2 >= Settings.System.Wat_max_time)
                                {
                                    WatLog(LogType.Warning, "Watering 2 reached max time", "The watering procedure on line #2 reached the maximum time for watering and was aborted " + wt2 + "sec");
                                    break;
                                }
                            }
                            Globals.GpioServer.StopPump();                                                  // Stop Pump
                        });
                        t_Water2.Wait();                                             // Check flow every second and wait for full volume watering
                        WatLog(LogType.Information, "Watering Plant 2 finished (" + Globals.SpiServer.Flow2 + "0 ml/" + wt2 + "s)", "The watering procedure of Plant 2 ended after " + wt2 + "sec and " + Globals.SpiServer.Flow2 + "0 ml");
                        t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_PumpStop); });
                        t_wait.Wait();                                                                  // wait 5s to depressurize
                        Globals.GpioServer.CloseValve2();                                                      // Close Valve #2
                    }
                    if (Out3_active)
                    {
                        WatLog(LogType.Status, "Watering Plant 3", "The routine activated watering of plant #3 for " + tmp_Vol3 + " ml");
                        Globals.GpioServer.OpenValve3();                                                       // Open Valve #3
                        var t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_ValveOpen); });
                        t_wait.Wait();     // wait 2s
                        var t_Water3 = Task.Run(async delegate
                        {
                            Globals.GpioServer.StartPump();                                                   // Start Pump
                            while ((Globals.SpiServer.Flow3 * 10) < tmp_Vol3)
                            {
                                await Task.Delay(1000);
                                wt3++;
                                if (wt3 >= Settings.System.Wat_max_time)
                                {
                                    WatLog(LogType.Warning, "Watering 3 reached max time", "The watering procedure on line #3 reached the maximum time for watering and was aborted " + wt3 + "sec");
                                    break;
                                }
                            }
                            Globals.GpioServer.StopPump();                                                  // Stop Pump
                        });
                        t_Water3.Wait();                                             // Check flow every second and wait for full volume watering
                        WatLog(LogType.Information, "Watering Plant 3 finished (" + Globals.SpiServer.Flow3 + "0 ml/" + wt3 + "s)", "The watering procedure of Plant 3 ended after " + wt3 + "sec and " + Globals.SpiServer.Flow3 + "0 ml");
                        t_wait = Task.Run(async delegate
                        {
                            await Task.Delay(Settings.System.DLY_PumpStop);
                        });
                        t_wait.Wait();     // wait 5s to depressurize
                        Globals.GpioServer.CloseValve3();                                                      // Close Valve #3
                    }
                    if (Out4_active)
                    {
                        WatLog(LogType.Status, "Watering Plant 4", "The routine activated watering of plant #4 " + tmp_Vol4 + " ml");
                        Globals.GpioServer.OpenValve4();                                                       // Open Valve #4
                        var t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_ValveOpen); });
                        t_wait.Wait();     // wait 2s
                        var t_Water4 = Task.Run(async delegate
                        {
                            Globals.GpioServer.StartPump();                                                   // Start Pump
                            while ((Globals.SpiServer.Flow4 * 10) < tmp_Vol4)
                            {
                                await Task.Delay(1000);
                                wt4++;
                                if (wt4 >= Settings.System.Wat_max_time)
                                {
                                    WatLog(LogType.Warning, "Watering 4 reached max time", "The watering procedure on line #4 reached the maximum time for watering and was aborted " + wt4 + "sec");
                                    break;
                                }
                            }
                            Globals.GpioServer.StopPump();                                                  // Stop Pump
                        });
                        t_Water4.Wait();                                             // Check flow every second and wait for full volume watering
                        WatLog(LogType.Information, "Watering Plant 4 finished (" + Globals.SpiServer.Flow4 + "0 ml/" + wt4 + "s)", "The watering procedure of Plant 4 ended after " + wt4 + "sec and " + Globals.SpiServer.Flow4 + "0 ml");
                        t_wait = Task.Run(async delegate
                        {
                            await Task.Delay(Settings.System.DLY_PumpStop);
                        });
                        t_wait.Wait();     // wait 5s to depressurize
                        Globals.GpioServer.CloseValve4();                                                      // Close Valve #4
                    }
                    if (Out5_active)
                    {
                        WatLog(LogType.Status, "Watering Plant 5", "The routine activated watering of plant #5 " + tmp_Vol5 + " ml");
                        Globals.GpioServer.OpenValve5();                                                       // Open Valve #5
                        var t_wait = Task.Run(async delegate { await Task.Delay(Settings.System.DLY_ValveOpen); });
                        t_wait.Wait();     // wait 2s
                        var t_Water5 = Task.Run(async delegate
                        {
                            Globals.GpioServer.StartPump();                                                   // Start Pump
                            while ((Globals.SpiServer.Flow5 * 10) < tmp_Vol5)
                            {
                                await Task.Delay(1000);
                                wt5++;
                                if (wt5 >= Settings.System.Wat_max_time)
                                {
                                    WatLog(LogType.Warning, "Watering 5 reached max time", "The watering procedure on line #5 reached the maximum time for watering and was aborted " + wt5 + "sec");
                                    break;
                                }
                            }
                            Globals.GpioServer.StopPump();                                                  // Stop Pump
                        });
                        t_Water5.Wait();                                             // Check flow every second and wait for full volume watering
                        WatLog(LogType.Information, "Watering Plant 5 finished (" + Globals.SpiServer.Flow5 + "0 ml/" + wt5 + "s)", "The watering procedure of Plant 5 ended after" + wt5 + "sec and " + Globals.SpiServer.Flow5 + "0 ml");
                        t_wait = Task.Run(async delegate
                        {
                            await Task.Delay(Settings.System.DLY_PumpStop);
                        });
                        t_wait.Wait();     // wait 5s to depressurize
                        Globals.GpioServer.CloseValve5();                                                      // Close Valve #5
                    }
                    Globals.SpiServer.SPI_Reset();
                }
                finally { Globals.WateringActive = false; }
            }
        }

        private static void LevelLogger_Routine(object sender, ElapsedEventArgs e)
        {
            int sum = 0;
            int num = Globals.TankLevel.Count;
            foreach (Object obj in Globals.TankLevel)
            {
                sum += (int)obj;
            }
            double avg = sum / num;
            avg = (avg * 0.0081) - 18;          // Force to volume in liter
            avg = ((avg / 75) - 0.063) * 100;   // Liter to Percent

            while (LogLists.LevelLog.Count > 65500) { LogLists.LevelLog.RemoveAt(0); }
            LogLists.LevelLog.Add(new cLevelLog
            {
                TimeStamp = DateTime.Now,
                Tank = (byte)Math.Floor(avg)
            });
        }

        private static void EnvLogger_Routine(object sender, ElapsedEventArgs e)
        {
            byte vRain = 0;
            byte vGround = 0;
            switch (Math.Floor((double)(Globals.SpiServer.Rain / 100)))
            {
                case 1: vRain = 1; break;
                case 2: vRain = 2; break;
                default: vRain = 0; break;
            }

            switch (Math.Floor((double)(Globals.SpiServer.Ground / 100)))
            {
                case 1: vGround = 1; break;
                case 2: vGround = 2; break;
                default: vGround = 0; break;
            }

            while (LogLists.EnvLog.Count > 65500) { LogLists.EnvLog.RemoveAt(0); }
            LogLists.EnvLog.Add(new cEnvLog
            {
                TimeStamp = DateTime.Now,
                Rain = vRain,
                Ground = vGround,
                TempAmb = Globals.TwiServer.AmbientTemp,
                TempCPU = DataProvisionController.GetTemperature(),
                TempExp = Globals.TwiServer.ExposedTemp
            });
        }

        private static void PowerLogger_Routine(object sender, ElapsedEventArgs e)
        {
            while (LogLists.PowerLog.Count > 65500) { LogLists.PowerLog.RemoveAt(0); }
            LogLists.PowerLog.Add(new cPowerLog
            {
                TimeStamp = DateTime.Now,
                PowerGood_5V = (byte)(Globals.GpioServer.PowerGood_5V ? 0 : 2),
                PowerGood_12V = (byte)(Globals.GpioServer.PowerGood_12V ? 0 : 2),
                PowerGood_24V = (byte)(Globals.GpioServer.PowerGood_24V ? 0 : 2),
                PowerFail_5V = (byte)(Globals.GpioServer.PowerFail_5V ? 0 : 3),
                PowerFail_12V = (byte)(Globals.GpioServer.PowerFail_12V ? 0 : 3),
                PowerFail_24V = (byte)(Globals.GpioServer.PowerFail_24V ? 0 : 3),
                WatchdogPrealarm = (byte)(Globals.GpioServer.WatchDog_Prewarn ? 1 : 0)
            });
        }

        private static void SaveTask_Routine(object sender, ElapsedEventArgs e)
        {
            // Frequent log to file
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/JournalLog.json", JsonConvert.SerializeObject(LogLists.JournalLog.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/LevelLog.json", JsonConvert.SerializeObject(LogLists.LevelLog.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/EnvironmentLog.json", JsonConvert.SerializeObject(LogLists.EnvLog.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/PowerLog.json", JsonConvert.SerializeObject(LogLists.PowerLog.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/Watering1Log.json", JsonConvert.SerializeObject(LogLists.WateringLog1.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/Watering2Log.json", JsonConvert.SerializeObject(LogLists.WateringLog2.ToList()));
            System.IO.File.WriteAllText(@"usrdata/SavedLogs/Watering3Log.json", JsonConvert.SerializeObject(LogLists.WateringLog3.ToList()));
        }

        private static void BufferClean_Task(object sender, ElapsedEventArgs e)
        {
            for (var i = 0; i < LogLists.JournalLog.Count; i++)
            {
                if (DateTime.Parse(LogLists.JournalLog[i].TimeStamp) < DateTime.Today.AddDays(-Settings.System.LOG_Buffer_Expiry))
                {
                    LogLists.JournalLog.RemoveAt(i);
                }
            }
            for (var i = 0; i < LogLists.JournalBuffer.Count; i++)
            {
                if (DateTime.Parse(LogLists.JournalBuffer[i].TimeStamp) < DateTime.Today.AddDays(-Settings.System.LOG_Buffer_Expiry))
                {
                    LogLists.JournalBuffer.RemoveAt(i);
                }
            }
        }

        public static bool Initialize()
        {
            if (!Globals.IsInitialized)
            {
                // Load Settings
                SettingsController.ReadFromFileStatic();

                Globals.FastTask.Interval = Settings.System.FastLog;
                Globals.MainTask.Interval = Settings.System.Task_cycle;
                Globals.SaveTask.Interval = Settings.System.Save_cycle * 60000;
                Globals.LevelLogger.Interval = Settings.System.Log_Level * 60000;
                Globals.EnvLogger.Interval = Settings.System.Log_Enviroment * 60000;
                Globals.PowerLogger.Interval = Settings.System.Log_Power * 60000;

                Globals.SpiServer.MinLevel = Settings.System.Tank_Min;
                Globals.SpiServer.MaxLevel = Settings.System.Tank_Max;

                // DEBUG : Log Tank Force
                string filename1 = "usrdata/TankLog0.csv";
                System.IO.File.WriteAllText(filename1, "Timestamp;Value\n");
                string filename2 = "usrdata/IPaccess.csv";
                System.IO.File.WriteAllText(filename2, "TimeStamp;IP address;Target;Country;Region;City;Timezone\n");

                // Load saved Logs

                LogLists.JournalLog = JsonConvert.DeserializeObject<List<cJournal>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/JournalLog.json"));
                LogLists.LevelLog = JsonConvert.DeserializeObject<List<cLevelLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/LevelLog.json"));
                LogLists.EnvLog = JsonConvert.DeserializeObject<List<cEnvLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/EnvironmentLog.json"));
                LogLists.PowerLog = JsonConvert.DeserializeObject<List<cPowerLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/PowerLog.json"));
                LogLists.WateringLog1 = JsonConvert.DeserializeObject<List<cWaterLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/Watering1Log.json"));
                LogLists.WateringLog2 = JsonConvert.DeserializeObject<List<cWaterLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/Watering2Log.json"));
                LogLists.WateringLog3 = JsonConvert.DeserializeObject<List<cWaterLog>>(System.IO.File.ReadAllText(@"usrdata/SavedLogs/Watering3Log.json"));

                LogLists.RecentEntries.Tank = 255;

                Globals.GpioServer.GpioLogEvent += DebugLogEvent;
                Globals.SpiServer.SpiLogEvent += DebugLogEvent;
                Globals.TwiServer.TwiLogEvent += DebugLogEvent;

                Globals.GpioServer.InitGPIO();
                Globals.SpiServer.InitSPI();
                Globals.TwiServer.InitTWIAsync();

                Globals.FastTask.Elapsed += new ElapsedEventHandler(BackgroundTaskController.FastTask_Routine);
                Globals.MainTask.Elapsed += new ElapsedEventHandler(BackgroundTaskController.MainTask_Routine);
                Globals.SaveTask.Elapsed += new ElapsedEventHandler(BackgroundTaskController.SaveTask_Routine);
                Globals.LevelLogger.Elapsed += new ElapsedEventHandler(BackgroundTaskController.LevelLogger_Routine);
                Globals.EnvLogger.Elapsed += new ElapsedEventHandler(BackgroundTaskController.EnvLogger_Routine);
                Globals.PowerLogger.Elapsed += new ElapsedEventHandler(BackgroundTaskController.PowerLogger_Routine);
                Globals.BufferCleanTask.Elapsed += new ElapsedEventHandler(BackgroundTaskController.BufferClean_Task);

                Globals.FastTask.Enabled = true;
                Globals.MainTask.Enabled = true;
                Globals.SaveTask.Enabled = true;
                Globals.LevelLogger.Enabled = true;
                Globals.EnvLogger.Enabled = true;
                Globals.PowerLogger.Enabled = true;
                Globals.BufferCleanTask.Enabled = true;

                Globals.IsInitialized = true;
            }

            return true;
        }

        private static void WatLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_BackgroundTaskController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_BackgroundTaskController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_BackgroundTaskController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_BackgroundTaskController.LogError(vMessage);
                    break;
                case LogType.Fatal:
                    Parents.Logger_BackgroundTaskController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_BackgroundTaskController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }
            vDetail = vDetail.Trim();
            vDetail = vDetail.Replace("\n", " ");
            vDetail = vDetail.Replace("\r", " ");
            vDetail = vDetail.Replace("\t", " ");
            vDetail = vDetail.Replace("  ", " ");
            vDetail = vDetail.Replace(Globals.IllegalCharacters[0], Globals.ReplaceCharacters[0]);
            //Globals.SqlLog_Data += String.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", DateTime.Now.ToString("o", CultureInfo.CurrentCulture), vInstance, vType, vMsg, vDetail, Globals.AppInDebug);
            AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[WAT]", vType, vMessage, vDetail);
        }

        private static void SysLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_BackgroundTaskController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_BackgroundTaskController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_BackgroundTaskController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_BackgroundTaskController.LogError(vMessage);
                    break;
                case LogType.Fatal:
                    Parents.Logger_BackgroundTaskController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_BackgroundTaskController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }
            vDetail = vDetail.Trim();
            vDetail = vDetail.Replace("\n", " ");
            vDetail = vDetail.Replace("\r", " ");
            vDetail = vDetail.Replace("\t", " ");
            vDetail = vDetail.Replace("  ", " ");
            vDetail = vDetail.Replace(Globals.IllegalCharacters[0], Globals.ReplaceCharacters[0]);
            //Globals.SqlLog_Data += String.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", DateTime.Now.ToString("o", CultureInfo.CurrentCulture), vInstance, vType, vMsg, vDetail, Globals.AppInDebug);
            AddJournal(DateTime.Now.ToString("o", CultureInfo.CurrentCulture), "[SYS]", vType, vMessage, vDetail);
        }

        public static void DebugLogEvent(object sender, LoggingEventArgs e)
        {
            e.Detail = e.Detail.Trim();
            e.Detail = e.Detail.Replace("\n", " ");
            e.Detail = e.Detail.Replace("\r", " ");
            e.Detail = e.Detail.Replace("\t", " ");
            e.Detail = e.Detail.Replace("  ", " ");
            e.Detail = e.Detail.Replace(Globals.IllegalCharacters[0], Globals.ReplaceCharacters[0]);
            //Globals.SqlLog_Data += String.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", e.TimeStamp.ToString("o", CultureInfo.CurrentCulture), e.Instance, e.Type, e.Message, e.Detail, Globals.AppInDebug);
            AddJournal(e.TimeStamp.ToString("o", CultureInfo.CurrentCulture), e.Instance, e.Type, e.Message, e.Detail);
        }

        public static void AddJournal(string TimeStamp, string App, string Type, string Message, string Details)
        {
            try
            {
                while (LogLists.JournalLog.Count > 10000) { LogLists.JournalLog.RemoveAt(0); }
                while (LogLists.JournalBuffer.Count > 250) { LogLists.JournalBuffer.RemoveAt(0); }

                LogLists.JournalLog.Add(new cJournal
                {
                    TimeStamp = TimeStamp,
                    App = App,
                    Type = Type,
                    Message = Message,
                    Details = Details
                });

                LogLists.JournalBuffer.Add(new cJournal
                {
                    TimeStamp = TimeStamp,
                    App = App,
                    Type = Type,
                    Message = Message,
                    Details = Details
                });
            }
            catch
            {
                Console.WriteLine($">>> Error adding Journal entry");
            }
        }
    }
}
