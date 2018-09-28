using System;
using Microsoft.AspNetCore.Mvc;
using GC.WebClock.Models;
using GC.WebClock.Services;
using GC.WebClock.Utilities;
using Microsoft.AspNetCore.Authorization;


namespace GC.WebClock.Controllers
{    
    public class HomeController : Controller
    {
        GC_WebClockContext _context;
        DBService dbService;        
        public HomeController(GC_WebClockContext context)
        {
            _context = context;            
            dbService = new DBService(_context);
        }
        [AllowAnonymous]    
        public IActionResult Index()
        {
            //Redirects user to Startup to read userAgent string if request is anonymous (First hit)
            string storeFromQueryString = HttpContext.Request.Query["STORE"];
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated == false)
                return RedirectToAction("Startup",new { location = storeFromQueryString });
            return View();
        }
       [AllowAnonymous]
        public IActionResult Startup(string location)
        {        
            string locationID = "";
            ViewData["RedirectURL"] = "";
            ViewData["LocationID"] = "";
            var userAgent = Request.Headers["User-Agent"].ToString();
            CustomLogging.InfoLog("User-Agent String = " + userAgent);
            locationID = String.IsNullOrEmpty(location)? Util.GetStoreFromUserAgent(userAgent):location;
            if (!String.IsNullOrEmpty(locationID))
            {
                // Now if location is present, generate URL's 
                ClockGenerator generatorObj = new ClockGenerator(_context);
                ClockURL clockDTO = generatorObj.GetClockURLObject(locationID);                              
                //Check if clock is registered or not.
                if(clockDTO.StatusCode == "101")
                {
                    ViewData["RedirectURL"] = clockDTO.StatusCode;
                    ViewData["LocationID"] = locationID;
                }
                else
                {
                    //setting Redirection Clock
                    string redirectionType = dbService.GetConfigurationProperties("WebClockType");
                    if (redirectionType.ToUpper() == "WEB")
                        clockDTO.RedirectURL = clockDTO.SilverLighClockURL;
                    else if (redirectionType.ToUpper() == "HTML")
                        clockDTO.RedirectURL = clockDTO.HTMLClockURL;
                    ViewData["RedirectURL"] = clockDTO.RedirectURL;
                }               
            }
            return View();
        }                  
        [AllowAnonymous]
        public IActionResult Error(string location)
        {
            if(String.IsNullOrEmpty(location))
            {
                UserInfo userInfo = new UserInfo(_context, HttpContext, User);
                string locationID = "";
                var userDetails = userInfo.GetUserInfo();
                locationID = userDetails.locationID;
                if (locationID == "0" || locationID == null)
                {
                    var userAgent = Request.Headers["User-Agent"].ToString();
                    locationID = Util.GetStoreFromUserAgent(userAgent);
                }
                ViewBag.location = locationID;
            }
            else
            {
                ViewBag.location = location;
            }                       
            return View();
        }
        [AllowAnonymous]
        public IActionResult ServerInfo()
        {            
            return View();
        }     
    }
}
