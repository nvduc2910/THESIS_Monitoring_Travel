using MonitoringTourSystem.Controllers;
using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.ViewModel
{
    public class TourDetailViewModel
    {
        public List<tour> ListTour { get; set; }
        public List<ScheduleDay> ListScheduleDay { get; set;}
        public tour TourItem {
            get;
            set;
        }
        public string TourGuideName { get; set; }

        public List<tour> ListTourVietNam { get; set; }
        public List<tour> ListTourForeign { get; set; }

    }
}