using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.Infrastructures
{
    public class DbContext
    {
        public monitoring_tour_v3Entities GetContext()
        {
            return new monitoring_tour_v3Entities();
        }
    }
}