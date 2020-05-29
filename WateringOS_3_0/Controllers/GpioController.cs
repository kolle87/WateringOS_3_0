using Microsoft.Extensions.Logging;
using System;
using System.Device.Gpio;
using WateringOS_3_0.Models;

/// GPIO Class for RaspberryPi 3
/// to be used with WateringOS >3.0
/// 
/// (C) Michael Kollmeyer, 02-May-2020
/// 

namespace WateringOS_3_0
{
    public class GPIOController
    {
        // Private Declarations
        private System.Device.Gpio.GpioController gpio;
        
        readonly byte PIN_DO_Pump      = 18; 
        readonly byte PIN_DO_Valve1    = 23; 
        readonly byte PIN_DO_Valve2    = 24; 
        readonly byte PIN_DO_Valve3    = 25; 
        readonly byte PIN_DO_Valve4    = 12; 
        readonly byte PIN_DO_Valve5    = 16; 
        readonly byte PIN_DI_PF5       =  6;  
        readonly byte PIN_DI_PF12      =  5;  
        readonly byte PIN_DI_PF24      =  4;  
        readonly byte PIN_DI_PG5       = 20;  
        readonly byte PIN_DI_PG12      = 19;  
        readonly byte PIN_DI_PG24      = 17;  
        readonly byte PIN_DI_WD_Warn   = 26;  
        readonly byte PIN_DO_WD_Enable = 27; 

        // Public Declarations
        public void StartPump()      { this.SetPin(PIN_DO_Pump,   true ); }
        public void StopPump()       { this.SetPin(PIN_DO_Pump,   false); }
        public void OpenValve1()     { this.SetPin(PIN_DO_Valve1, true ); }
        public void CloseValve1()    { this.SetPin(PIN_DO_Valve1, false); }
        public void OpenValve2()     { this.SetPin(PIN_DO_Valve2, true ); }
        public void CloseValve2()    { this.SetPin(PIN_DO_Valve2, false); }
        public void OpenValve3()     { this.SetPin(PIN_DO_Valve3, true ); }
        public void CloseValve3()    { this.SetPin(PIN_DO_Valve3, false); }
        public void OpenValve4()     { this.SetPin(PIN_DO_Valve4, true ); }
        public void CloseValve4()    { this.SetPin(PIN_DO_Valve4, false); }
        public void OpenValve5()     { this.SetPin(PIN_DO_Valve5, true ); }
        public void CloseValve5()    { this.SetPin(PIN_DO_Valve5, false); }
        public void EnableWD()       { this.SetPin(PIN_DO_WD_Enable, true ); }
        public void DisableWD()      { this.SetPin(PIN_DO_WD_Enable, false); }
        public bool WatchDog_Prewarn { get { return (this.GetPin(PIN_DI_WD_Warn)); } }
        public bool PowerGood_5V     { get { return (this.GetPin(PIN_DI_PG5    )); } }
        public bool PowerGood_12V    { get { return (this.GetPin(PIN_DI_PG12   )); } }
        public bool PowerGood_24V    { get { return (this.GetPin(PIN_DI_PG24   )); } }
        public bool PowerFail_5V     { get { return (this.GetPin(PIN_DI_PF5    )); } }
        public bool PowerFail_12V    { get { return (this.GetPin(PIN_DI_PF12   )); } }
        public bool PowerFail_24V    { get { return (this.GetPin(PIN_DI_PF24   )); } }
        public bool PumpActive       { get { return (this.GetPin(PIN_DO_Pump   )); } }
        public bool Valve1Open       { get { return (this.GetPin(PIN_DO_Valve1 )); } }
        public bool Valve2Open       { get { return (this.GetPin(PIN_DO_Valve2 )); } }
        public bool Valve3Open       { get { return (this.GetPin(PIN_DO_Valve3 )); } }
        public bool Valve4Open       { get { return (this.GetPin(PIN_DO_Valve4 )); } }
        public bool Valve5Open       { get { return (this.GetPin(PIN_DO_Valve5 )); } }
       
        private bool GetPin(byte Pin)
        {
            try
            {
                if (this.gpio.IsPinOpen(Pin))
                {
                    return (PinValue.High == this.gpio.Read(Pin));
                }
                else
                {
                    GpioLog(LogType.Error, "GPIO_DO pin not open", "The requested pin action is not available, since the pin is not open. Ensure the controller have been initialized.");
                    return false;
                }
            }
            catch (Exception e)
            {
                GpioLog(LogType.Error, "Error while reading IO pin status", e.Message);
                return false;
            }
        }

        private void SetPin(byte Pin, bool Active)
        {
            try
            {
                if (this.gpio.IsPinOpen(Pin))
                {
                    this.gpio.Write(Pin, (Active == PinValue.High));
                }
                else
                {
                    GpioLog(LogType.Error, "GPIO_DI pin not open", "The requested pin action is not available, since the pin is not open. Ensure the controller have been initialized.");
                }
            }
            catch (Exception e)
            {
                GpioLog(LogType.Error, "Error while setting IO pin status", e.Message);
            }
        }

        public void InitGPIO()
        {
            GpioLog(LogType.Information, "Start initialization", "The intialization of the Raspberry GPIO communication class has started.");
            try
            {
                this.gpio = new System.Device.Gpio.GpioController();

                this.gpio.OpenPin(PIN_DO_Pump,      PinMode.Output); // DO_Pump
                this.gpio.OpenPin(PIN_DO_Valve1,    PinMode.Output); // DO_Valve1
                this.gpio.OpenPin(PIN_DO_Valve2,    PinMode.Output); // DO_Valve2
                this.gpio.OpenPin(PIN_DO_Valve3,    PinMode.Output); // DO_Valve3
                this.gpio.OpenPin(PIN_DO_Valve4,    PinMode.Output); // DO_Valve4
                this.gpio.OpenPin(PIN_DO_Valve5,    PinMode.Output); // DO_Valve5
                this.gpio.OpenPin(PIN_DI_PF5,       PinMode.Input);  // DI_PF5
                this.gpio.OpenPin(PIN_DI_PF12,      PinMode.Input);  // DI_PF12
                this.gpio.OpenPin(PIN_DI_PF24,      PinMode.Input);  // DI_PF24
                this.gpio.OpenPin(PIN_DI_PG5,       PinMode.Input);  // DI_PG5
                this.gpio.OpenPin(PIN_DI_PG12,      PinMode.Input);  // DI_PG12
                this.gpio.OpenPin(PIN_DI_PG24,      PinMode.Input);  // DI_PG24
                this.gpio.OpenPin(PIN_DI_WD_Warn,   PinMode.Input);  // DI_WD_Warn
                this.gpio.OpenPin(PIN_DO_WD_Enable, PinMode.Output); // DO_WD_Enable

                this.SetPin(PIN_DO_Pump,   false); // DO_Pump
                this.SetPin(PIN_DO_Valve1, false); // DO_Valve1
                this.SetPin(PIN_DO_Valve2, false); // DO_Valve2
                this.SetPin(PIN_DO_Valve3, false); // DO_Valve3
                this.SetPin(PIN_DO_Valve4, false); // DO_Valve4
                this.SetPin(PIN_DO_Valve5, false); // DO_Valve5
            }
            catch (Exception e)
            {
                GpioLog(LogType.Error, "Error while GPIO Class Initialization", e.Message);
            }
        }
        private void GpioLog(string vType, string vMessage, string vDetail)
        {
            switch (vType)
            {
                case LogType.Status:
                    Parents.Logger_GPIOController.LogDebug(vMessage);
                    break;
                case LogType.Information:
                    Parents.Logger_GPIOController.LogInformation(vMessage);
                    break;
                case LogType.Warning:
                    Parents.Logger_GPIOController.LogWarning(vMessage);
                    break;
                case LogType.Error:
                    Parents.Logger_GPIOController.LogError(vMessage);
                    break;
                case LogType.Fatal:
                    Parents.Logger_GPIOController.LogCritical(vMessage);
                    break;
                default:
                    Parents.Logger_GPIOController.LogWarning("LOG TYPE UNKNOWN /// " + vMessage);
                    break;
            }
            
            LoggingEventArgs args = new LoggingEventArgs();
            args.TimeStamp = DateTime.Now;
            args.Instance = "[DIO]";
            args.Type = vType;
            args.Message = vMessage;
            args.Detail = vDetail;
            OnGpioLoggingEvent(args);
        }
        protected void OnGpioLoggingEvent(LoggingEventArgs e)
        {
            EventHandler<LoggingEventArgs> handler = GpioLogEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LoggingEventArgs> GpioLogEvent;

    }
}
