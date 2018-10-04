using System;
using Microsoft.AspNetCore.Mvc;
using GC.WebClock.Models;
using GC.WebClock.Services;
using GC.WebClock.Utilities;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GC.WebClock.Controllers
{    
    
    public class WebClockApiController : Controller
    {
        GC_WebClockContext _context;
        DBService dbService;
       // public static dynamic allLocations;
        public WebClockApiController(GC_WebClockContext context)
        {
            _context = context;
            dbService = new DBService(_context);            
        }
        [HttpGet]
        [Route("WebClockApi/GetClockURL")]
        [Authorize]
        public ActionResult GetClockURL(int number)
        {
            try
            {
                UserInfo userInfo = new UserInfo(_context, HttpContext, User);
                bool isAdmin = userInfo.isAdmin();              
                if(isAdmin) //If user is admin, show admin panel to user by setting is admin property to true
                {
                    ClockURL dto = new ClockURL();
                    dto.IsAdmin = isAdmin;
                    return Json(dto);
                    
                }
                else
                {
                    //If user is not admin, try to find location of user from user info.
                    string locationID = "";
                    var userDetails = userInfo.GetUserInfo();
                    CustomLogging.InfoLog("UserInfo details: empID=" + userDetails.empID + "; locationID=" + userDetails.locationID +
                        "; jobCode=" + userDetails.jobCode + "; dept=" + userDetails.dept);
                    locationID = userDetails.locationID;                   
                    //IF location is not recieived from user info, try to find it from user agent string
                    if (locationID == "0" || locationID == null)
                    {
                        var userAgent = Request.Headers["User-Agent"].ToString();
                        locationID = Util.GetStoreFromUserAgent(userAgent);
                    }
                    //If location is not present in user agent string, show dropdown to user by sending empty object
                    if(locationID == "0" || String.IsNullOrEmpty(locationID))
                    {
                        ClockURL dtObject = new ClockURL();                      
                        return Json(dtObject);
                    }
                    // Now if location is present, generate URL's                   
                    ClockGenerator generatorObj = new ClockGenerator(_context);
                    ClockURL clockDTO = generatorObj.GetClockURLObject(locationID);

                    //setting Redirection Clock
                    string redirectionType = dbService.GetConfigurationProperties("WebClockType");
                    if (redirectionType.ToUpper() == "WEB")
                        clockDTO.RedirectURL = clockDTO.SilverLighClockURL;
                    else if (redirectionType.ToUpper() == "HTML")
                        clockDTO.RedirectURL = clockDTO.HTMLClockURL;
                    var result = Json(clockDTO);
                    return result;
                }                
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }

        [HttpGet]
        [Route("WebClockApi/GetClocksOfAllLocations")]
        [Authorize]
        public ActionResult GetClocksOfAllLocations(bool isPhysical)
        {
            try
            {
                dynamic allLocations = null;
                StoreInfo storeInfo = new StoreInfo(_context, HttpContext, User);
                if (allLocations==null)              
                    allLocations = storeInfo.GetAllStatesFromStoreInfo(isPhysical.ToString());               
                var result = Json(allLocations.Locations);
                return result;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }
        }

        /// <summary>
        /// To fill dropdown for normal user from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("WebClockApi/GetLocationsWithClocks")]
        [Authorize]
        public ActionResult GetLocationsWithClocks()
        {
            try
            {
                ADLocationsModel storeLocations = null;
                StoreInfo storeInfo = new StoreInfo(_context, HttpContext, User);
                storeLocations = storeInfo.GetAllStatesFromStoreInfo("all");
                ClocksListDTO[] allLocations = dbService.GetLocationsWithClocks(storeLocations);
                var result = Json(allLocations);
                return result;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }
        }
        [HttpGet]
        [Route("WebClockApi/GenerateClock")]
        [Authorize]
        public ActionResult GenerateClock(string location)
        {
            try
            {
                location = location.Trim();
                ClockGenerator generatorObj = new ClockGenerator(_context);
                ClockURL clockDTO = generatorObj.GenerateClock(location);
                //setting Redirection Clock
                string redirectionType = dbService.GetConfigurationProperties("WebClockType");                
                if(redirectionType.ToUpper() == "WEB")
                    clockDTO.RedirectURL =  clockDTO.SilverLighClockURL ;
                else if (redirectionType.ToUpper()=="HTML")
                    clockDTO.RedirectURL = clockDTO.HTMLClockURL;

                var result = Json(clockDTO);
                return result;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }
        /// <summary>
        /// To retrieve locations data from DB for data tables
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("WebClockApi/GetLocationsFromClocksList")]
        [Authorize]
        public ActionResult GetLocationsFromClocksList()
        {
            try
            {
                ADLocationsModel storeLocations = null;
                StoreInfo storeInfo = new StoreInfo(_context, HttpContext, User);
                storeLocations = storeInfo.GetAllStatesFromStoreInfo("all");                
                var locations = dbService.GetLocationsFromClocksList(storeLocations);
                var result = Json(locations);
                return result;              
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }
        }

        [HttpGet]
        [Route("WebClockApi/DeleteClock")]
        [Authorize]
        public ActionResult DeleteClock(int clockId)
        {
            try
            {
                bool isDeleted = dbService.DeleteClock(clockId);
                return Json(isDeleted);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }
        [HttpGet]
        [Route("WebClockApi/UpdateClock")]
        [Authorize]
        public ActionResult UpdateClock(int clockId, string location, string clockName,string securityCode, string locationType)
        {
            try
            {                
                string encodedSecurityCode = Util.GetEncodedSecurityCode(securityCode);
                bool isUpdated = dbService.UpdateClock(clockId, location,clockName, encodedSecurityCode, locationType);
                return Json(isUpdated);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }
        [HttpGet]
        [Route("WebClockApi/AddClock")]
        [Authorize]
        public ActionResult AddClock(string location, string locationName, string locType)
        {
            try
            {
                string securityPrefix = dbService.GetConfigurationProperties(locType);
                location = location.Trim();
                string clockType = locType.ToUpper() == "GC" ? dbService.GetConfigurationProperties("gcClockType") : dbService.GetConfigurationProperties("maaClockType");
                string clockName = clockType.ToLower() + location;
                string securityCode = securityPrefix.ToUpper() + location.ToUpper();
                var encodedSecurityCode = String.IsNullOrEmpty(securityCode) ? "" : Util.GetEncodedSecurityCode(securityCode);               
                bool isAdded = dbService.AddClock(location, clockName, locationName, encodedSecurityCode,"Standard", locType);
                return Json(isAdded);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }
        [HttpGet]
        [Route("WebClockApi/checkAdmin")]
        [Authorize]
        public ActionResult checkAdmin()
        {
            try
            {
                UserInfo userInfo = new UserInfo(_context, HttpContext, User);                
                return Json(userInfo.isAdmin().ToString());
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }      

        [HttpGet]
        [Route("WebClockApi/GetConfigurationProperties")]
        [AllowAnonymous]
        public string GetConfigurationProperties(string key)
        {
            try
            {
                return dbService.GetConfigurationProperties(key);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }
        [HttpGet]
        [Route("WebClockApi/AddExceptionClock")]
        [Authorize]
        public ActionResult AddExceptionClock(string location, string clockName, string locationName, string securityCode,string locationType)
        {
            try
            {
                var encodedSecurityCode =  Util.GetEncodedSecurityCode(securityCode);
                location = location.Trim();
                bool isAdded = dbService.AddClock(location, clockName, locationName, encodedSecurityCode,"Exception", locationType);
                return Json(isAdded);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return Json(false);
            }
        }
    }
}
