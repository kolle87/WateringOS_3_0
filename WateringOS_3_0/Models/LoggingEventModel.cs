using System;
using System.Diagnostics;

/// LoggingEventArg Class for RaspberryPi 3
/// to be used with WateringOS >3.0
/// 
/// (C) Michael Kollmeyer, 02-May-2020
///

namespace WateringOS_3_0
{
    static class LogType
    {
        public const string Status = "Status";
        public const string Information = "Information";
        public const string Warning = "Warning";
        public const string Error = "Error";
        public const string Fatal = "Fatal";
    }

    public sealed class LoggingEventArgs
    {
        public DateTimeOffset TimeStamp { get; set; }
        public string Instance { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Detail {get; set; }   
    }

    public sealed class ApiQueryEventArgs
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
;