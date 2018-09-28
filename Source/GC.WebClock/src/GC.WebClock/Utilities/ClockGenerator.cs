using GC.WebClock.Models;
using GC.WebClock.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GC.WebClock.Utilities
{
    public class ClockGenerator
    {

        GC_WebClockContext _context;
        DBService dbService;
        //UserInfo userInfo;
        public ClockGenerator(GC_WebClockContext context)
        {
            _context = context;
            dbService = new DBService(_context);
            //userInfo = new UserInfo(_context, HttpContext, User);
        }
        /// <summary>
        /// Generate clock on basis of locationID for user
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public ClockURL GetClockURLObject(string locationID)
        {
            try
            {
                // validating if location is registered in DB or not. return error in case of invalid location ID
                if(dbService.ValidateClock(locationID)==false)
                {
                    ClockURL clockDTObject = new ClockURL();
                    clockDTObject.StatusCode = "101"; //Location is empty or not registered in DB
                    clockDTObject.StatusMessage = "Invalid Location";
                    return clockDTObject;
                }
               
                // getting device name and encoded security code form Database to generate URL's
                string clockName =  dbService.GetClockNameFromLocationId(locationID);                 
                string encodedSecurityCode = dbService.GetEncodedSecurityCode(locationID);               
                var securityCode = Util.DecodeSecurityCode(encodedSecurityCode);

                //generating SilverLight URL
                string baseURL = dbService.GetConfigurationProperties("SilverLightBaseURL");
                string urlParams = dbService.GetConfigurationProperties("SilverLightURLParams");
                string silverLightURL = Util.GetSilverLightURL(clockName, encodedSecurityCode, baseURL, urlParams);

                //generating HTMLClock URL
                string HTMLBaseURL = dbService.GetConfigurationProperties("HTMLBaseURL");
                string urlParamsHTML = dbService.GetConfigurationProperties("HTMLURLParams");
                string nsValue = dbService.GetConfigurationProperties("nsValue");
                string HTMLClockURL = Util.GetHTMLClocktURL(clockName, encodedSecurityCode, HTMLBaseURL, urlParamsHTML, nsValue);
                //setting values to return in a single object
                ClockURL clockDTO = new ClockURL(clockName, securityCode);                
                clockDTO.EncodedSecurityCode = encodedSecurityCode;
                clockDTO.SilverLighClockURL = silverLightURL;
                clockDTO.HTMLClockURL = HTMLClockURL;
                return clockDTO;
            }catch(Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return new ClockURL();
            }
           
        }
        /// <summary>
        /// Generate clock on basis of ID passed from dropdown for normal user
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public ClockURL GenerateClock(string locationID)
        {
            try
            {
                string clockName = "", securityCode = "", encodedSecurityCode = "";               

                //getiing clockName and Encoded sercuirty code from Database
                clockName =  dbService.GetClockNameFromLocationId(locationID) ;
                encodedSecurityCode = dbService.GetEncodedSecurityCode(locationID);
                encodedSecurityCode = String.IsNullOrEmpty(encodedSecurityCode) ? Util.GetEncodedSecurityCode(clockName) : encodedSecurityCode;
                securityCode = Util.DecodeSecurityCode(encodedSecurityCode);

                //getting SilverLight URL
                string baseURL = dbService.GetConfigurationProperties("SilverLightBaseURL");
                string urlParams = dbService.GetConfigurationProperties("SilverLightURLParams");
                string silverLightURL = Util.GetSilverLightURL(clockName, encodedSecurityCode, baseURL, urlParams);

                //getting HTMLClock URL
                string HTMLBaseURL = dbService.GetConfigurationProperties("HTMLBaseURL");
                string urlParamsHTML = dbService.GetConfigurationProperties("HTMLURLParams");
                string nsValue = dbService.GetConfigurationProperties("nsValue");
                string HTMLClockURL = Util.GetHTMLClocktURL(clockName, encodedSecurityCode, HTMLBaseURL, urlParamsHTML, nsValue);
                // returning object
                ClockURL clockDTO = new ClockURL(clockName, securityCode);
                clockDTO.EncodedSecurityCode = encodedSecurityCode;
                clockDTO.SilverLighClockURL = silverLightURL;
                clockDTO.HTMLClockURL = HTMLClockURL;

                return clockDTO;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return new ClockURL();
            }

        }
    }
}
