using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WateringOS_3_0.Models;

namespace WateringOS_3_0.Controllers
{
    public class ManualController : Controller
    {
        // Manual Control Actions
        private bool SIM_MODE = false;

        public void CloseAll()    
        { 
            if (!SIM_MODE) 
            {
                Globals.GpioServer.CloseValve1();
                Globals.GpioServer.CloseValve2();
                Globals.GpioServer.CloseValve3();
                Globals.GpioServer.CloseValve4();
                Globals.GpioServer.CloseValve5();
            } 
            else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseAll()"); } 
        }
        public void ResetAll()    { if (!SIM_MODE) { Globals.SpiServer.SPI_Reset(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: ResetAll()"); } }

        public void StartPump()   { if (!SIM_MODE) { Globals.GpioServer.StartPump(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: StartPump()");  } }
        public void OpenValve1()  { if (!SIM_MODE) { Globals.GpioServer.OpenValve1(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: OpenValve1()"); } }
        public void OpenValve2()  { if (!SIM_MODE) { Globals.GpioServer.OpenValve2(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: OpenValve2()"); } }
        public void OpenValve3()  { if (!SIM_MODE) { Globals.GpioServer.OpenValve3(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: OpenValve3()"); } }
        public void OpenValve4()  { if (!SIM_MODE) { Globals.GpioServer.OpenValve4(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: OpenValve4()"); } }
        public void OpenValve5()  { if (!SIM_MODE) { Globals.GpioServer.OpenValve5(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: OpenValve5()"); } }

        public void StopPump()    { if (!SIM_MODE) { Globals.GpioServer.StopPump(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: StopPump()");    } }
        public void CloseValve1() { if (!SIM_MODE) { Globals.GpioServer.CloseValve1(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseValve1()"); } }
        public void CloseValve2() { if (!SIM_MODE) { Globals.GpioServer.CloseValve2(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseValve2()"); } }
        public void CloseValve3() { if (!SIM_MODE) { Globals.GpioServer.CloseValve3(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseValve3()"); } }
        public void CloseValve4() { if (!SIM_MODE) { Globals.GpioServer.CloseValve4(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseValve4()"); } }
        public void CloseValve5() { if (!SIM_MODE) { Globals.GpioServer.CloseValve5(); } else { Parents.Logger_ManualController.LogInformation("Action triggered: CloseValve5()"); } }
    }
}
