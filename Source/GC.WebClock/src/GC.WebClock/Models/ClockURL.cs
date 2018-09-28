using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GC.WebClock.Utilities;

namespace GC.WebClock.Models
{
    public class ClockURL : ClocksList 
    {                       
        public string SilverLighClockURL { get; set; }

        public string HTMLClockURL { get; set; }

        public string StatusMessage { get; set; }

        public string StatusCode { get; set; }

        public bool IsAdmin { get; set; }

        public string RedirectURL { get; set; }

        public ClockURL()
        {
            EncodedSecurityCode = "";
            SilverLighClockURL = "";
            HTMLClockURL = "";
            StatusCode = "200";
            StatusMessage = "";
            IsAdmin = false;
            RedirectURL = "";
        }

        public ClockURL(string clockName,  string securityCode)
        {
            ClockName = clockName;            
            SecurityCode = securityCode;
            StatusCode = "200";
            StatusMessage = "";
            IsAdmin = false;
            RedirectURL = "";

        }

      
    }
}
