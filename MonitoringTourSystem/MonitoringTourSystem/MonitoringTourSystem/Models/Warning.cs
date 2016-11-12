using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.Models
{
    public class Warning
    {
        public string CategoryWarnig { get; set; }
        public double Distance { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string WarningName { get; set; }
        public string DescriptionWarning { get; set; }

        public List<string> ListTourGuideId { get; set; }

    }
}