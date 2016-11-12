using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.ViewModel
{

    public class CreateTourViewModel
    {
        public List<place> ListPlace { get; set; }
        public List<tour_schedule> ListTourSchedule { get; set; }
        public List<string> ListVehical = new List<string>() { "Xe máy", "Xe công cộng", "Máy bay" };
        public string NameTour { get; set; }
        public List<tourguide> ListTourGuide { get; set; }
        public int TouristQuantity { get; set; }
        public string Place { get; set; }
        public int Day { get; set; }
        public DateTime DepartudeDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string DescriptionTour { get; set; }
        public string DescriptionScheduleTour { get; set; }

    }
}