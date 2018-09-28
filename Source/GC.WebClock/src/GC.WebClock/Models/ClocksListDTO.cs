using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC.WebClock.Models
{
    public class ClocksListDTO:ClocksList
    {
        public string Value { get; set; }
        public string Label { get; set; }
        
       // public string SecurityCode { get; set; }
        public string sortColumn { get; set; }
        public string sortDirection { get; set; }
        public string searchValue { get; set; }
        public int TotalCount { get; set; }       

    }
}
