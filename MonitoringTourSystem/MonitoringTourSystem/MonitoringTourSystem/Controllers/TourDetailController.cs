using MonitoringTourSystem.Infrastructures.EntityFramework;
using MonitoringTourSystem.Models;
using MonitoringTourSystem.Services;
using MonitoringTourSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MonitoringTourSystem.Controllers
{
    [Authorize]
    public class TourDetailController : Controller
    {
        public readonly monitoring_tour_v3Entities MonitoringTourSystem = new monitoring_tour_v3Entities();
        public bool IsStartTourActive;
        protected ManagerServices _managerServices = new ManagerServices();


        public ActionResult Index()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);
            var listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();
            var listPlace = MonitoringTourSystem.places.ToList();
            var listTourVietNam = listTour.Where(x => x.is_foreign_tour == 0).ToList();
            var listTourForeign = listTour.Where(x => x.is_foreign_tour == 1).ToList();
            var model = new TourDetailViewModel() { ListTour = listTour, ListTourVietNam = listTourVietNam, ListTourForeign = listTourForeign, ListScheduleDay = null, TourGuideName = null, TourItem = null };
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult GetDetailTour(int id)
        {
            var listTour = MonitoringTourSystem.tours.ToList();
            var listPlace = MonitoringTourSystem.places.ToList();
            var listTourVietNam = listTour.Where(x => x.is_foreign_tour == 0).ToList();
            var listTourForeign = listTour.Where(x => x.is_foreign_tour == 1).ToList();

            if (!IsStartTourActive)
            {
                // Nothing
            }
            else
            {
                listTour = MonitoringTourSystem.tours.ToList();
            }
            int indexDay = 0;
            int indexStart = 0;
            List<ScheduleDay> ListScheduleDay = new List<ScheduleDay>();
            var listSchedule = (from schedule in MonitoringTourSystem.tour_schedule
                                where schedule.tour_id == id
                                select schedule).ToList();
           
            for (int i = 0; i < listSchedule.Count; i++)
            {
                var a = (listSchedule[i].time - listSchedule[indexStart].time).TotalHours;
                if ((listSchedule[i].time - listSchedule[indexStart].time).TotalHours >= 24 || (listSchedule[i].time.Day > listSchedule[indexStart].time.Day))
                {
                    var tourSchedule = new List<tour_schedule>();
                    for(int j = indexStart; j < i ; j++)
                    {
                        int place_id = Convert.ToInt32(listSchedule[j].place_id);
                        var image = listPlace.Where(x => x.place_id == place_id).First();
                        listSchedule[j].image = image.cover_photo;
                        tourSchedule.Add(listSchedule[j]);

                    }
                    ListScheduleDay.Add(new ScheduleDay() { TourSchedule = tourSchedule});
                    indexStart = i;
                    i = i - 1;
                }
            }
            var tourScheduleItem = new List<tour_schedule>();

            for (int j = indexStart; j < listSchedule.Count; j ++)
            {
                int place_id = Convert.ToInt32(listSchedule[j].place_id);
                var image = listPlace.Where(x => x.place_id == place_id).First();
                listSchedule[j].image = image.cover_photo;
                tourScheduleItem.Add(listSchedule[j]);    
            }
            ListScheduleDay.Add(new ScheduleDay() { TourSchedule = tourScheduleItem });

            for(int k = 0; k < ListScheduleDay.Count; k ++)
            {
                indexDay = indexDay + 1;
                ListScheduleDay[k].NumberDay = "NGÀY " + indexDay;
            }

            //Get ID Tour Guide of tour
            var idTourGuide = listTour.Where(x => x.tour_id == id).First().tourguide_id;

            var tourGuideName = (from tourGuide in MonitoringTourSystem.tourguides
                                where tourGuide.tourguide_id == idTourGuide
                                 select tourGuide).ToList();
            var touItem = listTour.Where(x => x.tour_id == id).First();

            var model = new TourDetailViewModel() { ListTour = listTour, ListTourVietNam = listTourVietNam, ListTourForeign = listTourForeign, TourItem = touItem, ListScheduleDay = ListScheduleDay, TourGuideName = tourGuideName[0].tourguide_name };
            return PartialView("Index", model);
        }

        [HttpGet]
        public ActionResult SearchTourVietNam(string id)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);

            List<tour> listTour; 

            if (id != null && id.Length > 2)
            {
                id = id.ToUpper();
                listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();
                var listTourSearch = (from item in listTour
                                      where item.tour_code.ToString().Contains(id) && item.is_foreign_tour == 0
                                      select item).ToList();
                var model = new TourDetailViewModel() { ListTourVietNam = listTourSearch };
                return PartialView("ListTourVietNam", model);
            } 
            else
            {
                listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();
                var listTourSearch = listTour.Where(x => x.is_foreign_tour == 0).ToList();
                var model = new TourDetailViewModel() { ListTourVietNam = listTourSearch };
                return PartialView("ListTourVietNam", model);
            }
        }

        [HttpGet]
        public ActionResult SearchTourForeign(string id)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);

            List<tour> listTour; 

            if (id != null && id.Length > 2)
            {
                listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();
                id = id.ToUpper();
                var listTourSearch = (from item in listTour
                                      where item.tour_code.ToString().Contains(id) && item.is_foreign_tour == 1
                                      select item).ToList();
                var model = new TourDetailViewModel() { ListTourForeign = listTourSearch };
                return PartialView("ListTourForeign", model);
            }
            else
            {
                listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();
                var listTourSearch = listTour.Where(x => x.is_foreign_tour == 1).ToList();
                var model = new TourDetailViewModel() { ListTourForeign = listTourSearch };
                return PartialView("ListTourForeign", model);
            }

        }

        [HttpPost]
        public ActionResult SearchByDateAndTown(string regionSearch, DateTime dateSearch)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);

            var listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId);

            if (regionSearch != null && dateSearch != null && regionSearch != "Chọn miền")
            {
                var listTourSearch = (from item in listTour
                                      where (item.departure_date < dateSearch && item.return_date > dateSearch) && item.is_foreign_tour == 0
                                      select item).ToList();
                var model = new TourDetailViewModel() { ListTourVietNam = listTourSearch };
                return PartialView("ListTourVietNam", model);

                using (var transaction = MonitoringTourSystem.Database.BeginTransaction())
                {

                }
            }
            else if(dateSearch != null)
            {
                var listTourSearch = (from item in listTour
                                      where (item.departure_date < dateSearch && item.return_date > dateSearch)
                                      select item).ToList();
                var model = new TourDetailViewModel() { ListTourVietNam = listTourSearch };
                return PartialView("ListTourVietNam", model);
            }
            return null;
        }

        [HttpGet]
        public ActionResult ActiveTour(string id)
        {
            var idInt = Convert.ToInt32(id);
            using (var context = new monitoring_tour_v3Entities())
            {
                var tour = (from item in context.tours
                            where item.tour_id == idInt
                            select item).First();
                tour.status = StatusTour.Running.ToString();
                context.SaveChanges();
                
            }
            IsStartTourActive = true;
            return GetDetailTour(idInt);
        }

        [HttpGet]
        public ActionResult DeleteTour(string id)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);

            var listTour = MonitoringTourSystem.tours.Where(s => s.manager_id == userId).ToList();

            try
            {
                var idInt = Convert.ToInt32(id);
                using (var context = new monitoring_tour_v3Entities())
                {

                    
                    var tour = (from item in context.tours
                                where item.tour_id == idInt
                                select item).First();
                    context.tours.Remove(tour);
                    context.SaveChanges();

                    var listSchedule = (from item in context.tour_schedule
                                        where item.tour_id == idInt
                                        select item).ToList();

                    foreach(var itemSchedule in listSchedule)
                    {
                        context.tour_schedule.Remove(itemSchedule);
                        context.SaveChanges();
                    }

                    listTour.Remove(tour);
                }

                var model = new TourDetailViewModel() { ListTour = listTour};
                return PartialView("Index", model);
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }
        }

        public ActionResult EditTour(string id)
        {
            return RedirectToAction(id, "EditTour/EditTour");
        }
    }
    public class ScheduleDay
    {
        public string NumberDay { get; set; }
        public List<tour_schedule> TourSchedule { get; set; }       
        public string Image { get; set; }
        public string NameProvince { get; set; }
    }
}
