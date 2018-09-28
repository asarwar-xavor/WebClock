using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GC.WebClock.Models
{
    public class DataProtectionKey
    {
        public int DataProtectionKeysId { get; set; }
        [Key]
        public string AuthKey { get; set; }
        public string XmlData { get; set; }
    }
}
