using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.RealTimeServer.Model
{
    public class WarningTourguide
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string WarningName { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> Tourist { get; set; }

    }
}