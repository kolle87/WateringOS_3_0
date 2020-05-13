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

namespace WateringOS_3_0.Controllers
{
    public class WateringController : Controller
    {
        public IActionResult Index()         { return View(); }
        public IActionResult Journal()       { return View(); }
        public IActionResult Level()         { return View(); }
        public IActionResult Environment()   { return View(); }
        public IActionResult Power()         { return View(); }
        public IActionResult Watering()      { return View(); }
        public IActionResult Login()         { return View(); }
        public IActionResult Privacy()       { return View(); }

        [SimpleMembershipAttribute]
        public IActionResult ManualControl()  { return View(); }
        [SimpleMembershipAttribute]
        public IActionResult Settings()       { return View(); }
        [SimpleMembershipAttribute]
        public IActionResult SystemSettings() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }

        public string DoLogin(string phrase, string returnUrl)
        {
            Parents.Logger_WateringController.LogDebug("Login was triggered ("+phrase+")");
            if (Authorization.GrantAccess(phrase))
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
            } else { Parents.Logger_SimpleMembershipAttribute.LogDebug("Login OK - proceed"); }
        }
    }
}
