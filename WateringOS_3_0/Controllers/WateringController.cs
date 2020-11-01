using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WateringOS_3_0.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using System.Net;

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
    public class WateringController : Controller
    {
        public IActionResult Index() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Index()");            */return View(); }
        public IActionResult Journal() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Journal()");          */return View(); }
        public IActionResult Level() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Level()");            */return View(); }
        public IActionResult Environment() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Environment()");      */return View(); }
        public IActionResult Power() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Power()");            */return View(); }
        public IActionResult Watering() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Watering()");         */return View(); }
        public IActionResult Login() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Login()");            */return View(); }
        public IActionResult Privacy() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Privacy()");          */return View(); }

        [SimpleMembershipAttribute]
        public IActionResult ManualControl() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "ManualControl()");   */return View(); }
        [SimpleMembershipAttribute]
        public IActionResult Settings() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Settings()");        */return View(); }
        [SimpleMembershipAttribute]
        public IActionResult SystemSettings() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "SystemSettings()");  */return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() { /*LogRequesterIP(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "Error()\n");         */return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }

        public string DoLogin(string phrase, string returnUrl)
        {
            Parents.Logger_WateringController.LogDebug("Login was triggered (" + phrase + ")");
            if (Models.Authorization.GrantAccess(phrase))
            {
                Parents.Logger_WateringController.LogInformation("Login Authorized");
                HttpContext.Session.SetString("Authorized", "true");
                Uri vUri = new Uri(returnUrl);
                Parents.Logger_WateringController.LogDebug("Redirecting... " + HttpUtility.ParseQueryString(vUri.Query).Get("ReturnUrl"));
                return HttpUtility.ParseQueryString(vUri.Query).Get("ReturnUrl");
            }
            Parents.Logger_WateringController.LogWarning("WRONG PASSWORD - NOT AUTHORIZED");
            return returnUrl;
        }

        private void LogRequesterIP(string vIPaddress, string vTarget)
        {
            vIPaddress = vIPaddress.Replace("::ffff:", "");
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://ipinfo.io/" + vIPaddress + "/json");
                IPinfo IPresult = JsonConvert.DeserializeObject<IPinfo>(json);
                System.IO.File.AppendAllText("usrdata/IPaccess.csv", DateTime.Now.ToString() + ";" + IPresult.ip + ";" + vTarget + ";" + IPresult.country + ";" + IPresult.region + ";" + IPresult.city + ";" + IPresult.timezone + "\n");
            }
        }
    }

    public class IPinfo
    {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
        public string postal { get; set; }
        public string timezone { get; set; }
        public string readme { get; set; }
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            ip = "n/a";
            hostname = "n/a";
            city = "n/a";
            region = "n/a";
            country = "n/a";
            loc = "n/a";
            org = "n/a";
            postal = "n/a";
            timezone = "n/a";
            readme = "n/a";

            Console.WriteLine(errorContext.Error.ToString());
            errorContext.Handled = true;
        }

    }

    public class SimpleMembershipAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(filterContext.HttpContext.Session.GetString("Authorized")) || (filterContext.HttpContext.Session.GetString("Authorized") != "true"))
            {
                Parents.Logger_SimpleMembershipAttribute.LogWarning("Log in needed");
                string loginUrl = "/Watering/Login" + string.Format("?ReturnUrl={0}", filterContext.HttpContext.Request.GetEncodedUrl());
                filterContext.HttpContext.Response.Redirect(loginUrl, false);
            }
            else { Parents.Logger_SimpleMembershipAttribute.LogDebug("Login OK - proceed"); }
        }
    }
}
