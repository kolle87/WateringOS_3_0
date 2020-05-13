using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Timers;
using Microsoft.AspNetCore.Mvc;
using WateringOS_3_0.Models;
using Microsoft.VisualBasic.CompilerServices;

namespace WateringOS_3_0.Controllers
{
    public class BackgroundTaskController : Controller
    {
        private static void TestTimer_Routine(object sender, ElapsedEventArgs e)
        {
            if (Globals.vTest < 200)
            {
                Globals.vTest++;
            }
            else
            {
                Globals.vTest = 0;
            }
        }

        public static bool Initialize()
        {
            if (!Globals.IsInitialized)
            {
                Globals.GpioServer.GpioLogEvent += DebugLogEvent;
                Globals.GpioServer.InitGPIO();
                //await Task.Delay(200);

                Globals.TestTimer.Elapsed += new ElapsedEventHandler(BackgroundTaskController.TestTimer_Routine);
                Globals.TestTimer.Enabled = true;
                Globals.IsInitialized = true;
            }

            return true;
        }
        public static void DebugLogEvent(object sender, LoggingEventArgs e)
        {
            e.Detail = e.Detail.Trim();
            e.Detail = e.Detail.Replace("\n", " ");
            e.Detail = e.Detail.Replace("\r", " ");
            e.Detail = e.Detail.Replace("\t", " ");
            e.Detail = e.Detail.Replace("  ", " ");
            e.Detail = e.Detail.Replace(Globals.IllegalCharacters[0], Globals.ReplaceCharacters[0]);
            Globals.SqlLog_Data += String.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", e.TimeStamp.ToString("o", CultureInfo.CurrentCulture), e.Instance, e.Type, e.Message, e.Detail, Globals.AppInDebug);
        }
        public static void DebugLog(string vInstance, string vType, string vMsg, string vDetail)
        {
            vDetail = vDetail.Trim();
            vDetail = vDetail.Replace("\n", " ");
            vDetail = vDetail.Replace("\r", " ");
            vDetail = vDetail.Replace("\t", " ");
            vDetail = vDetail.Replace("  ", " ");
            vDetail = vDetail.Replace(Globals.IllegalCharacters[0], Globals.ReplaceCharacters[0]);
            Globals.SqlLog_Data += String.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", DateTime.Now.ToString("o", CultureInfo.CurrentCulture), vInstance, vType, vMsg, vDetail, Globals.AppInDebug);

        }
    }
}
