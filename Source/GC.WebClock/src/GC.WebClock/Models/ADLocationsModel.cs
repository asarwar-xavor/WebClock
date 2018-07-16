using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GC.WebClock.Models
{   
    [XmlRoot("NewDataSet")]
    public class ADLocationsModel
    {
        [XmlElement("LocInfo")]
        public List<ADLocations> Locations;
    }

    [XmlRoot("LocInfo")]
    public class ADLocations
    {
        //[XmlElement("LOCATION")]
        public string Value;
        [XmlElement("TITLE")]
        public string Label;
        [XmlElement("LOCATION_TYPE")]
        public string LocType;

        [XmlElement("LOCATION")]
        public string Location { get; set; }

    }
}
