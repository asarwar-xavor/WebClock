using System;
using System.Collections.Generic;

namespace GC.WebClock.Models
{
    public partial class ClocksList
    {
        public int ClockId { get; set; }
        public string ClockName { get; set; }      
        public string EncodedSecurityCode { get; set; }
        public string Location { get; set; }
        public string ClockType { get; set; }
       // public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public string LocationName;
        public string SecurityCode;
    }
}
