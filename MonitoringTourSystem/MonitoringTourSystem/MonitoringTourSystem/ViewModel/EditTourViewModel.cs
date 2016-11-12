using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.ViewModel
{
    public class EditTourViewModel
    {

        public EditTourViewModel()
        {
            TourDetail = new TourDetailViewModel();
            ListProvienWithPlace = new List<ListProvienWithPlace>();
            ListTourGuide = new List<tourguide>();
            ListTourSchedule = new List<tour_schedule>();
        }

        public TourDetailViewModel TourDetail { get; set; }
        public List<ListProvienWithPlace> ListProvienWithPlace { get; set; }
        public List<tourguide> ListTourGuide { get; set; }
        public List<tour_schedule> ListTourSchedule { get; set; }
    }

    public class ListProvienWithPlace
    {
        public string ProvinceName { get; set; }
        public List<place> ListPlaceWithProvince { get; set; }
    }
}