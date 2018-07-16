using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GC.WebClock.Models;
using GC.WebClock.Services;
using GC.WebClock.Utilities;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GC.WebClock.Controllers
{    
    public class HomeController : Controller
    {
        GC_WebClockContext _context;
        DBService dbService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(GC_WebClockContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            dbService = new DBService(_context);
        }
        [AllowAnonymous]    
        public IActionResult Index()
        {
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated == false)
                return RedirectToAction("Startup", "Home");
            return View();
        }
       [AllowAnonymous]
        public IActionResult Startup()
        {        
            string locationID = "";
            ViewData["RedirectURL"] = "";
            var userAgent = Request.Headers["User-Agent"].ToString();
            CustomLogging.InfoLog("User-Agent String = " + userAgent);
            locationID = Util.GetStoreFromUserAgent(userAgent);
            if (!String.IsNullOrEmpty(locationID))
            {
                // Now if location is present, generate URL's 
                ClockGenerator generatorObj = new ClockGenerator(_context);
                ClockURL clockDTO = generatorObj.GetClockURLObject(locationID);
                //Check if clock is registered or not.
                if(clockDTO.StatusCode == "101")
                {
                    ViewData["RedirectURL"] = clockDTO.StatusCode;
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewData["InvalidCredentials"] = "";
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["InvalidCredentials"] = "";
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }             
                else
                {
                    ViewData["InvalidCredentials"] = "Invalid Credentials";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public async Task<IActionResult> Logout(string type)
        {
            try
            {
                await _signInManager.SignOutAsync();
                ViewBag.Message = "You have successfully logged out.";               
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                ViewBag.Message = "You have successfully logged out.";
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult Error()
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
            return View();
        }
        [AllowAnonymous]
        public IActionResult ServerInfo()
        {            
            return View();
        }
        #region Helpers

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion     
    }
}
