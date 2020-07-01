using System;
using System.Diagnostics;


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