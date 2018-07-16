using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GC.WebClock.Models
{
    [XmlRoot("myInfo")]
    public class UserInfoModel
    {
        public string empID;
        public string fullName;
        public string jobTitle;
        public string location;
        public string locationID;
        public string jobCode;
        public string jobCategory;
        public string dept;
        public string unitName;
        public string dsmName;
        public string forgotPasswordRegistered;
        public bool IsRetail;
        public bool IsSupportCenter;
        public UserInfoModel()
        {
            empID = "0";
            fullName = "n/a";
            jobTitle = "n/a";
            location = "n/a";
            locationID = "0";
            jobCode = "0";
            jobCategory = "n/a";
            dept = "0";
            unitName = "n/a";
            dsmName = "n/a";
            forgotPasswordRegistered = "n/a";
            IsRetail = false;
            IsSupportCenter = false;
        }
    }
}
